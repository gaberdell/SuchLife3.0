using UnityEngine;

[SaveableId(new byte[1] { 1 })]
public class PrefabSaveInfo : MonoBehaviour
{
    //Id of actual prefab
    [Saveable]
    public byte[] PrefabId;
    public uint EntityId;
}
