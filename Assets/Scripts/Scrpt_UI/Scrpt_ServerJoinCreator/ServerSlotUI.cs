using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//TODO : Refactor this so isnt a cut and paste from NewWorldUI
public class ServerSlotUI : MonoBehaviour
{
    private ServerInfo currentServerInfo;

    [SerializeField]
    private TextMeshProUGUI topText;

    [SerializeField]
    private TMP_InputField tmpInputField;

    [SerializeField]
    private TMP_InputField ipField;

    [SerializeField]
    private Button upArrow;
    [SerializeField]
    private Button downArrow;
    [SerializeField]
    private Button cloneButton;
    [SerializeField]
    private Button deleteButton;
    [SerializeField]
    private Button playButton;

    private bool wasModifiedFromOriginalValue = false;

    private void OnEnable() {
        tmpInputField.onDeselect.AddListener(updateName);
        tmpInputField.onEndEdit.AddListener(updateName);

        ipField.onDeselect.AddListener(updateIp);
        ipField.onEndEdit.AddListener(updateIp);

        playButton.onClick.AddListener(playGame);
        cloneButton.onClick.AddListener(cloneSave);
        deleteButton.onClick.AddListener(deleteSlot);
        upArrow.onClick.AddListener(() => moveSlotAmount(-1));
        downArrow.onClick.AddListener(() => moveSlotAmount(1));
    }

    private void OnDisable() {
        tmpInputField.onDeselect.RemoveAllListeners();
        tmpInputField.onEndEdit.RemoveAllListeners();

        ipField.onDeselect.RemoveAllListeners();
        ipField.onEndEdit.RemoveAllListeners();

        playButton.onClick.RemoveAllListeners();
        cloneButton.onClick.RemoveAllListeners();
        deleteButton.onClick.RemoveAllListeners();
        upArrow.onClick.RemoveAllListeners();
        downArrow.onClick.RemoveAllListeners();

        if (wasModifiedFromOriginalValue) {
            DataService.ResaveBasicServerSaveInfo(currentServerInfo);
        }
    }

    private void playGame() {
        DataService.LoadServer(currentServerInfo.path, false);
    }

    private void cloneSave() {
        ServerInfo cloneSaveInfo = DataService.CloneServerSaveData(currentServerInfo);
        NewServerUI.AddClone(this, cloneSaveInfo);
    }

    private void deleteSlot() {
        DeleteServerCheckUI.DeleteSave(currentServerInfo, this);
    }

    private void moveSlotAmount(int amount) {
        NewServerUI.UpdateSlotPosition(this, amount);
    }


    private void updateName(string newName) {
        currentServerInfo.name = newName;
        tmpInputField.text = currentServerInfo.name;

        wasModifiedFromOriginalValue = true;
    }

    private void updateIp(string newName) {
        currentServerInfo.ip = newName;
        tmpInputField.text = currentServerInfo.name;

        wasModifiedFromOriginalValue = true;
    }

    public void UpdateSaveInfo(ServerInfo newSaveInfo)
    {
        currentServerInfo = newSaveInfo;
        tmpInputField.text = newSaveInfo.name;
    }

    public ServerInfo GetSaveInfo() {
        return currentServerInfo;
    }
}
