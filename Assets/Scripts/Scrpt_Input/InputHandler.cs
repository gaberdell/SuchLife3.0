using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.iOS;
using UnityEngine.SceneManagement;
using UnityEngine.tvOS;

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

    public Vector2 MoveInput { get; private set; }
    public float SprintValue { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool IsPlacing { get; private set; }
    public bool PreviousTriggered { get; private set; }
    public bool NextTriggered { get; private set; }
    public bool InteractTriggered { get; private set; }
    public bool EscapeTriggered { get; private set; }

    public bool IsMouseEnabled { get; private set; }
    public static InputHandler Instance { get; private set; }

    //handling for context-sensitive use; checked against in scripts that perform use-actions with the attack key (or other)
    public enum SelectedContext {None = 0, Tool = 1, Block = 2, Consumable = 3, Weapon = 4}
    static public SelectedContext currSelectedContext { get; private set; }

    private void Awake()
    {


        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            //Instance.moveAction.Enable();
            return;
        }

        setUpAllActions();
    }

    private void setUpAllActions()
    {
        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        sprintAction = playerControls.FindActionMap(actionMapName).FindAction(sprint);
        attackAction = playerControls.FindActionMap(actionMapName).FindAction(attack);
        placeAction = playerControls.FindActionMap(actionMapName).FindAction(place);
        previousAction = playerControls.FindActionMap(actionMapName).FindAction(previous);
        nextAction = playerControls.FindActionMap(actionMapName).FindAction(next);
        interactAction = playerControls.FindActionMap(actionMapName).FindAction(interact);
        escapeAction = playerControls.FindActionMap(actionMapName).FindAction(escape);


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

            InputSystem.onDeviceChange -= onDeviceChange;
            SceneManager.sceneUnloaded -= regenerateActions;
        }
    }


    private void LateUpdate()
    {

        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Is Ui Enabled : " + playerControls.FindActionMap("UI").enabled);
            Debug.Log("Is Player Map Enabled : " + playerControls.FindActionMap(actionMapName).enabled);
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
