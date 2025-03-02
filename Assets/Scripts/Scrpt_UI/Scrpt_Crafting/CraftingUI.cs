using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;


public class CraftingUI : MonoBehaviour
{

    public GameObject player; 

    public GameObject inv_panel; 


    //TODO:


    void Start()
    {
        gameObject.SetActive(false);

        // foreach (Transform panel in inv_panel)
        // {
            
        //         Transform textChild = panel.transform.GetChild(0); // Get the first child
        //         TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();

        //         tmp.text = ""; 
        //         tmp.color = new Color32(255,255,225,100);


            
        
        // }

        
    }

    void onEnable(){

        // int count = 0; 
        // foreach (Transform panel in inv_panel)
        // {
        //     Transform textChild = panel.transform.GetChild(0); // Get the first child
        //     TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();

        //     if(!player.inventory[count].isEmpty){

        //         tmp.text = player.inventory[count].quantity; 
        //         tmp.color = new Color32(255,255,225,100);
        //         panel[count].GetComponent<Image>().sprite = player.inventory[count].item.icon; 
        //     player.GetComponent<TextMeshProUGUI>();



        //     }
           



        // }

    }




    // Update is called once per frame
    void Update()
    {

        int count = 0; 
        foreach (InventorySlot slot in player.GetComponent<PlayerInventory>().inventory){
            if(slot.isEmpty == false){
                    panel.




            }
        }

        
    }

  
}
