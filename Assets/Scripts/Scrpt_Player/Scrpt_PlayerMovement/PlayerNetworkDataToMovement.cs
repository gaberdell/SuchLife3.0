using System.Linq;
using UnityEngine;

public class PlayerNetworkDataToMovement : AbstractPlayerMovementGetter {
    void Start()
    {
        MovementInput = Vector2.zero;
        PlayerIntendedRotation = 0f;
    }

    public void SetFromNetworkBytes(byte[] bytes) {
        float xValue = (float)ConvertToByteArray.ConvertBytesToValue(typeof(float), bytes, out int bytesUsed);
        float yValue = (float)ConvertToByteArray.ConvertBytesToValue(typeof(float), bytes.Skip(bytesUsed).ToArray(), out int ybytesUsed);
        float rot = (float)ConvertToByteArray.ConvertBytesToValue(typeof(float), bytes.Skip(bytesUsed + ybytesUsed).ToArray(), out _);
        MovementInput = new Vector2(xValue, yValue);
        PlayerIntendedRotation = rot;
        Debug.Log(PlayerIntendedRotation);
    }
    public void SetPlayerControlValues(float xValue, float yValue, float rot) {
        MovementInput = new Vector2(xValue, yValue);
        PlayerIntendedRotation = rot;
    }

}
