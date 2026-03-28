using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.UI;

//TODO : Refactor this so isnt a cut and paste from NewWorldUI
public class NewServerUI : MonoBehaviour
{

    static NewServerUI instance;

    [SerializeField]
    ServerSlotUI serverSlotUIPrefab;

    [SerializeField]
    Transform parentToAddThemTo;

    [SerializeField] 
    private static uint maxSize;

    [SerializeField]
    Button createNewWorldButton;

    [SerializeField]
    TMP_InputField newWorldInputField;

    static List<ServerSlotUI> ServerSlotUIs;

    static ServerSlotUI newServerSlotUI() {
        ServerSlotUI newSlot = Instantiate(instance.serverSlotUIPrefab, instance.parentToAddThemTo);

        ServerSlotUIs.Add(newSlot);
        return newSlot;
    }

    static void recalcPositions() {
        for (int i = 0; i < ServerSlotUIs.Count; i++) {
            ServerSlotUI ServerSlotUI = ServerSlotUIs[i];
            ServerInfo saveInfo = ServerSlotUI.GetSaveInfo();
            if (saveInfo.order != (uint)i) {
                saveInfo.order = (uint)i;
                ServerSlotUI.UpdateSaveInfo(saveInfo);
            }

            ServerSlotUI.transform.SetSiblingIndex(i);
        }
    }

    void makeSaveSlots(List<ServerInfo> sSlots) {
        while (parentToAddThemTo.childCount > 0) {
            DestroyImmediate(parentToAddThemTo.GetChild(0).gameObject);
        }


        foreach (ServerInfo s in sSlots) {
            string worldPath = s.path; //protection level on path and name are restricted
            string worldName = s.name;

            //use the above to populate a clickable button corresponding to a saved world
            ServerSlotUI newSlot = newServerSlotUI();

            newSlot.UpdateSaveInfo(s);
        }

        maxSize = (uint)ServerSlotUIs.Count;
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

            ServerSlotUIs = new List<ServerSlotUI>();

        makeSaveSlots(DataService.FetchServers());
    }

    private void OnEnable()
    {
        createNewWorldButton.onClick.AddListener(CreateWorldButton);
    }
    private void OnDisable()
    {
        createNewWorldButton.onClick.RemoveAllListeners();
    }

    public static void UpdateSlotPosition(ServerSlotUI slot, int change) {
        int indexOfSaveSlot = ServerSlotUIs.IndexOf(slot);

        int newVal = indexOfSaveSlot + change;

        //Dont change it in this case cuz it wont change anything
        if (newVal < 0) {
            return;
        }
        else if (newVal >= ServerSlotUIs.Count) {
            return;
        }
        ServerSlotUI otherUI = ServerSlotUIs[newVal];

        ServerSlotUIs[newVal] = slot;
        ServerSlotUIs[indexOfSaveSlot] = otherUI;


        //Recalc positions and stuff
        recalcPositions();
    }

    public static void DeleteSlot(ServerSlotUI slot) {
        ServerSlotUIs.Remove(slot);
        slot.transform.parent = null;

        recalcPositions();
    }

    public static void AddClone(ServerSlotUI nonClone, ServerInfo cloneSaveData) {
        int index = ServerSlotUIs.IndexOf(nonClone);

        ServerSlotUI newCloneSaveSlot = Instantiate(instance.serverSlotUIPrefab, instance.parentToAddThemTo);

        newCloneSaveSlot.UpdateSaveInfo(cloneSaveData);

        ServerSlotUIs.Insert(index, newCloneSaveSlot);

        recalcPositions();
    }

    public void CreateWorldButton()
    {
        Debug.Log("Creating world...");

        ServerInfo s = DataService.NewServerSave(newWorldInputField.text, ++maxSize);

        ServerSlotUI newSlot = newServerSlotUI();
        newSlot.UpdateSaveInfo(s);
    }
}
