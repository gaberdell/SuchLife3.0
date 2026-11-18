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
        userNameUI.onSubmit.AddListener(SaveUsername);
    }

    private void OnDisable() {
        userNameUI.onSubmit.RemoveListener(SaveUsername);
    }
}
