using UnityEngine;

public class PlayerArmRotation : MonoBehaviour
{
    [Header("Parent")]
    [SerializeField]
    Transform parent;

    [Header("Config")]
    [SerializeField]
    float angleSmoothing = 0.1f;

    [SerializeField]
    float catchUpAngle = 30.0f;

    float currentArmRotation = 0f;
    void Start()
    {
        parent = parent == null ? transform.parent : parent;
    }

    void Update()
    {

        float armRotDif = MathHelper.RotationTo180Scale(currentArmRotation - parent.rotation.eulerAngles.z);
        float absArmRotDif = Mathf.Abs(armRotDif);
        float rotSign = Mathf.Sign(armRotDif);

        if (absArmRotDif > catchUpAngle+0.1f)
        {
            currentArmRotation = parent.rotation.eulerAngles.z + catchUpAngle * rotSign;
        }
        else
        {
            currentArmRotation = MathHelper.DampAngle(currentArmRotation, parent.rotation.eulerAngles.z, angleSmoothing, Time.deltaTime);
        }

        transform.rotation = Quaternion.Euler(0f,0f, currentArmRotation);
    }
}
