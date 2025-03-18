using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;


public class PlayerInventory : MonoBehaviour
{

    public List<InventorySlot> inventory; 

    public GameObject gameUI; 
    
    //Parent objects.
    public GameObject hotbarPanel; 
    public GameObject fullInventory; 


    //List of all the panels in Hotbar and Fullnav.
    public List<GameObject> hotbar = new List<GameObject>(); 
    public List<GameObject> fullInv = new List<GameObject>(); 


    //our current selected item, 0-9.
    public int selectedItem;

    int inventorySize = 40; 

    public Sprite empty; 

    

    void Start(){

        selectedItem = -1; //nothing selected to begin with.

        inventory = new List<InventorySlot>();
        //Get panels as hotbar.

      
        
        //Get all children of hotbarPanel (So every hotbar button) and add them to Hotbar.
        for (int i = 0; i < hotbarPanel.transform.childCount; i++)
            {
                hotbar.Add(hotbarPanel.transform.GetChild(i).gameObject);

    

            }    

           // Debug.Log(hotbar.Count);

        //Do the same for FullInventory and fullInv
        for (int i = 0; i < fullInventory.transform.childCount; i++)
            {
                fullInv.Add(fullInventory.transform.GetChild(i).gameObject);
            

            }   

        fullInventory.SetActive(false);
        for(int i = 0; i < inventorySize; i++)
        {
            InventorySlot slot = new InventorySlot(); 
            slot.isEmpty = true;
            inventory.Add(slot); 
        }


        //Making all the quantity texts blank for both the hotbar and the fullinv.

        foreach (GameObject panel in hotbar)
        {
            if (panel.transform.childCount > 0) // Ensure it has a child
            {
                Transform textChild = panel.transform.GetChild(0); // Get the first child
                TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();

                tmp.text = ""; 
                tmp.color = new Color32(255,255,225,100);

            }
        
        }

        foreach (GameObject panel in fullInv)
        {
            if (panel.transform.childCount > 0) // Ensure it has a child
            {
                Transform textChild = panel.transform.GetChild(0); // Get the first child
                TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();

                tmp.text = ""; 
                tmp.color = new Color32(255,255,225,100);


            }
        
        }


    }


    void Update()
    {
        for (int i = 0; i <= 9; i++)
        {
            if (Input.GetKeyDown(i.ToString())) // Check if a number key is pressed
            {

                if(selectedItem != -1){ //if selectedItem has changed before, we need to turn that selection off.
                    hotbar[selectedItem].GetComponent<Image>().color = new Color32(255,255,225,100);

                }
                //we have to check 0 first because 0 is 10, not 0.
                if(i == 0){
                    selectedItem = 9;

                    hotbar[9].GetComponent<Image>().color = new Color32(50,255,225,100);

                }
                else{
                    //i-1 to account for 0 being 10.
                    selectedItem = i-1;

                    hotbar[i-1].GetComponent<Image>().color = new Color32(50,255,225,100);

                }
             


            }
        }

        //Let's assume E is Inventory button, because it usually is.... 

        if(Input.GetKeyDown(KeyCode.E) ){

            if (fullInventory != null){
    
                bool isActive = fullInventory.activeSelf;
                
                fullInventory.SetActive(!isActive);
                gameUI.SetActive(isActive); 
                

                this.GetComponent<helperFunctions>().togglePause();

            }

        }


    }
 


    public void AddItem(Item item){
        Debug.Log("Adding Item...");

        int isInHotbar = 0;
        foreach (InventorySlot slot in inventory){

            if(slot.item == item){
                Debug.Log("Already exists, increasing quantity!");

                //Item already exists in inventory, therefore, we increment its amount.
                slot.quantity++; 

                 if(isInHotbar < 10){
                    //Change color to teal just for debugging, so we know it's working.
                   //hotbar[isInHotbar].GetComponent<Image>().color = new Color32(50,255,225,100);
                  //hotbar[isInHotbar].GetComponent<Image>().sprite = slot.item.icon; 

                   Transform textChild = hotbar[isInHotbar].transform.GetChild(0); // Get the first child
                   TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();
                   tmp.text = slot.quantity.ToString(); 

                }

               // fullInv[isInHotbar].GetComponent<Image>().color = new Color32(50,255,225,100);
                  fullInv[isInHotbar].GetComponent<Image>().sprite = slot.item.icon; 

                   Transform textChild_inv = fullInv[isInHotbar].transform.GetChild(0); // Get the first child
                   TextMeshProUGUI tmp_inv = textChild_inv.GetComponent<TextMeshProUGUI>();
                   tmp_inv.text = slot.quantity.ToString(); 


    
                return; 
            }
            isInHotbar++;
        }

        isInHotbar = 0;
        
        //If we go through entire loop and havent found a match, we simply add it to next free slot.
         foreach (InventorySlot slot in inventory){
            Debug.Log("Adding to next free slot!");

            if(slot.isEmpty == true){
                slot.isEmpty = false; 
                slot.item = item; 
                slot.quantity++; 

               //If isInHotbar is 0 - 9, it's in the hotbar! (first 10 indeces of inventory are hotbar.)

                if(isInHotbar < 10){
                    //Change color to teal just for debugging, so we know it's working.
                   //hotbar[isInHotbar].GetComponent<Image>().color = new Color32(50,255,225,100);
                  hotbar[isInHotbar].GetComponent<Image>().sprite = slot.item.icon; 

                }

               // fullInv[isInHotbar].GetComponent<Image>().color = new Color32(50,255,225,100);
                  fullInv[isInHotbar].GetComponent<Image>().sprite = slot.item.icon; 


                return; 
            }
            isInHotbar++;
        }

        

    }

   public void RemoveItem(Item item){

        int isInHotbar = 0;

        foreach (InventorySlot slot in inventory){
            if(slot.item == item){
                Debug.Log("Removing Item");

                slot.quantity--;

                if(slot.quantity == 0){
                     Debug.Log("0 quantity, emptying slot.");

                     //Now we also have to remove the image in hotbar.

                     
                    if(isInHotbar < 10){
                        //Change color to white just for debugging, so we know it's working.
                        //hotbar[isInHotbar].GetComponent<Image>().color = new Color32(255,255,225,100);
                        hotbar[isInHotbar].GetComponent<Image>().sprite = empty; 

                    }

                    fullInv[isInHotbar].GetComponent<Image>().sprite = empty; 


                    slot.isEmpty = true; 
                    slot.item = null;
                }

                if(slot.quantity <= 1){

                    //remove number if 1 . 
                   Transform textChild = hotbar[isInHotbar].transform.GetChild(0); // Get the first child
                   TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();
                   tmp.text = "";

                    Transform textChild_inv = fullInv[isInHotbar].transform.GetChild(0); // Get the first child
                   TextMeshProUGUI tmp_inv = textChild_inv.GetComponent<TextMeshProUGUI>();
                   tmp_inv.text = "";

                }
                else{
                      //just lower number if above one. 
                   Transform textChild = hotbar[isInHotbar].transform.GetChild(0); // Get the first child
                   TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();
                   tmp.text = slot.quantity.ToString(); 

                    Transform textChild_inv = fullInv[isInHotbar].transform.GetChild(0); // Get the first child
                   TextMeshProUGUI tmp_inv = textChild_inv.GetComponent<TextMeshProUGUI>();
                   tmp_inv.text = slot.quantity.ToString(); 

                }


                
                return; 
            }
            isInHotbar++;
        }

    }

   public void PrintAllItemsToDebug(){
        foreach (InventorySlot slot in inventory){
            if(slot.isEmpty == false){
              Debug.Log(slot.item.itemName + " : Quantity " + slot.quantity );

            }
        }
    }




}
