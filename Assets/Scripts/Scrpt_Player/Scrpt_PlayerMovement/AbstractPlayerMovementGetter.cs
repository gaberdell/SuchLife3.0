using UnityEngine;

public abstract class AbstractPlayerMovementGetter : MonoBehaviour
{
    public Vector2 MovementInput { get; protected set; }
    //A unit vector that returns the direction of the mouse position relative to the player
    public Vector2 MousePositionUnitVector { get; protected set; }

}
