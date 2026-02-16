using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
                Inherited = false, AllowMultiple = false)]
public class SaveableComponent : Attribute {
    public string SaveClassName { get; private set; }

    public SaveableComponent(string name) {
        SaveClassName = name;
    }
}
