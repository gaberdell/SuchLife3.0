using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Overlays;


public class DeleteServerCheckUI : MonoBehaviour
{
    static DeleteServerCheckUI instance;

    [SerializeField]
    GameObject UIWithDeleteObjects;

    [SerializeField]
    TextMeshProUGUI deleteMessageText;

    [SerializeField]
    TextMeshProUGUI deleteButtonText;

    [SerializeField]
    Button deleteButton;

    [SerializeField]
    Button goBackButton;

    static bool yesDelete = false;

    static ServerInfo? deleteSaveInfo = null;
    static ServerSlotUI deleteSaveUI = null;

    static string originalText = "Are you sure you want to <color=red>delete </color>server connection \"{0}\" your player uuid will be deleted will have have to start over on the server!!";

    static string areYouSureText = "Are you <b>ABSOLUTLEY</b> sure you want to <color=red>delete </color>server connection \"{0}\" your player uuid will be deleted will have have to <b>START OVER</b> on the server!!";

    static string originalDeleteButtonText = "DELETE";

    static string areYouSureDeleteButtonText = "YES DELETE";

    private void OnEnable() {
        deleteButton.onClick.AddListener(goAndDelete);
        goBackButton.onClick.AddListener(cancelDelete);
    }

    private void OnDisable() {
        deleteButton.onClick.RemoveAllListeners();
        goBackButton.onClick.RemoveAllListeners();
    }


    void Start()
    {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(instance);
        }
    }

    void goAndDelete() {
        if (yesDelete) {
            NewServerUI.DeleteSlot(deleteSaveUI);
            DataService.DeleteServerSaveData((ServerInfo)deleteSaveInfo);
            Destroy(deleteSaveUI);
            instance.UIWithDeleteObjects.SetActive(false);
        }
        else {
            yesDelete = true;

            instance.deleteMessageText.text = string.Format(areYouSureText, ((ServerInfo)deleteSaveInfo).name);
            instance.deleteButtonText.text = areYouSureDeleteButtonText;
        }
    }

    void cancelDelete() {
        instance.UIWithDeleteObjects.SetActive(false);
        deleteSaveInfo = null;
        deleteSaveUI = null;
    }

    public static void DeleteSave(ServerInfo saveInfo, ServerSlotUI saveUI) {
        deleteSaveInfo = saveInfo;
        deleteSaveUI = saveUI;

        yesDelete = false;
        instance.deleteMessageText.text = string.Format(originalText, saveInfo.name);
        instance.deleteButtonText.text = originalDeleteButtonText;

        instance.UIWithDeleteObjects.SetActive(true);
    }
}
