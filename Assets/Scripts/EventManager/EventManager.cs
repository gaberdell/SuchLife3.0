using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void SetPlayerAnimSpeedDelegate(float playerSpeed);
    public static event SetPlayerAnimSpeedDelegate setPlayerAnimSpeed;

    public delegate void NoParamDelegate();
    public static event NoParamDelegate Clicked;

    public delegate void EnterDungeon(int offsetX, int offsetY, int dWidth, int dHeight);
    public static event EnterDungeon PlayerEnterDungeon;

    public static event NoParamDelegate PlayerExitDungeon;

    public static event NoParamDelegate CraftingTableUpdate;

    public delegate void UpdateSlotPositionDelegate(string pathName, uint position);
    public static event UpdateSlotPositionDelegate UpdateSlotPosition;

    public delegate void GameObjectSingleDelegate(GameObject gameObject);
    public static event GameObjectSingleDelegate PrefabAddedToScene;

    public static event GameObjectSingleDelegate PrefabRemovedFromScene;

    public static event GameObjectSingleDelegate LocalGameObjectPlayerAddedToScene;
    public static event GameObjectSingleDelegate LocalGameObjectPlayerLeftScene;
    public static event GameObjectSingleDelegate OnlinePlayerJoined;
    public static event GameObjectSingleDelegate OnlinePlayerLeft;

    public static event NoParamDelegate PlayerExitedGameSoSafeToLeave;

    public static EventManager Instance;

    public static void SetPlayerEnterDungeon(int offsetX, int offsetY, int dWidth, int dHeight)
    {
        PlayerEnterDungeon?.Invoke(offsetX, offsetY, dWidth, dHeight);
    }

    public static void SetPlayerExitDungeon()
    {
        PlayerExitDungeon?.Invoke();
    }

    public static void SetPlayerAnimSpeedTrue(float playerSpeed)
    {
        setPlayerAnimSpeed?.Invoke(playerSpeed);
    }

    public static void CheckForCraftTableUpdate()
    {
        CraftingTableUpdate?.Invoke();
    }

    public static void SetUpdateSlotPosition(string pathName, uint position)
    {
        UpdateSlotPosition?.Invoke(pathName, position);
    }

    public static void SetPrefabRemovedFromScene(GameObject gameObject) {
        PrefabRemovedFromScene?.Invoke(gameObject);
    }
    public static void SetPrefabAddedToScene(GameObject gameObject) {
        PrefabAddedToScene?.Invoke(gameObject);
    }

    public static void SetLocalGameObjectPlayerAddedToScene(GameObject player) {
        LocalGameObjectPlayerAddedToScene?.Invoke(player);
    }

    public static void SetLocalGameObjectPlayerLeftScene(GameObject player) {
        LocalGameObjectPlayerLeftScene?.Invoke(player);
    }

    public static void SetOnlinePlayerJoined(GameObject player) {
        OnlinePlayerJoined?.Invoke(player);
    }

    public static void SetOnlinePlayerLeft(GameObject player) {
        OnlinePlayerLeft?.Invoke(player);
    }

    public static void SetPlayerExitedGameSoSafeToLeave() {
        PlayerExitedGameSoSafeToLeave?.Invoke();
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
