using UnityEngine;

public class CanvasController : MonoBehaviour
{

    public GameObject noUI;
    public GameObject inventoryUI;
    public GameObject craftingUI; 

    GameObject currentUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        currentUI = noUI; 
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
