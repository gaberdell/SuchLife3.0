using UnityEngine;
using UnityEngine.Tilemaps;
using TileMapHelper;
using System.Collections.Generic;
 
namespace BlockIteraction
{
    public class BlockIteractionController : MonoBehaviour
    {
        [Header("TileMaps")]
        [SerializeField]
        Tilemap placeTileMap;
 
        [SerializeField]
        string placeTileMapTag = "PlaceTileMap";
 
        [SerializeField]
        BlockIteractionView playerBlockView;
 
        InputHandler inputHandler;
 
        [SerializeField]
        TileBase tileToPlace;
 
        [Header("Config")]
        [SerializeField]
        float maxRange = 6.0f;
 
        [SerializeField]
        float placingMinCoolDown = 0.1f;
 
        [SerializeField]
        float destroyingMinCoolDown = 0.1f;
 
        //cooldowns are per hand (index with (int)Hand)
        readonly float[] placingCoolDown = new float[2];
        readonly float[] destroyingCoolDown = new float[2];
 
        List<Vector3> listOfStepPoints;
 
        Vector3Int drawWhereHit;
 
        [SerializeField]
        GameObject droppedBlockItem;
 
        private PlayerInfo playerInfo;
        private PlayerInventory playerInventory;
 
        void Start()
        {
            inputHandler = InputHandler.Instance;
            playerInfo = GetComponent<PlayerInfo>();
            playerInventory = GetComponent<PlayerInventory>();
        }
 
        private void OnEnable() {
            EventManager.LocalGameObjectPlayerAddedToScene += LinkToScenesTileMapAndBlockInteraction;
        }
 
        private void OnDisable() {
            EventManager.LocalGameObjectPlayerAddedToScene -= LinkToScenesTileMapAndBlockInteraction;
        }

        void LinkToScenesTileMapAndBlockInteraction(GameObject player) {
            playerBlockView = FindAnyObjectByType<BlockIteractionView>();
            placeTileMap = GameObject.FindGameObjectWithTag(placeTileMapTag).GetComponent<Tilemap>();
        }
        private void OnDrawGizmos()
        {
            if (listOfStepPoints != null)
            {
                foreach (var item in listOfStepPoints)
                {
                    Gizmos.DrawSphere(item, 0.05f);
                }
            }
 
            Gizmos.DrawCube(drawWhereHit, Vector3.one*0.1f);
        }
 
 
        void Update()
        {
            //Null guard because it takes a sec for localGameObjectPlayerAddedToScene to be called
            if (inputHandler.IsMouseEnabled == true && playerBlockView != null)
            {
                Vector3 mousePos = inputHandler.GetMousePos();
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);
 
                Vector3Int hitVal;
                bool boolHitBlock;
 
 
                //only draw block placement preview when block is held
                drawWhereHit = TileMapHelperFunc.DDARayCheck(placeTileMap, transform.position, mousePos, maxRange, out hitVal, out boolHitBlock, out listOfStepPoints);
                Quaternion destroyRotation = Quaternion.Euler(0, 0, Mathf.Round((Mathf.Rad2Deg * Mathf.Atan2(transform.position.y-mousePos.y, transform.position.x - mousePos.x)) / 90f)*90f + 90f);
 
                //will this work
                playerBlockView.SetLookAtObject(drawWhereHit, hitVal, destroyRotation, null, false);
 
                InputHandler.SelectedContext leftContext = InputHandler.GetSelectedContext(Hand.Left);
                InputHandler.SelectedContext rightContext = InputHandler.GetSelectedContext(Hand.Right);
 
                bool anyTool = leftContext == InputHandler.SelectedContext.Tool
                            || rightContext == InputHandler.SelectedContext.Tool;
                bool anyBlock = leftContext == InputHandler.SelectedContext.Block
                             || rightContext == InputHandler.SelectedContext.Block;
 
                //only draw breakable preview when a tool is held (in either hand)
                if (anyTool)
                {
                    playerBlockView.SetLookAtObject(drawWhereHit, hitVal, destroyRotation, null, false);
 
                    //each hand holding a tool breaks with its own click
                    TryBreak(Hand.Left);
                    TryBreak(Hand.Right);
                } else
                {
                    //otherwise clear breakable preview 
                    playerBlockView.ClearGhost(false);
                }
 
                //draw block preview when a block is held (in either hand)
                if (anyBlock)
                {
                    playerBlockView.SetLookAtObject(drawWhereHit, hitVal, destroyRotation, null, true);
 
                    //each hand holding a block places with its own click, but only one block can be placed per frame
                    if (!TryPlace(Hand.Left, hitVal))
                    {
                        TryPlace(Hand.Right, hitVal);
                    }
                } else
                {
                    //otherwise clear block place preview
                    playerBlockView.ClearGhost(true);
                }
 
                for (int h = 0; h < 2; h++)
                {
                    placingCoolDown[h] = Mathf.Max(placingCoolDown[h] - Time.deltaTime, 0);
                    destroyingCoolDown[h] = Mathf.Max(destroyingCoolDown[h] - Time.deltaTime, 0);
                }
            }
        }
 
        private void TryBreak(Hand hand)
        {
            int h = (int)hand;
 
            if (InputHandler.GetSelectedContext(hand) != InputHandler.SelectedContext.Tool) return;
            if (!inputHandler.IsAttacking(hand) || destroyingCoolDown[h] > 0) return;
 
            destroyingCoolDown[h] = destroyingMinCoolDown;
            //placeTileMap.SetTile(drawWhereHit, null);
            //create block item object to pick up (slap in fix for demo; would use a droppable item class in the future)
            //check if a block was actually destroyed
            if (ChunkManager.GetTile(drawWhereHit, true) != null)
            {
                Instantiate(droppedBlockItem, drawWhereHit + new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity);
            }
            ChunkManager.SetTile(drawWhereHit, null, true);
        }
 
        //returns true if a block was placed
        private bool TryPlace(Hand hand, Vector3Int hitVal)
        {
            int h = (int)hand;
 
            if (InputHandler.GetSelectedContext(hand) != InputHandler.SelectedContext.Block) return false;
            if (!inputHandler.IsAttacking(hand) || placingCoolDown[h] > 0) return false;
 
            placingCoolDown[h] = placingMinCoolDown;
 
            //determine block to place based on the item in this hand
            Item item = playerInfo.GetHeldItem(hand);
            if (item == null || item.tileThisPlaces == null)
            {
                Debug.Log("trying to place a block when an item without a valid tileThisPlaces attribute is selected!");
                return false;
            }
            tileToPlace = item.tileThisPlaces;
 
            //placeTileMap.SetTile(hitVal, tileToPlace);
            ChunkManager.SetTile(hitVal, tileToPlace, true);
 
            //remove placed block from inventory
            playerInventory.RemoveItem(playerInfo.GetHeldItemIndex(hand));
            return true;
        }
    }
}