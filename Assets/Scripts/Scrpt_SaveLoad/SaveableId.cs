using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct,
                Inherited = false, AllowMultiple = false)]
public class SaveableId : Attribute
{
    private byte[] id;

    //Only used for mods and stuff our stuff is assumed blank
    private string customNameFrom;

    public SaveableId(params byte[] ourId)
    {
        id = ourId;
    }

    public virtual byte[] Id { get { return id; } }

    public virtual string CustomNameFrom { 
        get { return customNameFrom; }
        set { customNameFrom = value; }
    }
}
