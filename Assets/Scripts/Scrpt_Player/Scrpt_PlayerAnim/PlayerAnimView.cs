using UnityEngine;

public class PlayerAnimView : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField]
    Animator playerAnimator;


    [Header("Config")]

    [SerializeField]
    string blendName = "IdleToWalk";
    [SerializeField]
    string speedOfWalkName = "AnimWalkSpeed";



    private void Start()
    {
        playerAnimator = playerAnimator ? playerAnimator : GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventManager.setPlayerAnimSpeed += setPlayerSpeed;
    }

    private void OnDisable()
    {
        EventManager.setPlayerAnimSpeed -= setPlayerSpeed;
    }

    void setPlayerSpeed(float speed)
    {
        playerAnimator.SetFloat(blendName, Mathf.Clamp01(speed));
        playerAnimator.SetFloat(speedOfWalkName, Mathf.Max(speed, 1f));

    }
}
