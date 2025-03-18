using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField]
    Transform target;

    [Header("Config")]
    [SerializeField]
    float smoothing = 0.1f;

    [SerializeField]
    float mouseOffsetEffect = 5.0f;

    Vector3 tempVec;

    Vector3 mousePos;

    InputHandler inputHandler;

    //TODO : Link up to event system to add cam shake
    //TODO : Potentially make a mode where the thing goes infront of the player?
    /*private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }*/

    void Start()
    {
        inputHandler = InputHandler.Instance;
    }

    void Update()
    {
        //TODO : Maybe add hotkey to toggle this?
        if (inputHandler.IsMouseEnabled == true)
        {
            mousePos = inputHandler.GetMousePos();

            mousePos.x = (mousePos.x-Screen.width/2)/Screen.width;
            mousePos.y = (mousePos.y-Screen.height/2)/Screen.height;

            mousePos.Normalize();
            mousePos *= mouseOffsetEffect;
       }
        else
        {
            mousePos = Vector2.zero;
        }


        tempVec = target.position;
        tempVec.z = transform.position.z;

        transform.position = MathHelper.DampVec3(transform.position, tempVec + mousePos, smoothing, Time.deltaTime);
    }

    //TODO : Make this work probably have some set of random values this lerps through
    void screenShake(float amount)
    {

    }
}
