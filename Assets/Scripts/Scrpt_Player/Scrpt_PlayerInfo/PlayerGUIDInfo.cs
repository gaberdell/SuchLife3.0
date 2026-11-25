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

    [SerializeField]
    TextMeshPro textToCopy;

    [SerializeField]
    Vector3 textOffset;

#if UNITY_STANDALONE
    TextMeshPro userNameUI;
#endif

    private void Start() {
        if (username == null) {
            username = "";
        }

#if UNITY_STANDALONE
        userNameUI = Instantiate(textToCopy, transform.position + textOffset, Quaternion.identity, transform.parent);
        userNameUI.text = username;
#endif
    }

#if UNITY_STANDALONE
    private void Update() {
        //TODO : Fix this so we don't have to do it like this :skull:
        userNameUI.text = username;
        userNameUI.transform.position = transform.position + textOffset;
    }
#endif
}
