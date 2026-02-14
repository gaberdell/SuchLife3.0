using System;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

public class ByteConvertor
{


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

            int i = 0;
            for (; i < sizeof(float); i++) {
                returnBytes[i] = xBytes[i];
            }
            for (; i < sizeof(float); i++) {
                returnBytes[i] = yBytes[i];
            }
            for (; i < sizeof(float); i++) {
                returnBytes[i] = zBytes[i];
            }


            return returnBytes;
        }

        Debug.LogError("Bytes serailized with unsupported type : " + value.GetType());
        return null;
    }


    public static object ConvertBytesToValue(Type wantedType, byte[] value) {
        if (wantedType == typeof(int)) {
            return BitConverter.ToInt32(value);
        }
        else if (wantedType == typeof(uint)) {
            return BitConverter.ToUInt32(value);
        }
        else if (wantedType == typeof(float)) {
            return BitConverter.ToSingle(value);
        }
        else if (wantedType == typeof(double)) {
            return BitConverter.ToDouble(value);
        }
        else if (wantedType == typeof(Vector3)) {
            byte[] xBytes = new byte[sizeof(float)];
            byte[] yBytes = new byte[sizeof(float)];
            byte[] zBytes = new byte[sizeof(float)];

            int i = 0;
            for (; i < sizeof(float); i++) {
                xBytes[i] = value[i];
            }

            return new Vector3(BitConverter.ToSingle(xBytes), BitConverter.ToSingle(yBytes),
                               BitConverter.ToSingle(zBytes));
        }


        Debug.LogError("Bytes serailized with unsupported type : " + value.GetType());
        return null;
    }
}
