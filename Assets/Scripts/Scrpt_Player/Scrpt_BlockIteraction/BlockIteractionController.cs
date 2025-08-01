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

        float placingCoolDown = 0;
        float destroyingCoolDown = 0;

        List<Vector3> listOfStepPoints;

        Vector3Int drawWhereHit;

        void Start()
        {
            inputHandler = InputHandler.Instance;
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

            if (drawWhereHit != null)
            {
                Gizmos.DrawCube(drawWhereHit, Vector3.one*0.1f);
            }
        }


        void Update()
        {
            if (inputHandler.IsMouseEnabled == true)
            {
                Vector3 mousePos = inputHandler.GetMousePos();
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                Vector3Int hitVal;
                bool boolHitBlock;


                //only draw block placement preview when block is held
                drawWhereHit = TileMapHelperFunc.DDARayCheck(placeTileMap, transform.position, mousePos, maxRange, out hitVal, out boolHitBlock, out listOfStepPoints);

                Quaternion destroyRotation = Quaternion.Euler(0, 0, Mathf.Round((Mathf.Rad2Deg * Mathf.Atan2(transform.position.y-mousePos.y, transform.position.x - mousePos.x)) / 90f)*90f + 90f);

                //only draw breakable preview when tool is held
                if (playerInfo.heldType == "Tool")
                {
                    playerBlockView.SetLookAtObject(drawWhereHit, hitVal, destroyRotation, null, false);

                    //only break item when tool is held
                    if (inputHandler.IsAttacking && destroyingCoolDown <= 0)
                    {
                        destroyingCoolDown = destroyingMinCoolDown;
                        //placeTileMap.SetTile(drawWhereHit, null);
                        ChunkManager.SetTile(drawWhereHit, null, true);
                    }

                }

                //draw block preview when block is held

                //Debug.Log(drawWhereHit);

                if (inputHandler.IsPlacing && placingCoolDown <= 0)
                {
                    placingCoolDown = placingMinCoolDown;
                    
                    //placeTileMap.SetTile(hitVal, tileToPlace);
                    ChunkManager.SetTile(hitVal, tileToPlace, true);
                }



                placingCoolDown = Mathf.Max(placingCoolDown - Time.deltaTime, 0);
                destroyingCoolDown = Mathf.Max(destroyingCoolDown - Time.deltaTime, 0);
            }
        }
    }
}
