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

    InputHandler inputHandler;

    float previousRotationZ;
    float goalRot;
    float moveTowardsVal;

    Vector3 plrVelocity;

    Vector3 debugPoint;

    Vector3 previousPosition;
    List<ContactPoint2D> contactPoints;

    void Start()
    {
        plrCollider = plrCollider ? plrCollider : GetComponent<CircleCollider2D>();
        rigidBody = rigidBody ? rigidBody : GetComponent<Rigidbody2D>();

        contactPoints = new List<ContactPoint2D>();

        previousPosition = transform.position;

        inputHandler = InputHandler.Instance;
    }

    private void FixedUpdate()
    {
        EventManager.SetPlayerAnimSpeedTrue(plrVelocity.magnitude);
    }

    void Update()
    {
        handleMovement();
        handleRotation();
        //handleCollision();
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

        previousPosition = transform.position;
        rigidBody.linearVelocity = plrVelocity;
        //transform.position += plrVelocity * Time.deltaTime;
    }

    private void handleRotation()
    {
        if (inputHandler.IsMouseEnabled == true)
        {
            Vector3 mousePos = inputHandler.GetMousePos();
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);

            goalRot = Mathf.Rad2Deg * Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) + 90f;
        }
        else
        {
            goalRot = plrVelocity.magnitude == 0.0f ? goalRot : 90f + Mathf.Rad2Deg * Mathf.Atan2(plrVelocity.y, plrVelocity.x);
        }

        goalRot = MathHelper.RotationTo180Scale(goalRot);
        
        float goalRotationDif = MathHelper.RotationTo180Scale(goalRot - transform.rotation.eulerAngles.z);

        float absGoalRotDif = Mathf.Abs(goalRotationDif);
        moveTowardsVal = absGoalRotDif < 1f ? 0f :
                            rotationSpeed * Mathf.Sign(goalRotationDif) * MathHelper.BezierBuffClamped(absGoalRotDif / 180f);

        float clampedCurrentRot = MathHelper.RotationTo180Scale(transform.rotation.eulerAngles.z);


        bool valIsLessThanGoal = clampedCurrentRot < goalRot;
        bool hadToBeFlipped = false;

        float newMoveTowardsVal = clampedCurrentRot + moveTowardsVal * Time.deltaTime;

        //Did overflipping happen if so change it so we are back in the [-180, 180] range
        //P.S. yes technically we want it in [-180, 180) but we don't really care if 180 slips in
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

        bool newValIsLessThanGoal = newMoveTowardsVal < goalRot;


        //Logic behind this is that if before its rotation being changed if the value was
        //less than the goal angle than after being flipped if it did not overshoot or
        //go from -180 to 180 then it should still be less. Ortherwise we know we overshot
        //and we have missed the goal
        bool didNotOverShoot = (!hadToBeFlipped && valIsLessThanGoal == newValIsLessThanGoal) || (hadToBeFlipped && valIsLessThanGoal != newValIsLessThanGoal);

        transform.rotation = Quaternion.Euler(0f, 0f, didNotOverShoot ? newMoveTowardsVal : goalRot);

        previousRotationZ = goalRot;

        //Some Debug lines
        Debug.DrawLine(new Vector3(-1f, 0f), new Vector3(2f, 0f), valIsLessThanGoal ? Color.blue : Color.red);
        Debug.DrawLine(new Vector3(-1f, 1f), new Vector3(2f, 1f), newValIsLessThanGoal ? Color.blue : Color.red);
        //Debug.Log("PlayerMovement moveTowardsVal : " + moveTowardsVal);
    }

    //Currently going unused might use it later though for custom collision effects
    private void handleCollision()
    {
        foreach (var item in Physics2D.CircleCastAll(transform.position, plrCollider.radius, Vector2.zero))
        {
            if (item.transform != transform)
            {
                Vector3 itemPointVec3 = (Vector3)item.point;

                Vector3 closestPoint = item.collider.bounds.ClosestPoint((Vector3)item.point);

                Vector3 moveToPoint = closestPoint - itemPointVec3;

                transform.position = transform.position - new Vector3(0, moveToPoint.y);

                debugPoint = item.point;

                //transform.position = previousPosition;
            }
        }
        Debug.DrawLine(debugPoint, debugPoint + new Vector3(1f,0f), Color.red);
        Debug.DrawLine(transform.position, transform.position + new Vector3(1f, 0f), Color.red);
    }
}
