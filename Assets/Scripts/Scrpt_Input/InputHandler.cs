using UnityEngine;
using UnityEngine.InputSystem;

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

    public Vector2 MoveInput { get; private set; }
    public float SprintValue { get; private set; }
    public bool IsAttacking { get; private set; }
    public bool IsPlacing { get; private set; }
    public bool PreviousTriggered { get; private set; }
    public bool NextTriggered { get; private set; }
    public bool InteractTriggered { get; private set; }

    public bool IsMouseEnabled { get; private set; }

    public static InputHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        sprintAction = playerControls.FindActionMap(actionMapName).FindAction(sprint);
        attackAction = playerControls.FindActionMap(actionMapName).FindAction(attack);
        placeAction = playerControls.FindActionMap(actionMapName).FindAction(place);
        previousAction = playerControls.FindActionMap(actionMapName).FindAction(previous);
        nextAction = playerControls.FindActionMap(actionMapName).FindAction(next);
        interactAction = playerControls.FindActionMap(actionMapName).FindAction(interact);

        registerInputActions();

        checkIfEnabledDevices();
    }

    private void checkIfEnabledDevices()
    {
        IsMouseEnabled = false;

        foreach (InputDevice device in InputSystem.devices)
        {
            Debug.Log(device.name);
            if (device.enabled && device.name == mouse)
            {
                
                IsMouseEnabled = true;
            }
            break;
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
    }

    private void OnEnable()
    {
        moveAction.Enable();
        sprintAction.Enable();
        attackAction.Enable(); 
        placeAction.Enable();
        previousAction.Enable();
        nextAction.Enable();
        interactAction.Enable();

        InputSystem.onDeviceChange += onDeviceChange;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        sprintAction.Disable();
        attackAction.Disable();
        placeAction.Disable();
        previousAction.Disable();
        nextAction.Disable();
        interactAction.Disable();

        InputSystem.onDeviceChange -= onDeviceChange;
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

}
