using UnityEngine;
using UnityEngine.Tilemaps;


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

        void Start()
        {
            inputHandler = InputHandler.Instance;
        }


        void Update()
        {
            if (inputHandler.IsMouseEnabled == true)
            {
                Vector3 mousePos = inputHandler.GetMousePos();
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

                playerBlockView.SetLookAtObject(mousePos, null);

            }
        }
    }
}
