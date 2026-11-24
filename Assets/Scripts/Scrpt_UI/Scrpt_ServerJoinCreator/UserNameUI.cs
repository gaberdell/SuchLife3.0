using TMPro;
using UnityEngine;

public class UserNameUI : MonoBehaviour
{
    [SerializeField]
    TMP_InputField userNameUI;

    void SaveUsername(string text) {
        DataService.SaveGlobalPlayerAvatarData(text);
    }


    private void OnEnable() {
        userNameUI.onEndEdit.AddListener(SaveUsername);
        userNameUI.onDeselect.AddListener(SaveUsername);
    }

    private void OnDisable() {
        userNameUI.onEndEdit.RemoveListener(SaveUsername);
        userNameUI.onDeselect.RemoveListener(SaveUsername);
    }

    private void Start() {
        string loadText = DataService.LoadGlobalPlayerAvatarData();

        if (loadText != "guest") {
            userNameUI.text = loadText;
        }
    }
}
