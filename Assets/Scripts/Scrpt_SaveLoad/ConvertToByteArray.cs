using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConvertToByteArray
{
    public static byte[] ConvertValueArrayToBytes(object[] values) {
        List<byte> byteList = new List<byte>();

        byteList.AddRange(ConvertValueToBytes(values.Length));

        for (int i = 0; i < values.Length; i++) {
            byteList.AddRange(ConvertValueToBytes(values[i]));
        }

        return byteList.ToArray();
    }

    public static byte[] ConvertValueToBytes(object value) {

        if (value.GetType() == typeof(int)) {
            return BitConverter.GetBytes((int)value);
        }
        else if (value.GetType() == typeof(uint)) {
            return BitConverter.GetBytes((uint)value);
        }
        else if (value.GetType() == typeof(float)) {
            return BitConverter.GetBytes((float)value);
        }
        else if (value.GetType() == typeof(double)) {
            return BitConverter.GetBytes((double)value);
        }
        else if (value.GetType() == typeof(Vector3)) {
            Vector3 vector3 = (Vector3)value;
            byte[] xBytes = BitConverter.GetBytes(vector3.x);
            byte[] yBytes = BitConverter.GetBytes(vector3.y);
            byte[] zBytes = BitConverter.GetBytes(vector3.z);

            byte[] returnBytes = new byte[3 * sizeof(float)];

            for (int i = 0; i < sizeof(float); i++) {
                returnBytes[i] = xBytes[i];
            }
            for (int i = 0; i < sizeof(float); i++) {
                returnBytes[i + sizeof(float)] = yBytes[i];
            }
            for (int i = 0; i < sizeof(float); i++) {
                returnBytes[i + 2*sizeof(float)] = zBytes[i];
            }


            return returnBytes;
        }
        else if (value.GetType() == typeof(Guid)) {
            //Tested it is in fact 16!
            return ((Guid)value).ToByteArray();
        }

        Debug.LogError("Bytes serailized with unsupported type : " + value.GetType());
        return null;
    }


    public static object[] ConvertBytesToValueArray(Type wantedType, byte[] value, out int bytesUsed) {
        int lengthOfArray = (int) ConvertBytesToValue(typeof(int), value, out int newBytesUsed);
        object[] returnArray = new object[lengthOfArray];
        bytesUsed = newBytesUsed;

        for (int i = 0; i < lengthOfArray; i++) {
            returnArray[i] = ConvertBytesToValue(wantedType, value.Skip(bytesUsed).ToArray(), out newBytesUsed);
            bytesUsed += newBytesUsed;
        }

        return returnArray;
    }

    public static object ConvertBytesToValue(Type wantedType, byte[] value, out int bytesUsed) {
        if (wantedType == typeof(int)) {
            bytesUsed = sizeof(int);
            return BitConverter.ToInt32(value);
        }
        else if (wantedType == typeof(uint)) {
            bytesUsed = sizeof(uint);
            return BitConverter.ToUInt32(value);
        }
        else if (wantedType == typeof(float)) {
            bytesUsed = sizeof(float);
            return BitConverter.ToSingle(value);
        }
        else if (wantedType == typeof(double)) {
            bytesUsed = sizeof(double);
            return BitConverter.ToDouble(value);
        }
        else if (wantedType == typeof(Vector3)) {
            byte[] xBytes = new byte[sizeof(float)];
            byte[] yBytes = new byte[sizeof(float)];
            byte[] zBytes = new byte[sizeof(float)];

            for (int i = 0; i < sizeof(float); i++) {
                xBytes[i] = value[i];
            }
            for (int i = 0; i < sizeof(float); i++) {
                yBytes[i] = value[i + sizeof(float)];
            }
            for (int i = 0; i < sizeof(float); i++) {
                zBytes[i] = value[i + 2 * sizeof(float)];
            }

            bytesUsed = 3*sizeof(float);

            return new Vector3(BitConverter.ToSingle(xBytes), BitConverter.ToSingle(yBytes),
                               BitConverter.ToSingle(zBytes));
        }
        else if (wantedType == typeof(Guid)) {
            //TODO: Big assumtion that the GUID will always be 16 bytes
            bytesUsed = 16;
            return new Guid(value.Take(16).ToArray());
        }


            Debug.LogError("Bytes serailized with unsupported type : " + value.GetType());
        bytesUsed = 0;
        return null;
    }
}
