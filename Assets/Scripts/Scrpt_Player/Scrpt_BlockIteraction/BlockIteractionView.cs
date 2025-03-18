using UnityEngine;
using UnityEngine.Tilemaps;

namespace BlockIteraction
{
    public class BlockIteractionView : MonoBehaviour
    {

        [Header("Config")]
        [SerializeField]
        GameObject lookAtObject;

        [SerializeField]
        GameObject player;

        [SerializeField]
        float trnsGoalPlace = 0.5f;

        [SerializeField]
        float trnsGoalVrnc = 0.1f;


        float trnsGoal = 0.0f;

        SpriteRenderer lookAtObjectSprite;

        InputHandler inputHandler;

        Vector3 offsetVector = new Vector3(0.5f, 0.5f);

        public void SetLookAtObject(Vector3 stayPosition, Sprite placeSprite)
        {
            stayPosition.z = lookAtObject.transform.position.z;
            lookAtObject.transform.position = stayPosition + offsetVector;
        }


        void Start()
        {
            lookAtObjectSprite = lookAtObject.GetComponent<SpriteRenderer>();

            inputHandler = InputHandler.Instance;
        }


        void Update()
        {
            if (inputHandler.IsMouseEnabled == true)
            {
                Vector3 mousePos = inputHandler.GetMousePos();
                mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            }

            if (lookAtObject.transform.localPosition.magnitude <= 2f)
            {
                trnsGoal = 0.0f;
            }
            else
            {
                trnsGoal = trnsGoalPlace + trnsGoalVrnc * Mathf.Sin(Time.time);
            }

            lookAtObjectSprite.color = new Color(1f, 1f, 1f, MathHelper.Damp(lookAtObjectSprite.color.a, trnsGoal, 0.5f, Time.deltaTime));
        }
    }
}
