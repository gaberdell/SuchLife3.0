using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PacketWrapper : IDisposable
{
    private List<byte> packetBytes;
    private int readIndex;
    private bool disposed;

    public PacketWrapper() {
        packetBytes = new List<byte>();
        readIndex = 0;
        disposed = false;
    }

    public PacketWrapper(byte[] bytes) {
        packetBytes = new List<byte>();
        readIndex = 0;
        disposed = false;
        AddBytes(bytes);
    }

    public byte[] GetBytes() {
        return packetBytes.Skip(readIndex).ToArray();
    }

    public void AdvanceReadPos(int amount) {
        readIndex += amount;
    }

    public int BytesLeftToRead() {
        return packetBytes.Count - (readIndex + 1);
    }

    public void AddBytes(byte[] bytes) {
        packetBytes.AddRange(bytes);
    }

    protected virtual void Dispose(bool disposing) {
        if (!disposed) {
            if (disposing) {
                packetBytes.Clear();
                packetBytes = null;
                readIndex = 0;
            }
            disposed = true;
        }
    }

    public byte[] ReadBytes(int length, bool moveCounter = true) {
        if (packetBytes.Count > readIndex) {
            byte[] bytes = packetBytes.GetRange(readIndex, length).ToArray();
            if (moveCounter) {
                readIndex += length;
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
