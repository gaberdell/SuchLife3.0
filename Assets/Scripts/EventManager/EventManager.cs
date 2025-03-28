using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void SetPlayerAnimSpeedDelegate(float playerSpeed);
    public static event SetPlayerAnimSpeedDelegate setPlayerAnimSpeed;


    public delegate void ClickAction();
    public static event ClickAction Clicked;

    public delegate void EnterDungeon();
    public static event EnterDungeon PlayerEnterDungeon;

    public delegate void ExitDungeon();
    public static event ExitDungeon PlayerExitDungeon;


    public static EventManager Instance;

    public static void SetPlayerEnterDungeon()
    {
        PlayerEnterDungeon?.Invoke();
    }

    public static void SetPlayerExitDungeon()
    {
        PlayerExitDungeon?.Invoke();
    }

    public static void SetPlayerAnimSpeedTrue(float playerSpeed)
    {
        setPlayerAnimSpeed?.Invoke(playerSpeed);
    }

    void Awake()
    {
        if (Instance != null)
        {
            Instance = gameObject.GetComponent<EventManager>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
