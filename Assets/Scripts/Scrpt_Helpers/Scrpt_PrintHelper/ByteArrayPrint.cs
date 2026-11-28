using UnityEngine;

//TODO : Prob move this thing to helper or debug class?
public static class ByteArrayPrint
{
    public static string ByteArrayToString(byte[] byteArray) {
        string returnString = "{";
        for (int i = 0; i < byteArray.Length; i++) {
            returnString += byteArray[i] + (i < byteArray.Length - 1 ? 
                                ((i+1) % 4 == 0 ? ", " : ",") : "");
        }
        returnString += "}";

        return returnString;
    }

    public static void PrintByteArray(byte[] byteArray, string message) {
        Debug.Log(message + ByteArrayToString(byteArray));
    }
}
