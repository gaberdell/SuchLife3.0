using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

//Code from this fantastic tutorial by SpeedTutor
//https://www.youtube.com/watch?v=lclDl-NGUMg

public class InputHandler : MonoBehaviour
{
    [Header("Input Action Assets")]
    [SerializeField]
    private InputActionAsset playerControls;

    [Header("Action Map Name Reference")]
    [SerializeField]
    private string actionMapName = "Player";

    [Header("Action Name Reference")]
    [SerializeField]
    private string move = "Move";
    [SerializeField]
    private string sprint = "Sprint";
    [SerializeField]
    private string attack = "Attack";
    [SerializeField]
    private string place = "Place";
    [SerializeField]
    private string previous = "Previous";
    [SerializeField]
    private string next = "Next";
    [SerializeField]
    private string interact = "Interact";
    [SerializeField]
    private string escape = "Escape";
    [SerializeField]
    private string inventoryOpen = "Inventory";
    [SerializeField]
    private string crafting = "Crafting";
    [SerializeField]
    private string inventoryHotKey = "InventoryHotKey";

    [Header("Devices")]
    [SerializeField]
    private string mouse = "Mouse";

    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction attackAction;
    private InputAction placeAction;
    private InputAction previousAction;
    private InputAction nextAction;
    private InputAction interactAction;
    private InputAction escapeAction;
    private InputAction inventoryOpenAction;
    private InputAction craftingAction;

    private InputAction[] inventoryAction = new InputAction[10];

    public Vector2 MoveInput { get; private set; }
    public float SprintValue { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool IsPlacing { get; private set; }
    public bool PreviousTriggered { get; private set; }
    public bool NextTriggered { get; private set; }
    public bool InteractTriggered { get; private set; }
    public bool EscapeTriggered { get; private set; }
    public bool IsMouseEnabled { get; private set; }
    public bool QuickSaveTriggered { get; private set; }
    public bool InventoryOpenTriggered { get; private set; }
    public bool CraftingTriggered { get; private set; }
    public bool[] IsInventoryKey { get; private set; }
    public static InputHandler Instance { get; private set; }

    //handling for context-sensitive use; checked against in scripts that perform use-actions with the attack key (or other)
    public enum SelectedContext {None = 0, Tool = 1, Block = 2, Consumable = 3, Weapon = 4}
    static public SelectedContext currSelectedContext { get; private set; }

    private InputActionMap curInputActionMap;

    private void Awake()
    {
        IsInventoryKey = new bool[inventoryAction.Length];

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        curInputActionMap = playerControls.FindActionMap(actionMapName);
        setUpAllActions();
    }

    private InputAction GetInputAction(string name) {
        return curInputActionMap.FindAction(name);
    }

    private void setUpAllActions()
    {
        moveAction = GetInputAction(move);
        sprintAction = GetInputAction(sprint);
        attackAction = GetInputAction(attack);
        placeAction = GetInputAction(place);
        previousAction = GetInputAction(previous);
        nextAction = GetInputAction(next);
        interactAction = GetInputAction(interact);
        escapeAction = GetInputAction(escape);
        inventoryOpenAction = GetInputAction(inventoryOpen);
        craftingAction = GetInputAction(crafting);

        for (int i = 0; i < inventoryAction.Length; i++) {
            inventoryAction[i] = GetInputAction(inventoryHotKey + i.ToString());
        }

        registerInputActions();

        registerAllInitialDevices();
    }

    private void enableAllActions()
    {
        moveAction.Enable();
        sprintAction.Enable();
        attackAction.Enable();
        placeAction.Enable();
        previousAction.Enable();
        nextAction.Enable();
        interactAction.Enable();
        escapeAction.Enable();
        inventoryOpenAction.Enable();
        craftingAction.Enable();

        for (int i = 0; i < inventoryAction.Length; i++) {
            inventoryAction[i].Enable();
        }
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            enableAllActions();

            InputSystem.onDeviceChange += onDeviceChange;
            SceneManager.sceneUnloaded += regenerateActions;
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.Disable();
            sprintAction.Disable();
            attackAction.Disable();
            placeAction.Disable();
            previousAction.Disable();
            nextAction.Disable();
            interactAction.Disable();
            escapeAction.Disable();
            inventoryOpenAction.Disable();
            craftingAction.Disable();

            for (int i = 0; i < inventoryAction.Length; i++) {
                inventoryAction[i].Disable();
            }

            InputSystem.onDeviceChange -= onDeviceChange;
            SceneManager.sceneUnloaded -= regenerateActions;
        }
    }

    private void registerInputActions()
    {
        //Tricky bit of syntax but events need a function which the context is a lambda
        //function that take a InputAction.CallbackContext and returns whatever variable type it stored
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        sprintAction.performed += context => SprintValue = context.ReadValue<float>();
        sprintAction.canceled += context => SprintValue = 0f;

        attackAction.performed += context => IsAttacking = true;
        attackAction.canceled += context => IsAttacking = false;

        placeAction.performed += context => IsPlacing = true;
        placeAction.canceled += context => IsPlacing = false;

        previousAction.performed += context => PreviousTriggered = true;
        previousAction.canceled += context => PreviousTriggered = false;

        nextAction.performed += context => NextTriggered = true;
        nextAction.canceled += context => NextTriggered = false;

        interactAction.performed += context => InteractTriggered = true;
        interactAction.canceled += context => InteractTriggered = false;

        escapeAction.performed += context => EscapeTriggered = true;
        escapeAction.canceled += context => EscapeTriggered = false;

        inventoryOpenAction.performed += context => InventoryOpenTriggered = true;
        inventoryOpenAction.canceled += context => InventoryOpenTriggered = false;

        craftingAction.performed += context => CraftingTriggered = true;
        craftingAction.canceled += context => CraftingTriggered = false;

        for (int i = 0; i < inventoryAction.Length; i++) {
            int curIndex = i;
            inventoryAction[curIndex].performed += context => IsInventoryKey[curIndex] = true;
            inventoryAction[curIndex].canceled += context => IsInventoryKey[curIndex] = false;
        }

    }

    private void LateUpdate() {
        InventoryOpenTriggered = false;
        CraftingTriggered = false;
    }

    private void registerAllInitialDevices()
    {
        IsMouseEnabled = false;

        foreach (InputDevice device in InputSystem.devices)
        {
            onDeviceChange(device, InputDeviceChange.Added);
        }
    }

    private void onDeviceChange(InputDevice device, InputDeviceChange change)
    {
        if (device.name == mouse)
            IsMouseEnabled = !(change == InputDeviceChange.Disconnected || change == InputDeviceChange.Disabled);
    }

    public Vector2 GetMousePos()
    {
        return Mouse.current.position.ReadValue();
    }

    //Re register things if a PlayerInput was deleted
    private void regenerateActions(Scene current)
    {
        if (!playerControls.FindActionMap(actionMapName).enabled)
        {
            playerControls.FindActionMap(actionMapName).Enable();
            setUpAllActions();
            enableAllActions();


            Debug.Log("OnSceneUnloaded: " + current);
        }
    }

    static public void setSelectedContext(int itemFlag)
    {
        currSelectedContext = (SelectedContext) itemFlag; //casting int to enum equivalence
    }

}
