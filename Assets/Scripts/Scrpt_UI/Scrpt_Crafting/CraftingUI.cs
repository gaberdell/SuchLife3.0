using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;


[System.Serializable]
public struct tuple_struct
{
    public int index;
    public string type;
}

public class CraftingUI : MonoBehaviour
{

    public GameObject player; 

    //the actual data inventory
    public List<InventorySlot> inventory;
    //the actual data of the crafting grid.
    public List<InventorySlot> crafting;


    public GameObject inv_panel; //Full inventory panel
    public GameObject craftinggrid;  //Crafting Grid Button

    public List<GameObject> inv = new List<GameObject>();  //list of all the game objs


    //This doesn't have to be public, but I wanna see this in inspector when I debug, soooo. Cou
    public tuple_struct firstButtonPress; 
    public tuple_struct secondButtonPress;

    //TODO:




    void Start()
    {

    firstButtonPress.type = null; 
    firstButtonPress.index = -1;

    secondButtonPress.type = null; 
    secondButtonPress.index = -1;

           for (int i = 0; i < inv_panel.transform.childCount; i++)
            {
                inv.Add(inv_panel.transform.GetChild(i).gameObject);

    

            }    


        inventory =  player.GetComponent<PlayerInventory>().inventory;
   
        crafting = new List<InventorySlot>(new InventorySlot[9]);  // Initializes with x zeros

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

        //Coudl rewrite to use game events.
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
            else {

                inv[count].GetComponent<Image>().sprite = player.GetComponent<PlayerInventory>().empty; 
                inv[count].GetComponent<Image>().color = new Color32(255,255,225,100);
                tmp.text = "";




            }
           
           count++; 



        }
        
    }


    public void onInventorySlotPress(int index){

        if (firstButtonPress.type == null){ //If we've selected nothing. 
            firstButtonPress.type = "inv";
            firstButtonPress.index = index;

            //change color too!!!
            inv[index].GetComponent<Image>().color = new Color32(50,255,225,100);
                        Debug.Log("Selected first!");



        }
        else if (secondButtonPress.type == null){
            secondButtonPress.type = "inv";
            secondButtonPress.index = index;

            Debug.Log("Selected second!");
            inv[index].GetComponent<Image>().color = new Color32(255,255,225,100);


            //Since we have two things selected , we can swap inventory places now. 

            swapOrAdd(); 

            //reset back to nothing.
            firstButtonPress.type = null; 
            firstButtonPress.index = -1;

            secondButtonPress.type = null; 
            secondButtonPress.index = -1;
        }
        
        

    }

    //This differs from AddItem in PLayerInventory, because this is adding an item to a specific slot, not just the first avaliable one. Type is if we're adding to the crafting grid or inventory grid.
    //Type and Index are the type and index of the destination.
    void AddItemInSlot(Item item, string type, int index){

        InventorySlot destination; 
    
        if(type == "inv"){

            destination = inventory[index];

        }
        else if(type == "craft"){

            destination = crafting[index];
        }
        else{
            //oops!
            destination = null;
        }

        
        if(destination.isEmpty ){  //if is empty. Just in case we wanna add grouping function.
            destination.isEmpty = false; 
            destination.item = item;
            destination.quantity = 1; 

        } 
        else{ //If destination isn't empty. 

            if(destination.item == item){ // will only be called if this is trye outside, but no harm in double checking.
                destination.quantity += 1; 
            }

        }



    }

    //This is only done for the source of a click. T
    void RemoveItemInSlot(string type, int index){

        InventorySlot destination; 
    
        if(type == "inv"){

            destination = inventory[index];

        }
        else if(type == "craft"){

            destination = crafting[index];
        }
        else{
            //oops!
            destination = null;
        }

        if(destination.quantity == 1 ){ 
            Debug.Log("Removed") ;
            destination.isEmpty = true; 
            destination.item = null;
            destination.quantity = 0; 

        } 
        else{ // we just subtract one.
            destination.quantity -= 1; 


        }


    }


    //If the second slot is empty and doesn't have an item in it, we want to move one of our first selected thing into there. If we've selected two things, we should swap.
    void swapOrAdd(){

        //recognize which slot we're talking about.
        InventorySlot first; 
        InventorySlot sec; 


        if (firstButtonPress.type == "inv"){
            first = inventory[firstButtonPress.index]; 
        }
        else if (firstButtonPress.type == "craft"){
            first = crafting[firstButtonPress.index]; 

        }
        else {
            //Shouldnt happen lol
            first = null;
        }

        /// Do the same for sec.

        if (secondButtonPress.type == "inv"){
            sec = inventory[secondButtonPress.index];

        }
        else if (secondButtonPress.type == "craft"){
            sec = crafting[secondButtonPress.index]; 
        }
        else {
            //also shouldnt happen.
            sec = null;
        }
    
   
        //Logic time.

        if(first.isEmpty && sec.isEmpty){ // if both empty
            //do nothing lol 
        }
        if(first.isEmpty && !sec.isEmpty){ 
            first = sec; 
            //also do nothing.
        }
        if((!first.isEmpty && sec.isEmpty)){ //if first is empty, we add one . ADD

            AddItemInSlot(first.item, secondButtonPress.type, secondButtonPress.index ); //adds FIRST'S item to Sec location.
            RemoveItemInSlot(firstButtonPress.type, firstButtonPress.index); 

        }
        else if(!first.isEmpty && !sec.isEmpty){ // if they both have items, we swap.
            // Debug.Log("The two selected things are:");
            // Debug.Log("first = " + firstButtonPress.type + ", " + firstButtonPress.index + ": " + first.item.itemName );
            // Debug.Log("sec = " + secondButtonPress.type + ", " + secondButtonPress.index + ": " + sec.item.itemName );


            Debug.Log("Swap!");
            ///// girl youre dumb. 
            InventorySlot temp; 

            temp = sec; 
            sec = first;
            first = temp; 

            inventory[firstButtonPress.index] = first;
            inventory[secondButtonPress.index] = sec;


            // Debug.Log("Now:");
            // Debug.Log("first : " + first.item.itemName );
            // Debug.Log("sec : " + sec.item.itemName );

            
        


        }




    }

    public void onCraftingGridSlotPress(int index){
        if (firstButtonPress.type == null){ //If we've selected nothing. 
            firstButtonPress.type = "craft";
            firstButtonPress.index = index;

        }
        else if (secondButtonPress.type == null){
            secondButtonPress.type = "craft";
            secondButtonPress.index = index;

            //Since we have two things selected , we can swap inventory places now. 

            swapOrAdd(); 

            //reset back to nothing.
            firstButtonPress.type = null; 
            firstButtonPress.index = -1;

            secondButtonPress.type = null; 
            secondButtonPress.index = -1;

        }

    }

  
}
