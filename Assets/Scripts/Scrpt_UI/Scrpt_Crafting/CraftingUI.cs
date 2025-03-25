using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;


public class CraftingUI : MonoBehaviour
{

    public GameObject player; 

    //the actual data inventory
    public List<InventorySlot> inventory;


    public GameObject inv_panel; 
    public GameObject craftinggrid; 

    public List<GameObject> inv = new List<GameObject>(); 

    //TODO:


    void Start()
    {
                       Debug.Log("HERE!");

           for (int i = 0; i < inv_panel.transform.childCount; i++)
            {
                inv.Add(inv_panel.transform.GetChild(i).gameObject);

    

            }    


        inventory =  player.GetComponent<PlayerInventory>().inventory;
        //  for (int i = 0; i < fullInventory.transform.childCount; i++)
        //     {
        //         fullInv.Add(fullInventory.transform.GetChild(i).gameObject);
            

        //     }   

        foreach (GameObject panel in inv)
        {
            
                Transform textChild = panel.transform.GetChild(0); // Get the first child
                TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();

                tmp.text = ""; 
                tmp.color = new Color32(255,255,225,100);
        
        }

        gameObject.SetActive(false);

        
    }

    void onEnable(){
                       Debug.Log("HERE!");


        int count = 0; 
        foreach (GameObject panel in inv)
        {
            Transform textChild = panel.transform.GetChild(0); // Get the first child
            TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();

            if(!(player.GetComponent<PlayerInventory>().inventory)[count].isEmpty){

                tmp.text = (player.GetComponent<PlayerInventory>().inventory)[count].quantity.ToString(); 
                tmp.color = new Color32(255,255,225,100);
                inv[count].GetComponent<Image>().sprite = (player.GetComponent<PlayerInventory>().inventory)[count].item.icon; 
               // player.GetComponent<TextMeshProUGUI>();



            }
           
           count++; 



        }




    }




    // Update is called once per frame
    void Update()
    {

        // int count = 0; 
        // foreach (InventorySlot slot in player.GetComponent<PlayerInventory>().inventory){
        //     if(slot.isEmpty == false){
                   
                   




        //     }
        // }
 int count = 0; 
        foreach (GameObject panel in inv)
        {
            Transform textChild = panel.transform.GetChild(0); // Get the first child
            TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();

            if(!(player.GetComponent<PlayerInventory>().inventory)[count].isEmpty){

                tmp.text = (player.GetComponent<PlayerInventory>().inventory)[count].quantity.ToString(); 
                tmp.color = new Color32(255,255,225,100);
                inv[count].GetComponent<Image>().sprite = (player.GetComponent<PlayerInventory>().inventory)[count].item.icon; 
               // player.GetComponent<TextMeshProUGUI>();



            }
           
           count++; 



        }
        
    }

  
}
