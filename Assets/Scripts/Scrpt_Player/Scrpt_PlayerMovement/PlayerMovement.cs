using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Header("Input")]
    [SerializeField]
    PlayerInput input;

    [SerializeField] 
    float deadZone = 0.1f; //When we start to apply stopping friction

    [Header("Speed Values")]
    [SerializeField] 
    float maxSpeed = 5f;
    [SerializeField] 
    float accel = 27f;
    [SerializeField] 
    float stopingAccel = 20f;

    [Header("Collision")]
    [SerializeField] 
    CircleCollider2D plrCollider;
    [SerializeField] 
    Rigidbody2D rigidBody;

    [Header("Rotation")]
    [SerializeField]
    float rotationSpeed = 360.0f;

    [SerializeField]
    float rotationAccel = 360.0f;

    InputHandler inputHandler;

    float previousRotationZ;
    float goalRotationZ;
    float moveTowardsVal;

    Vector3 plrVelocity;

    Vector3 previousPosition;
    List<ContactPoint2D> contactPoints;

    bool isDebugOn = false;
    bool debugHitDeadZone = false;
    void Start()
    {
        plrCollider = plrCollider ? plrCollider : GetComponent<CircleCollider2D>();
        rigidBody = rigidBody ? rigidBody : GetComponent<Rigidbody2D>();

        contactPoints = new List<ContactPoint2D>();

        previousPosition = transform.position;

        inputHandler = InputHandler.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
        handleRotation();
    }

    private void handleMovement()
    {
        Vector3 plrMoveInput = inputHandler.MoveInput;

        if (plrMoveInput.magnitude > deadZone)
        {
            plrVelocity = Vector3.ClampMagnitude(plrVelocity + plrMoveInput * accel * Time.deltaTime, maxSpeed);
        }
        else
        {
            Vector3 slowDownVector = plrVelocity.normalized * stopingAccel * Time.deltaTime;


            plrVelocity.x = plrVelocity.x > 0 ? Mathf.Max(plrVelocity.x - slowDownVector.x, 0f) :
                                                    Mathf.Min(plrVelocity.x - slowDownVector.x, 0f);

            plrVelocity.y = plrVelocity.y > 0 ? Mathf.Max(plrVelocity.y - slowDownVector.y, 0f) :
                                                    Mathf.Min(plrVelocity.y - slowDownVector.y, 0f);
        }


        transform.position += plrVelocity * Time.deltaTime;
    }

    private void handleRotation()
    {
        if (inputHandler.IsMouseEnabled == true)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            goalRotationZ = Mathf.Rad2Deg * Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) + 90f;
        }
        else
        {
            goalRotationZ = plrVelocity.magnitude == 0.0f ? goalRotationZ : 90f + Mathf.Rad2Deg * Mathf.Atan2(plrVelocity.y, plrVelocity.x);
        }

        float num = Mathf.Repeat(goalRotationZ - transform.rotation.eulerAngles.z, 360f);
        goalRotationZ = Mathf.Repeat(goalRotationZ, 360f);
        if (num > 180f)
        {
            num -= 360f;
        }

        if (goalRotationZ > 180f)
        {
            goalRotationZ -= 360f;
        }

        if (Mathf.Repeat(Mathf.Abs(num),360f) < 1f) {
            moveTowardsVal = 0f;
        }
        else
        {
            if (num < 0)
            {
                moveTowardsVal = Mathf.Max(moveTowardsVal - rotationAccel * Time.deltaTime, -rotationSpeed);
            }
            else
            {
                moveTowardsVal = Mathf.Min(moveTowardsVal + rotationAccel * Time.deltaTime, rotationSpeed);
            }

            //moveTowardsVal = Mathf.Clamp(moveTowardsVal + rotationAccel * (Mathf.Abs(num) + rotationAccel) * Time.deltaTime, -rotationSpeed, rotationSpeed);
        }


        float clampedCurrentVal = Mathf.Repeat(transform.rotation.eulerAngles.z, 360f);
        if (clampedCurrentVal > 180f)
        {
            clampedCurrentVal -= 360f;
        }


        //if (moveTowardsVal > 0)
        //{
        bool valIsLessThanGoal = clampedCurrentVal < goalRotationZ;
        bool hadToBeFlipped = false;

        float newMoveTowardsVal = clampedCurrentVal + moveTowardsVal * Time.deltaTime;

        if (newMoveTowardsVal > 180f)
        {
            hadToBeFlipped = true;
            newMoveTowardsVal -= 360f;
        }
        if (newMoveTowardsVal < -180f)
        {
            hadToBeFlipped = true;
            newMoveTowardsVal += 360f;
        }

        bool newValIsLessThanGoal = newMoveTowardsVal < goalRotationZ;

        bool didNotOverShoot = (!hadToBeFlipped && valIsLessThanGoal == newValIsLessThanGoal) || (hadToBeFlipped && valIsLessThanGoal != newValIsLessThanGoal);

        Debug.DrawLine(new Vector3(-1f, 0f), new Vector3(2f, 0f), valIsLessThanGoal ? Color.blue : Color.red);
        Debug.DrawLine(new Vector3(-1f, 1f), new Vector3(2f, 1f), newValIsLessThanGoal ? Color.blue : Color.red);
        Debug.Log(goalRotationZ);

        transform.rotation = Quaternion.Euler(0f, 0f, didNotOverShoot ? newMoveTowardsVal : goalRotationZ);
        //}

        //Debug.Log(goalRotationZ);
        //transform.rotation = Quaternion.Euler(0f, 0f, transform.rotation.eulerAngles.z + moveTowardsVal * Time.deltaTime);

        previousRotationZ = goalRotationZ;
    }

}
