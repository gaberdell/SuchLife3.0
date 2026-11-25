using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Converts bytes to arrays and back and forth
/// Expect nulls as byte array of {0,0,0,0}
/// Additionally expect empty strings as {0,0,0,0}
/// For our purposes yet we treat empty strings and nulls
/// as identical
/// 
/// Four 0 value is done to make array reading easier
/// As it means a null array easily gets read as a 0 value
/// as arrays contain a prepended int value of four bytes
/// detailing how long the array is
/// 
/// PS array count value is int cuz thats how c# does it
/// even tho ya it should be uint but what u gonna do
/// </summary>
public class ConvertToByteArray
{
    //Null values are represented as four bytes
    public static byte[] GetNewNullValue() {
        return new byte[4] { 0, 0, 0, 0 };
    }

    public static byte[] ConvertValueToBytes(object value) {
        //TODO : Ensure potocols that could be nullable will easily be read if zero
        if (value == null) {
            //Like this since arrays can be max size of uint
            return GetNewNullValue();
        }

        if (value.GetType() == typeof(byte)) {
            return new byte[1] { (byte)value };
        }
        else if (value.GetType() == typeof(int)) {
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
        else if (value.GetType() == typeof(string)) {
            if (((string)value).Length == 0) {
                return GetNewNullValue();
            }

            return Encoding.UTF8.GetBytes((string)value).Concat(new byte[1] {0}).ToArray();
        }
        else if (value.GetType().IsArray) {
            object[] valueAsAnArray = (object[])value;
            IEnumerable<byte> arrayBytesToAdd = new List<byte>();

            arrayBytesToAdd = arrayBytesToAdd.Concat(ConvertValueToBytes(valueAsAnArray.Length));

            for (int i = 0; i < valueAsAnArray.Length; i++) {
                arrayBytesToAdd = arrayBytesToAdd.Concat(ConvertValueToBytes(valueAsAnArray[i]));
            }

            return arrayBytesToAdd.ToArray();
        }

            Debug.LogError("Bytes serailized with unsupported type : " + value.GetType());
        return null;
    }

    //Oof two switch statements back to back :sob: could break this out
    //into seperate abstract classes prob will
    public static int GetSizeOfValue(Type wantedType, byte[] value) {
        if (wantedType == typeof(int)) {
            return sizeof(int);
        }
        else if (wantedType == typeof(uint)) {
            return sizeof(uint);
        }
        else if (wantedType == typeof(float)) {
            return sizeof(float);
        }
        else if (wantedType == typeof(double)) {
            return sizeof(double);
        }
        else if (wantedType == typeof(Vector3)) {
            return 3 * sizeof(float);
        }
        else if (wantedType == typeof(Guid)) {
            return 16;
        }
        else if (wantedType == typeof(string)) {
            int indexOfString = Array.IndexOf(value, (byte)0);
            if (indexOfString == 0) {
                return 4; //Assume it uses four byte null value
            }

            return indexOfString + 1;
        }
        else if (wantedType.IsArray) {
            int countOfArray = (int)ConvertBytesToValue(typeof(int), value, out int totalSize);

            Type elementType = wantedType.GetElementType();

            for (int i = 0; i < countOfArray; i++) {
                int newBytesUsed = GetSizeOfValue(elementType, value.Skip(totalSize).ToArray());
                totalSize += newBytesUsed;
            }
            return totalSize;
        }

            return -1;
    }

    public static object ConvertBytesToValue(Type wantedType, byte[] value, out int bytesUsed) {
        bytesUsed = GetSizeOfValue(wantedType, value);

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

            for (int i = 0; i < sizeof(float); i++) {
                xBytes[i] = value[i];
            }
            for (int i = 0; i < sizeof(float); i++) {
                yBytes[i] = value[i + sizeof(float)];
            }
            for (int i = 0; i < sizeof(float); i++) {
                zBytes[i] = value[i + 2 * sizeof(float)];
            }

            return new Vector3(BitConverter.ToSingle(xBytes), BitConverter.ToSingle(yBytes),
                               BitConverter.ToSingle(zBytes));
        }
        else if (wantedType == typeof(Guid)) {
            return new Guid(value.Take(16).ToArray());
        }
        else if (wantedType == typeof(string)) {
            if (bytesUsed == 4) {
                if (Array.FindAll(value.Take(4).ToArray(), (byteVal) => byteVal == (byte)0).Count() == 4) {
                    return "";
                }
            }

            return Encoding.UTF8.GetString(value,0,Math.Max(bytesUsed-1,0));
        }
        else if (wantedType.IsArray) {
            int countOfArray = (int)ConvertBytesToValue(typeof(int), value, out int totalSize);

            object[] returnArray = new object[countOfArray];

            Type elementType = wantedType.GetElementType();

            for (int i = 0; i < countOfArray; i++) {
                returnArray[i] = ConvertBytesToValue(elementType, 
                                                    value.Skip(totalSize).ToArray(), out int newBytesUsed);
                totalSize += newBytesUsed;
            }

            return returnArray;
        }


        Debug.LogError("Bytes serailized with unsupported type : " + value.GetType());
        return null;
    }
}
