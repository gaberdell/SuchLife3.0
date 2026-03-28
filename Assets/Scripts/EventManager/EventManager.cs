using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void SetPlayerAnimSpeedDelegate(float playerSpeed);
    public static event SetPlayerAnimSpeedDelegate setPlayerAnimSpeed;


    public delegate void ClickAction();
    public static event ClickAction Clicked;

    public delegate void EnterDungeon(int offsetX, int offsetY, int dWidth, int dHeight);
    public static event EnterDungeon PlayerEnterDungeon;

    public delegate void ExitDungeon();
    public static event ExitDungeon PlayerExitDungeon;

    public delegate void CraftingTableUpdated();
    public static event CraftingTableUpdated CraftingTableUpdate;

    public delegate void UpdateSlotPositionDelegate(string pathName, uint position);
    public static event UpdateSlotPositionDelegate UpdateSlotPosition;

    public delegate void GameObjectSingleDelegate(GameObject gameObject);
    public static event GameObjectSingleDelegate PrefabAddedToScene;

    public static event GameObjectSingleDelegate PrefabRemovedFromScene;

    public static event GameObjectSingleDelegate LocalGameObjectPlayerAddedToScene;
    public static event GameObjectSingleDelegate LocalGameObjectPlayerLeftScene;
    public static event GameObjectSingleDelegate OnlinePlayerJoined;
    public static event GameObjectSingleDelegate OnlinePlayerLeft;

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
