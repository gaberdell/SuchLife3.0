using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.iOS;
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

    private void Awake()
    {
        Debug.Log("the hell?");


        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("remove me");
            Destroy(gameObject);
            Instance.moveAction.Enable();
            return;
        }


        Debug.Log("Change my brain up");
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

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.Enable();
            sprintAction.Enable();
            attackAction.Enable();
            placeAction.Enable();
            previousAction.Enable();
            nextAction.Enable();
            interactAction.Enable();
            escapeAction.Enable();

            InputSystem.onDeviceChange += onDeviceChange;
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            Debug.Log("Sus burger 23");
            moveAction.Disable();
            sprintAction.Disable();
            attackAction.Disable();
            placeAction.Disable();
            previousAction.Disable();
            nextAction.Disable();
            interactAction.Disable();
            escapeAction.Disable();

            InputSystem.onDeviceChange -= onDeviceChange;
        }
    }

    private void registerInputActions()
    {
        //Tricky bit of syntax but events need a function which the context is a lambda
        //function that take a InputAction.CallbackContext and returns whatever variable type it stored
        moveAction.performed += context =>
        {
            Debug.Log("HELLO");
            MoveInput = context.ReadValue<Vector2>();
        };
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

}
