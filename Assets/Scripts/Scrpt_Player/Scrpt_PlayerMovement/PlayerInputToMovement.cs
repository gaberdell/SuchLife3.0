using UnityEngine;

//Note script assumes its attached to the player
public class PlayerInputToMovement : AbstractPlayerMovementGetter {
    InputHandler inputHandler;

    bool safeToGrabCameraInput = false;

    private void Start() {
        inputHandler = InputHandler.Instance;
    }

    private void OnEnable() {
        EventManager.LocalGameObjectPlayerAddedToScene += safeToInitialized;
    }

    private void OnDisable() {
        EventManager.LocalGameObjectPlayerAddedToScene -= safeToInitialized;
    }


    private void safeToInitialized(GameObject player) {
        safeToGrabCameraInput = true;
    }

    void Update()
    {
        MovementInput = inputHandler.MoveInput;


        if (inputHandler.IsMouseEnabled == true && safeToGrabCameraInput) {
            Vector2 mousePos = inputHandler.GetMousePos();
            //Throws an error before its initialized maybe fix 
            MousePositionUnitVector = Camera.main.ScreenToWorldPoint(mousePos);

            MousePositionUnitVector -= (Vector2)transform.position;
            MousePositionUnitVector.Normalize();
        }
        else {
            //TODO : If the player has a controller change it so it follows there left thumbstick

            MousePositionUnitVector = Vector2.up;
        }   
    }
}
