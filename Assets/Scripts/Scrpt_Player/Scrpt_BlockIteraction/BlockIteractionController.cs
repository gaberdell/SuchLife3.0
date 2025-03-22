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

        Vector3 drawWhereHit;

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
                    Gizmos.DrawSphere(item, 0.5f);
                }
            }

            if (drawWhereHit != null)
            {
                Gizmos.DrawCube(drawWhereHit, Vector3.one);
            }
        }


        void Update()
        {
            if (inputHandler.IsMouseEnabled == true)
            {
                Vector3 mousePos = inputHandler.GetMousePos();
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                Vector3 hitVal;
                bool boolHitBlock;


                drawWhereHit = TileMapHelperFunc.DDARayCheck(placeTileMap, transform.position, mousePos, maxRange, out hitVal, out boolHitBlock, out listOfStepPoints);


                playerBlockView.SetLookAtObject(placeTileMap.WorldToCell(drawWhereHit), placeTileMap.WorldToCell(hitVal), null);

                if (inputHandler.IsPlacing && placingCoolDown <= 0)
                {
                    placingCoolDown = placingMinCoolDown;
                    placeTileMap.SetTile(placeTileMap.WorldToCell(hitVal), tileToPlace);
                }

                if (inputHandler.IsAttacking && destroyingCoolDown <= 0)
                {
                    destroyingCoolDown = destroyingMinCoolDown;
                    placeTileMap.SetTile(placeTileMap.WorldToCell(drawWhereHit), null);
                }

                placingCoolDown = Mathf.Max(placingCoolDown - Time.deltaTime, 0);
                destroyingCoolDown = Mathf.Max(destroyingCoolDown - Time.deltaTime, 0);
            }
        }
    }
}
