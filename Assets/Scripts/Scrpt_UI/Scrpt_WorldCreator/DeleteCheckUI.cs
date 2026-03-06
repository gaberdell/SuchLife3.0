using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor.Overlays;


public class DeleteCheckUI : MonoBehaviour
{
    static DeleteCheckUI instance;

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

    static SaveInfo? deleteSaveInfo = null;
    static SaveSlotUI deleteSaveUI = null;

    static string originalText = "Are you sure you want to <color=red>delete </color>world \"{0}\" all data will be lost!!";

    static string areYouSureText = "Are you <b>ABSOLUTLEY</b> sure you want to <color=red>delete </color>world \"{0}\" <b>ALL DATA</b> will be lost!!";

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
            NewWorldUI.DeleteSlot(deleteSaveUI);
            DataService.DeleteSaveData((SaveInfo)deleteSaveInfo);
            Destroy(deleteSaveUI);
            instance.UIWithDeleteObjects.SetActive(false);
        }
        else {
            yesDelete = true;

            instance.deleteMessageText.text = string.Format(areYouSureText, ((SaveInfo)deleteSaveInfo).name);
            instance.deleteButtonText.text = areYouSureDeleteButtonText;
        }
    }

    void cancelDelete() {
        instance.UIWithDeleteObjects.SetActive(false);
        deleteSaveInfo = null;
        deleteSaveUI = null;
    }

    public static void DeleteSave(SaveInfo saveInfo, SaveSlotUI saveUI) {
        deleteSaveInfo = saveInfo;
        deleteSaveUI = saveUI;

        yesDelete = false;
        instance.deleteMessageText.text = string.Format(originalText, saveInfo.name);
        instance.deleteButtonText.text = originalDeleteButtonText;

        instance.UIWithDeleteObjects.SetActive(true);
    }
}
