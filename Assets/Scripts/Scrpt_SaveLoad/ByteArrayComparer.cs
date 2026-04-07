using System.Collections.Generic;
using UnityEngine;

class ByteArrayComparer : IEqualityComparer<byte[]> {
    public bool Equals(byte[] x, byte[] y) {
        if (x.Length != y.Length)
            return false;
        else {
            for (int i = 0; i < x.Length; i++) {
                if (x[i] != y[i])
                    return false;
            }
            return true;
        }
    }

    public int GetHashCode(byte[] obj) {
        int returnByte;

        if (obj.Length > 0) {
            returnByte = obj[0];
        }
        else {
            returnByte = 0;
        }

        for (int i = 1; i < obj.Length; i++) {
            returnByte ^= obj[i];
        }

        return returnByte;
    }
}
