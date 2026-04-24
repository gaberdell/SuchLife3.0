using System;
using UnityEngine;

[SaveableComponent("Player GUID Info")]
public class PlayerGUIDInfo : MonoBehaviour
{
    [Saveable("GUID As bytes")]
    public Guid guid;
}
