using TMPro;
using UnityEngine;

public class SaveSlotUi : MonoBehaviour
{
    [SerializeField]
    private SaveInfo currentSaveInfo;

    [SerializeField]
    private TextMeshProUGUI topText;

    private uint currentOrder;

    public void UpdateSaveInfo(SaveInfo newSaveInfo)
    {
        currentSaveInfo = newSaveInfo;
        topText.text = newSaveInfo.name;
        currentOrder = newSaveInfo.order;
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
