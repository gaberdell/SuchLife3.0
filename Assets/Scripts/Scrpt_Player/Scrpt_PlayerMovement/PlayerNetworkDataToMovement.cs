using UnityEngine;

public class PlayerNetworkDataToMovement : AbstractPlayerMovementGetter {
    // TODO : Figure out what protocal for us is needed get player movement and apply it
    void Start()
    {
        MovementInput = Vector2.zero;
        MousePositionUnitVector = Vector2.up;
    }
}
