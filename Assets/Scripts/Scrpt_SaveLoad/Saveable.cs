using System;
using UnityEngine;

/// <summary>
/// An attribute class that can save
/// Vector3's, ints, and floats
/// Other more specific classes are handled with
/// SaveableId and then are properly dealt with in SaveManager
/// </summary>
[AttributeUsage(AttributeTargets.Field,
                Inherited = true, AllowMultiple = false)]
public class Saveable : Attribute
{
    public string SaveName { get; private set; }

    public Saveable(string name) {
        SaveName = name;
    }

}
