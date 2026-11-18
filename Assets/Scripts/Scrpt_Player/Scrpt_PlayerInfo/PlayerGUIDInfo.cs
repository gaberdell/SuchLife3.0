using System;
using UnityEngine;
using TMPro;

[SaveableComponent("Player GUID Info")]
public class PlayerGUIDInfo : MonoBehaviour
{
    [Saveable("GUID As bytes")]
    public Guid guid;

    [Saveable("Player Username As Bytes")]
    public string username;

#if UNITY_STANDALONE
    TextMeshPro userNameUI;

    [SerializeField]
    TextMeshPro textToCopy;

    [SerializeField]
    Vector3 textOffset;

    private void Start() {
        userNameUI = Instantiate(textToCopy, transform.position+textOffset, Quaternion.identity);
        userNameUI.text = username;
    }
    private void Update() {
        //TODO : Fix this so we don't have to do it like this :skull:
        userNameUI.text = username;
        userNameUI.transform.position = transform.position + textOffset;
    }
#endif
}
