using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class NewWorldUI : MonoBehaviour
{

    [SerializeField]
    SaveSlotUI saveSlotUIPrefab;

    [SerializeField]
    Transform parentToAddThemTo;

    [SerializeField] 
    private static uint maxSize;

    [SerializeField]
    Button createNewWorldButton;

    List<SaveInfo> info;

    void Start()
    {
        info = DataService.Fetch();
        Debug.Log(info); //debugging print statement
        foreach(SaveInfo s in info)
        {
           string worldPath = s.path; //protection level on path and name are restricted
           string worldName = s.name;
           DateTime worldDate = s.lastModified;

            //use the above to populate a clickable button corresponding to a saved world
            SaveSlotUI newSlot = Instantiate(saveSlotUIPrefab, parentToAddThemTo);
            newSlot.UpdateSaveInfo(s);
        }

        maxSize = (uint) info.Count;

    }

    private void OnEnable()
    {
        createNewWorldButton.onClick.AddListener(CreateWorldButton);

        EventManager.UpdateSlotPosition += UpdateSlotPosition;
    }
    private void OnDisable()
    {
        createNewWorldButton.onClick.RemoveAllListeners();

        EventManager.UpdateSlotPosition -= UpdateSlotPosition;
    }


    public void UpdateSlotPosition(string nameOfPath, uint newSlotPosition)
    {
        int findIndex = -1;
        for (int i = 0; i < info.Count; i++)
        {
            if (info[i].path == nameOfPath)
            {
                findIndex = i;
                break;
            }
        }
        if (findIndex < 0)
        {
            return;
        }
        SaveInfo newSave = info[findIndex];
        newSave.order = newSlotPosition;
        info[findIndex] = newSave;

        info.RemoveAt(findIndex);

        int insertPosition = findIndex > (int)newSlotPosition ? 1 : 0;

        info.Insert((int) newSlotPosition + insertPosition, newSave);


        //Recalc slot positions
        for (int i = insertPosition+1; i < info.Count; i++)
        {
            SaveInfo newSave2 = info[findIndex];
            newSave2.order = newSave2.order+1;
            info[findIndex] = newSave2;
        }
    }

    public void CreateWorldButton()
    {
        Debug.Log("Creating world...");

        SaveInfo s = DataService.NewSave(null, ++maxSize);

        SaveSlotUI newSlot = Instantiate(saveSlotUIPrefab, parentToAddThemTo);
        newSlot.UpdateSaveInfo(s);
    }

    public void LoadIntoWorld()
    {

    }
}
