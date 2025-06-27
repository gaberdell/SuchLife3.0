using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;


//Right now, 
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
    public List<GameObject> craft = new List<GameObject>();  //list of all the crafting objs oanels


    public tuple_struct firstButtonPress; 
    public tuple_struct secondButtonPress;


    public List<Item> current_crafting = new List<Item>();  //The list of items currently in Crafting. All initialized to null initially, and it goes from 0-8 from top left as 0 and bottom right as 8. 

    public Item emptyitem; //Used in comparisons. 

    public List<Recipe> allrecipes; //A list of all recipes. 

    public GameObject resultbox; 


    void Start()
    {

    

    crafting = new List<InventorySlot>();


    firstButtonPress.type = null; 
    firstButtonPress.index = -1;

    secondButtonPress.type = null; 
    secondButtonPress.index = -1;

           for (int i = 0; i < inv_panel.transform.childCount; i++)
            {
                inv.Add(inv_panel.transform.GetChild(i).gameObject);

            }    

            for (int i = 0; i < craftinggrid.transform.childCount; i++) //same thing w crafting
            {
                craft.Add(craftinggrid.transform.GetChild(i).gameObject);

            }   


      inventory = player.GetComponent<PlayerInventory>().inventory;
   
      for(int i = 0; i < 9; i++)
        {
            InventorySlot slot = new InventorySlot(); 
            slot.isEmpty = true;
            crafting.Add(slot);
            current_crafting.Add(emptyitem); //Because nothing is in crafting grid, fill it with 'empty' items.
             
        }
 
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

        Debug.Log("enabled");

        for (int i = 0; i < 9; i++){
            crafting[i].isEmpty = true;

        }

        int count = 0; 

        foreach (GameObject panel in inv)
        {
            Transform textChild = panel.transform.GetChild(0); // Get the first child
            TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();
            if (!(player.GetComponent<PlayerInventory>().inventory)[count].isEmpty){

                tmp.text = (player.GetComponent<PlayerInventory>().inventory)[count].quantity.ToString(); 

                tmp.color = new Color32(255,255,225,100);
                inv[count].GetComponent<Image>().sprite = (player.GetComponent<PlayerInventory>().inventory)[count].item.icon; 
               // player.GetComponent<TextMeshProUGUI>();

            }
           
           count++; 


        }   
        //Enable Craft Grid.

        
        foreach (GameObject panel in craft)
        {
            
                Transform textChild = panel.transform.GetChild(0); // Get the first child
                TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();

                tmp.text = ""; 
                tmp.color = new Color32(255,255,225,100);
        
        }


    }




    // Update is called once per frame
    void Update()
    {
        inventory = player.GetComponent<PlayerInventory>().inventory;

        //Coudl rewrite to use game events. Currently, this checks the panels of the Inventory and updates them to match every second.
        int count = 0;

        foreach (GameObject panel in inv)
        {
            Transform textChild = panel.transform.GetChild(0); // Get the first child
            TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();

            if(!(player.GetComponent<PlayerInventory>().inventory)[count].isEmpty){

                if((player.GetComponent<PlayerInventory>().inventory)[count].quantity > 1){
                    tmp.text = (player.GetComponent<PlayerInventory>().inventory)[count].quantity.ToString(); 

                }else {
                    tmp.text = "" ;

                }

                
                tmp.color = new Color32(255,255,225,100);


                if (inventory[count].item != null){
                 inv[count].GetComponent<Image>().sprite = (player.GetComponent<PlayerInventory>().inventory)[count].item.icon; 
                }

            }
            else {

                inv[count].GetComponent<Image>().sprite = player.GetComponent<PlayerInventory>().empty; 
                inv[count].GetComponent<Image>().color = new Color32(255,255,225,100);
                tmp.text = "";

            }
            
           count++; 


        }

        //the same for crafting.

        count = 0;
        foreach (GameObject panel in craft)
        {
            Transform textChild = panel.transform.GetChild(0); // Get the first child
            TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();


            if(!(crafting[count].isEmpty)){ //if we have something in crafting 

                if( crafting[count].quantity > 1){
                tmp.text = crafting[count].quantity.ToString(); 
                tmp.color = new Color32(255,255,225,100);
                }
                else {
                    tmp.text = "" ;

                }
               

                //breaking!!

                if(crafting[count].item != null){
                  craft[count].GetComponent<Image>().sprite = crafting[count].item.icon; 

                }
               // player.GetComponent<TextMeshProUGUI>();



            }
            else {

                craft[count].GetComponent<Image>().sprite = player.GetComponent<PlayerInventory>().empty; 
                craft[count].GetComponent<Image>().color = new Color32(255,255,225,100);
                tmp.text = "";




            }
           
           count++; 


        }


        //Now we need to update what Results looks like.

        foreach (Recipe recipe in allrecipes){
            if(recipe.compareRecipes(current_crafting)){ //IF WE'VE FOUND A MATCH

                resultbox.GetComponent<Image>().sprite = recipe.result.icon;
                Debug.Log("True!!!");
            }
            else{

                resultbox.GetComponent<Image>().sprite = player.GetComponent<PlayerInventory>().empty; 
            }

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
            inv[firstButtonPress.index].GetComponent<Image>().color = new Color32(255,255,225,100);


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
            current_crafting[index] = item; //Changes the current item in this index to the passed in icon.
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
            current_crafting[index] = emptyitem; //Changes the current item in this index to empty

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

            //First, check if theyre the same. If they are, we wanna merge first into second.

            if (first.item == sec.item){
                AddItemInSlot(sec.item, secondButtonPress.type, secondButtonPress.index );

                RemoveItemInSlot(firstButtonPress.type, firstButtonPress.index);

                    if (firstButtonPress.type == "inv"){
                    inventory[firstButtonPress.index] = first;
                    }
                    else if (firstButtonPress.type == "craft"){
                    crafting[firstButtonPress.index] = first;

                    }
                

                    /// Do the same for sec.
                    if (secondButtonPress.type == "inv"){
                    inventory[secondButtonPress.index] = sec;
                    }
                    else if (secondButtonPress.type == "craft"){
                    crafting[secondButtonPress.index] = sec;

                    }
                return; 
            }


            Debug.Log("Swap!");
            ///// girl youre dumb. 
            InventorySlot temp; 

            temp = sec; 
            sec = first;
            first = temp; 

            if (firstButtonPress.type == "inv"){
             inventory[firstButtonPress.index] = first;
            }
            else if (firstButtonPress.type == "craft"){
             crafting[firstButtonPress.index] = first;

            }
        

            /// Do the same for sec.
             if (secondButtonPress.type == "inv"){
             inventory[secondButtonPress.index] = sec;
            }
            else if (secondButtonPress.type == "craft"){
             crafting[secondButtonPress.index] = sec;

            }


        }




    }

    public void onCraftingGridSlotPress(int index){
        if (firstButtonPress.type == null){ //If we've selected nothing. 
            firstButtonPress.type = "craft";
            firstButtonPress.index = index;
            craft[index].GetComponent<Image>().color = new Color32(50,255,225,100);


        }
        else if (secondButtonPress.type == null){
            secondButtonPress.type = "craft";
            secondButtonPress.index = index;

            Debug.Log(firstButtonPress.index);
            craft[index].GetComponent<Image>().color = new Color32(255,255,225,100);
            //determine whether first button was in crafting grid or not
            if(firstButtonPress.type == "craft")
            {
                //if in craft then clear first crafting slot
                craft[firstButtonPress.index].GetComponent<Image>().color = new Color32(255,255,225,100);
            }
            else
            {
                //dont try to clear crafting slot because first click was not in crafting slot
            }



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
