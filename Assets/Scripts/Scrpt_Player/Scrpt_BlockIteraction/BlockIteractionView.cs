using UnityEngine;
using UnityEngine.Tilemaps;
using TileMapHelper;

namespace BlockIteraction
{
    public class BlockIteractionView : MonoBehaviour
    {

        [Header("Config")]
        [SerializeField]
        GameObject destroyBlockGhost;

        [SerializeField]
        GameObject placeBlockGhost;

        [SerializeField]
        GameObject player;

        [SerializeField]
        float trnsGoalPlace = 0.5f;

        [SerializeField]
        float trnsGoalVrnc = 0.1f;


        float trnsGoal = 0.0f;

        SpriteRenderer lookAtObjectSprite;

        SpriteRenderer placeBlockGhostSprite;

        InputHandler inputHandler;

        Vector3 offsetVector = new Vector3(0.5f, 0.5f);

        public void SetLookAtObject(Vector3 stayPosition, Vector3 blockPosition, Quaternion destroyRotation, Sprite placeSprite, bool placingBlocks)
        {
            //decide whether to display place preview or destroy preview based on input
            stayPosition.z = destroyBlockGhost.transform.position.z;
            if (placingBlocks)
            {
                placeBlockGhost.transform.position = blockPosition + offsetVector;
            } else
            {
                destroyBlockGhost.transform.position = stayPosition + offsetVector;
                destroyBlockGhost.transform.rotation = destroyRotation;
            }



        }


        void Start()
        {
            lookAtObjectSprite = destroyBlockGhost.GetComponent<SpriteRenderer>();

            placeBlockGhostSprite = placeBlockGhost.GetComponent<SpriteRenderer>();

            inputHandler = InputHandler.Instance;
        }


        void Update()
        {
            if (inputHandler.IsMouseEnabled == true)
            {
                Vector3 mousePos = inputHandler.GetMousePos();
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            }

            if (destroyBlockGhost.transform.localPosition.magnitude <= 2f)
            {
                trnsGoal = 0.0f;
            }
            else
            {
                trnsGoal = trnsGoalPlace + trnsGoalVrnc * Mathf.Sin(Time.time);
            }

            lookAtObjectSprite.color = new Color(1f, 1f, 1f, MathHelper.Damp(lookAtObjectSprite.color.a, trnsGoal, 0.5f, Time.deltaTime));
            placeBlockGhostSprite.color = new Color(1f, 1f, 1f, lookAtObjectSprite.color.a);
        }
    }
}
