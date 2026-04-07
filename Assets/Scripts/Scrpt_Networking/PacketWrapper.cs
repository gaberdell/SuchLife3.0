using System;
using System.Collections.Generic;
using UnityEngine;

public class PacketWrapper : IDisposable
{
    private List<byte> packetBytes;
    private int readPos;
    private bool disposed;

    public PacketWrapper() {
        packetBytes = new List<byte>();
        readPos = 0;
        disposed = false;
    }

    public PacketWrapper(byte[] bytes) {
        packetBytes = new List<byte>();
        readPos = 0;
        disposed = false;
        AddBytes(bytes);
    }

    public byte[] GetBytes() {
        return packetBytes.ToArray();
    }

    public void AddBytes(byte[] bytes) {
        packetBytes.AddRange(bytes);
    }

    protected virtual void Dispose(bool disposing) {
        if (!disposed) {
            if (disposing) {
                packetBytes.Clear();
                packetBytes = null;
                readPos = 0;
            }
            disposed = true;
        }
    }

    public byte[] ReadBytes(int length, bool moveCounter = true) {
        if (packetBytes.Count > readPos) {
            byte[] bytes = packetBytes.GetRange(readPos, length).ToArray();
            if (moveCounter) {
                readPos += length;
            }
            return bytes;
        }

        return null;
    }


    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
