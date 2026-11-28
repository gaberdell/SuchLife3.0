using System;
using UnityEngine;
using TMPro;

[SaveableComponent("Player GUID Info")]
public class PlayerGUIDInfo : MonoBehaviour
{
    [Saveable("GUID As bytes", true)]
    public Guid PlayerGUID;

    [Saveable("Player Username As Bytes")]
    public string PlayerUsername;

    [SerializeField]
    TextMeshPro textToCopy;

    [SerializeField]
    Vector3 textOffset;

#if UNITY_STANDALONE
    TextMeshPro userNameUI;
#endif

    private void Start() {
        if (PlayerUsername == null) {
            PlayerUsername = "";
        }

#if UNITY_STANDALONE
        userNameUI = Instantiate(textToCopy, transform.position + textOffset, Quaternion.identity, transform.parent);
        userNameUI.text = PlayerUsername;
#endif
    }

#if UNITY_STANDALONE
    private void Update() {
        //TODO : Fix this so we don't have to do it like this :skull:
        userNameUI.text = PlayerUsername;
        userNameUI.transform.position = transform.position + textOffset;
    }
#endif
}
