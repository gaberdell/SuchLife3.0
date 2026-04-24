using System.Linq;
using UnityEngine;

public class PlayerNetworkDataToMovement : AbstractPlayerMovementGetter {
    // TODO : Figure out what protocal for us is needed get player movement and apply it
    void Start()
    {
        MovementInput = Vector2.zero;
        MousePositionUnitVector = Vector2.up;
    }

    public void SetFromNetworkBytes(byte[] bytes) {
        float xValue = (float)ConvertToByteArray.ConvertBytesToValue(typeof(float), bytes, out int bytesUsed);
        float yValue = (float)ConvertToByteArray.ConvertBytesToValue(typeof(float), bytes.Skip(bytesUsed).ToArray(), out int _);

        MovementInput = new Vector2(xValue, yValue);
        Debug.Log("Movement input being set to : " + MovementInput.ToString());
    }
}
