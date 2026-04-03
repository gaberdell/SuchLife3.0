using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    private SaveInfo currentSaveInfo;

    [SerializeField]
    private TextMeshProUGUI topText;

    [SerializeField]
    private TMP_InputField tmpInputField;

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

        playButton.onClick.AddListener(playGame);
        cloneButton.onClick.AddListener(cloneSave);
        deleteButton.onClick.AddListener(deleteSlot);
        upArrow.onClick.AddListener(() => moveSlotAmount(-1));
        downArrow.onClick.AddListener(() => moveSlotAmount(1));
    }

    private void OnDisable() {
        tmpInputField.onDeselect.RemoveAllListeners();
        tmpInputField.onEndEdit.RemoveAllListeners();

        playButton.onClick.RemoveAllListeners();
        cloneButton.onClick.RemoveAllListeners();
        deleteButton.onClick.RemoveAllListeners();
        upArrow.onClick.RemoveAllListeners();
        downArrow.onClick.RemoveAllListeners();

        if (wasModifiedFromOriginalValue) {
            DataService.ResaveBasicSaveInfo(currentSaveInfo);
        }
    }

    private void playGame() {
        Debug.Log(currentSaveInfo.path);
        DataService.Load(currentSaveInfo);
    }

    private void cloneSave() {
        SaveInfo cloneSaveInfo = DataService.CloneSaveData(currentSaveInfo);
        NewWorldUI.AddClone(this, cloneSaveInfo);
    }

    private void deleteSlot() {
        DeleteCheckUI.DeleteSave(currentSaveInfo, this);
    }

    private void moveSlotAmount(int amount) {
        NewWorldUI.UpdateSlotPosition(this, amount);
    }


    private void updateName(string newName) {
        currentSaveInfo.name = newName;
        tmpInputField.text = currentSaveInfo.name;

        wasModifiedFromOriginalValue = true;
    }

    public void UpdateSaveInfo(SaveInfo newSaveInfo)
    {
        currentSaveInfo = newSaveInfo;
        tmpInputField.text = newSaveInfo.name;
    }

    public SaveInfo GetSaveInfo() {
        return currentSaveInfo;
    }
}
