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
    private string attackLeft = "AttackLeft";
    [SerializeField]
    private string attackRight = "AttackRight";
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
    private InputAction attackActionLeft;
    private InputAction attackActionRight;
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
 
    //true for as long as the hand's button is held down
    public bool IsAttackingLeft { get; private set; }
    public bool IsAttackingRight { get; private set; }
 
    //true for exactly one frame when the hand's button goes down (cleared in LateUpdate)
    //use these for one-shot actions like drinking a potion or firing a charged crossbow
    public bool AttackPressedLeft { get; private set; }
    public bool AttackPressedRight { get; private set; }
 
    //Place is no longer used, placing blocks is done with the hand's attack button
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
    public bool IsShiftHeld { get { return Keyboard.current != null && Keyboard.current.shiftKey.isPressed; }}
    
    public enum SelectedContext {None = 0, Tool = 1, Block = 2, Consumable = 3, Weapon = 4}
    static private readonly SelectedContext[] selectedContexts = new SelectedContext[2];
    
    private InputActionMap curInputActionMap;
    
    public bool IsAttacking(Hand hand)
    {
        return hand == Hand.Left ? IsAttackingLeft : IsAttackingRight;
    }
 
    public bool WasAttackPressed(Hand hand)
    {
        return hand == Hand.Left ? AttackPressedLeft : AttackPressedRight;
    }
 
    static public SelectedContext GetSelectedContext(Hand hand)
    {
        return selectedContexts[(int)hand];
    }
 
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
        attackActionLeft = GetInputAction(attackLeft);
        attackActionRight = GetInputAction(attackRight);
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
        attackActionLeft.Enable();
        attackActionRight.Enable();
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
            attackActionLeft.Disable();
            attackActionRight.Disable();
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
 
        attackActionLeft.performed += context => { IsAttackingLeft = true; AttackPressedLeft = true; };
        attackActionLeft.canceled += context => IsAttackingLeft = false;
 
        attackActionRight.performed += context => { IsAttackingRight = true; AttackPressedRight = true; };
        attackActionRight.canceled += context => IsAttackingRight = false;
 
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
        AttackPressedLeft = false;
        AttackPressedRight = false;
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
 
    static public void setSelectedContext(Hand hand, int itemFlag)
    {
        selectedContexts[(int)hand] = (SelectedContext) itemFlag;
    }
 
}