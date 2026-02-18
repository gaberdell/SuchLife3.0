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

        upArrow.onClick.AddListener(() => moveSlotAmount(1));
        downArrow.onClick.AddListener(() => moveSlotAmount(-1));
    }

    private void OnDisable() {
        tmpInputField.onDeselect.RemoveAllListeners();
        tmpInputField.onEndEdit.RemoveAllListeners();

        upArrow.onClick.RemoveAllListeners();
        downArrow.onClick.RemoveAllListeners();

        if (wasModifiedFromOriginalValue) {
            DataService.ResaveBasicSaveInfo(currentSaveInfo);
        }
    }

    private void cloneSave() {
        Debug.LogError("Do cloning in data service");
    }

    private void moveSlotAmount(int amount) {

        wasModifiedFromOriginalValue = true;

        currentSaveInfo.order = (uint)((int) Math.Clamp(currentSaveInfo.order+amount,0,int.MaxValue));

        EventManager.SetUpdateSlotPosition(currentSaveInfo.path, currentSaveInfo.order);
    }


    private void updateName(string newName) {
        currentSaveInfo.name = newName;
        topText.text = currentSaveInfo.name;

        wasModifiedFromOriginalValue = true;
    }

    public void UpdateSaveInfo(SaveInfo newSaveInfo)
    {
        currentSaveInfo = newSaveInfo;
        topText.text = newSaveInfo.name;
    }
}
