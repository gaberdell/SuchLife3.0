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

    public GameObject CraftingUI;
    int hotbarSize = 8; //amount of panels in hotbar so this can be adjusted
    

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
        for (int i = 0; i < hotbarSize+1; i++)
        {
            if (Input.GetKeyDown(i.ToString())) // Check if a number key is pressed
            {

                if(selectedItem != -1){ //if selectedItem has changed before, we need to turn that selection off.
                    hotbar[selectedItem].GetComponent<Image>().color = new Color32(255,255,225,100);

                }
                //we have to check 0 first because 0 is 10, not 0.
                //if (i == 0)
                //{
                //    selectedItem = hotbarSize-1;

                //    hotbar[hotbarSize-1].GetComponent<Image>().color = new Color32(50, 255, 225, 100);

                //}
                //else
                {
                    //i-1 to account for 0 being 10.
                    selectedItem = i - 1;

                    hotbar[i - 1].GetComponent<Image>().color = new Color32(50, 255, 225, 100);

                }
             


            }
        }

        //Let's assume E is Inventory button, because it usually is.... //setting to P for testing as E is the interact key so it conflicts

        if (Input.GetKeyDown(KeyCode.P))
        {

            if (fullInventory != null && !CraftingUI.activeSelf)
            {

                bool isActive = fullInventory.activeSelf;

                fullInventory.SetActive(!isActive);
                gameUI.SetActive(isActive);



                this.GetComponent<helperFunctions>().togglePause();

            }

        }

        if (Input.GetKeyDown(KeyCode.Q) ){

            if (CraftingUI != null && !fullInventory.activeSelf){
                 bool isActive = CraftingUI.activeSelf;

                //if disabling the crafting menu then make the hotbar match the first row of the crafting ui
                if (isActive)
                {
                    fillHotbar();
                }

                CraftingUI.SetActive(!isActive);
                gameUI.SetActive(isActive); 
                

                this.GetComponent<helperFunctions>().togglePause();

            }

        }
        
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseSelectedItem();
        }


    }

    public void fillHotbar()
    {
        //make hotbar match top row in inventory (that is where its item data is stored)
        for(int i = 0;i<hotbarSize;i++)
        {
            GameObject panel = hotbar[i];
            //make quantity match inventory data
            Transform textChild = panel.transform.GetChild(0); // Get the quantity object
            TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();
            if (inventory[i].quantity == 0) tmp.text = "";
            else tmp.text = inventory[i].quantity.ToString();
            //match icons
            try
            {
                hotbar[i].GetComponent<Image>().sprite = inventory[i].item.icon;
            }
            catch (System.Exception)
            {
                hotbar[i].GetComponent<Image>().sprite = empty;
                //throw;
            }
            
        }
    }


    public void AddItem(Item item){

        int isInHotbar = 0;
        foreach (InventorySlot slot in inventory){

            if(slot.item == item){

                //Item already exists in inventory, therefore, we increment its amount.
                slot.quantity++; 

                 if(isInHotbar < hotbarSize){
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

            if(slot.isEmpty == true){
                slot.isEmpty = false; 
                slot.item = item; 
                slot.quantity++; 

               //If isInHotbar is 0 - 9, it's in the hotbar! (first 10 indeces of inventory are hotbar.)

                if(isInHotbar < hotbarSize){
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
    

   public void RemoveItem(int index)
    {
        int isInHotbar = index;
        InventorySlot slot = inventory[index];

        slot.quantity--;

        if (slot.quantity == 0)
        {

            //Now we also have to remove the image in hotbar.


            if (isInHotbar < hotbarSize)
            {
                //Change color to white just for debugging, so we know it's working.
                //hotbar[isInHotbar].GetComponent<Image>().color = new Color32(255,255,225,100);
                hotbar[isInHotbar].GetComponent<Image>().sprite = empty;

            }

            fullInv[isInHotbar].GetComponent<Image>().sprite = empty;


            slot.isEmpty = true;
            slot.item = null;
        }

        if (slot.quantity <= 1)
        {

            //remove number if 1 . 
            Transform textChild = hotbar[isInHotbar].transform.GetChild(0); // Get the first child
            TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();
            tmp.text = "";

            Transform textChild_inv = fullInv[isInHotbar].transform.GetChild(0); // Get the first child
            TextMeshProUGUI tmp_inv = textChild_inv.GetComponent<TextMeshProUGUI>();
            tmp_inv.text = "";

        }
        else
        {
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

   public void RemoveItem(Item item){

        int isInHotbar = 0;

        foreach (InventorySlot slot in inventory){
            if(slot.item == item){

                //slot.quantity--;

                //if(slot.quantity == 0){

                //     //Now we also have to remove the image in hotbar.


                //    if(isInHotbar < 10){
                //        //Change color to white just for debugging, so we know it's working.
                //        //hotbar[isInHotbar].GetComponent<Image>().color = new Color32(255,255,225,100);
                //        hotbar[isInHotbar].GetComponent<Image>().sprite = empty; 

                //    }

                //    fullInv[isInHotbar].GetComponent<Image>().sprite = empty; 


                //    slot.isEmpty = true; 
                //    slot.item = null;
                //}

                //if(slot.quantity <= 1){

                //    //remove number if 1 . 
                //   Transform textChild = hotbar[isInHotbar].transform.GetChild(0); // Get the first child
                //   TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();
                //   tmp.text = "";

                //    Transform textChild_inv = fullInv[isInHotbar].transform.GetChild(0); // Get the first child
                //   TextMeshProUGUI tmp_inv = textChild_inv.GetComponent<TextMeshProUGUI>();
                //   tmp_inv.text = "";

                //}
                //else{
                //      //just lower number if above one. 
                //   Transform textChild = hotbar[isInHotbar].transform.GetChild(0); // Get the first child
                //   TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();
                //   tmp.text = slot.quantity.ToString(); 

                //    Transform textChild_inv = fullInv[isInHotbar].transform.GetChild(0); // Get the first child
                //   TextMeshProUGUI tmp_inv = textChild_inv.GetComponent<TextMeshProUGUI>();
                //   tmp_inv.text = slot.quantity.ToString(); 

                //}
                RemoveItem(isInHotbar);

                
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

    public void UseSelectedItem()
    {
        if (selectedItem < 0 || selectedItem >= inventory.Count)
            return;

        InventorySlot slot = inventory[selectedItem];
        if (slot.isEmpty)
            return;

        Item item = slot.item;
        ConsumableItem consumable = item as ConsumableItem;

        if (consumable != null)
        {
            bool used = false;

            // Heal if healAmount is set
            if (consumable.healAmount > 0)
            {
                Health health = GetComponent<Health>();
                if (health != null && health.currentHealth < health.maxHealth)
                {
                    health.Heal(consumable.healAmount);
                    used = true;
                }
                else
                {
                    Debug.Log("Health is already full. Cannot use potion.");
                }
            }

            // Damage boost if values are set
            if (consumable.damageBoostAmount > 0 && consumable.boostDuration > 0)
            {
                PlayerAttack playerAttack = GetComponent<PlayerAttack>();
                if (playerAttack != null)
                {
                    playerAttack.ApplyDamageBoost(consumable.damageBoostAmount, consumable.boostDuration);
                    used = true;
                }
            }

            if (used)
            {
                //RemoveItem(item); // Only remove if something was actually used
                RemoveItem(selectedItem);
                Debug.Log($"Used {consumable.itemName}");
            }
        }
    }

}
