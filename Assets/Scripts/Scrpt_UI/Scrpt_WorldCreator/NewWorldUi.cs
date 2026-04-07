using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewWorldUI : MonoBehaviour
{

    static NewWorldUI instance;

    [SerializeField]
    SaveSlotUI saveSlotUIPrefab;

    [SerializeField]
    Transform parentToAddThemTo;

    [SerializeField] 
    private static uint maxSize;

    [SerializeField]
    Button createNewWorldButton;

    [SerializeField]
    TMP_InputField newWorldInputField;

    static List<SaveSlotUI> saveSlotUIs;

    static SaveSlotUI newSaveSlotUI() {
        SaveSlotUI newSlot = Instantiate(instance.saveSlotUIPrefab, instance.parentToAddThemTo);

        saveSlotUIs.Add(newSlot);
        return newSlot;
    }

    static void recalcPositions() {
        for (int i = 0; i < saveSlotUIs.Count; i++) {
            SaveSlotUI saveSlotUI = saveSlotUIs[i];
            SaveInfo saveInfo = saveSlotUI.GetSaveInfo();
            if (saveInfo.order != (uint)i) {
                saveInfo.order = (uint)i;
                saveSlotUI.UpdateSaveInfo(saveInfo);
            }

            saveSlotUI.transform.SetSiblingIndex(i);
        }
    }

    void makeSaveSlots(List<SaveInfo> sSlots) {
        while (parentToAddThemTo.childCount > 0) {
            DestroyImmediate(parentToAddThemTo.GetChild(0).gameObject);
        }


        foreach (SaveInfo s in sSlots) {
            string worldPath = s.path; //protection level on path and name are restricted
            string worldName = s.name;
            DateTime worldDate = s.lastModified;

            //use the above to populate a clickable button corresponding to a saved world
            SaveSlotUI newSlot = newSaveSlotUI();

            newSlot.UpdateSaveInfo(s);
        }

        maxSize = (uint)saveSlotUIs.Count;
    }


    void Start()
    {
        if (instance == null) {
            instance = this;
        }
        else {
            Destroy(this);
            return;
        }

            saveSlotUIs = new List<SaveSlotUI>();

        makeSaveSlots(DataService.Fetch());
    }

    private void OnEnable()
    {
        createNewWorldButton.onClick.AddListener(CreateWorldButton);
    }
    private void OnDisable()
    {
        createNewWorldButton.onClick.RemoveAllListeners();
    }

    public static void UpdateSlotPosition(SaveSlotUI slot, int change) {
        int indexOfSaveSlot = saveSlotUIs.IndexOf(slot);

        int newVal = indexOfSaveSlot + change;

        //Dont change it in this case cuz it wont change anything
        if (newVal < 0) {
            return;
        }
        else if (newVal >= saveSlotUIs.Count) {
            return;
        }
        SaveSlotUI otherUI = saveSlotUIs[newVal];

        saveSlotUIs[newVal] = slot;
        saveSlotUIs[indexOfSaveSlot] = otherUI;


        //Recalc positions and stuff
        recalcPositions();
    }

    public static void DeleteSlot(SaveSlotUI slot) {
        saveSlotUIs.Remove(slot);
        slot.transform.parent = null;

        recalcPositions();
    }

    public static void AddClone(SaveSlotUI nonClone, SaveInfo cloneSaveData) {
        int index = saveSlotUIs.IndexOf(nonClone);

        SaveSlotUI newCloneSaveSlot = Instantiate(instance.saveSlotUIPrefab, instance.parentToAddThemTo);

        newCloneSaveSlot.UpdateSaveInfo(cloneSaveData);

        saveSlotUIs.Insert(index, newCloneSaveSlot);

        recalcPositions();
    }

    public void CreateWorldButton()
    {
        Debug.Log("Creating world...");

        SaveInfo s = DataService.NewSave(newWorldInputField.text, ++maxSize);

        SaveSlotUI newSlot = newSaveSlotUI();
        newSlot.UpdateSaveInfo(s);
    }

    public void LoadIntoWorld()
    {

    }
}
