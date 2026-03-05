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

        deleteButton.onClick.AddListener(deleteSlot);
        upArrow.onClick.AddListener(() => moveSlotAmount(-1));
        downArrow.onClick.AddListener(() => moveSlotAmount(1));
    }

    private void OnDisable() {
        tmpInputField.onDeselect.RemoveAllListeners();
        tmpInputField.onEndEdit.RemoveAllListeners();

        deleteButton.onClick.RemoveAllListeners();
        upArrow.onClick.RemoveAllListeners();
        downArrow.onClick.RemoveAllListeners();

        if (wasModifiedFromOriginalValue) {
            DataService.ResaveBasicSaveInfo(currentSaveInfo);
        }
    }

    private void cloneSave() {
        Debug.LogError("Do cloning in data service");
    }

    private void deleteSlot() {
        NewWorldUI.DeleteSlot(this);
        DataService.DeleteSaveData(currentSaveInfo);
        Destroy(this);
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
