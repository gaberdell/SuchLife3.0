using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using static UnityEngine.Rendering.DebugUI;
using static Unity.Burst.Intrinsics.X86.Avx;
using System;


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

    //crafting
    public List<Recipe> allrecipes; //A list of all recipes. 
    public GameObject resultbox;
    public Recipe resultRecipe = null;

    //for displaying selected item in cursor
    public GameObject itemInCursorObj;
    public Item itemInCursor;
    public int itemInCursorQuantity = 0;


    void Start()
    {

    

    crafting = new List<InventorySlot>();

    firstButtonPress.type = null; 
    firstButtonPress.index = -1;

    secondButtonPress.type = null; 
    secondButtonPress.index = -1;

    itemInCursorObj.GetComponent<Image>().enabled = false;


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
        //breaks in main scene without this line but works in crafting scene without it... ???
        inventory = player.GetComponent<PlayerInventory>().inventory;

        //Coudl rewrite to use game events. Currently, this checks the panels of the Inventory and updates them to match every second.
        int count = 0;

        //move itemInCursor position to match cursor
        Vector3 mousePos = Input.mousePosition;
        itemInCursorObj.transform.position = mousePos;

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





        
    }

    //only update recipe result when a change is detected on the crafting grid
    public void updateRecipeResult()
    {
        //assume no result found by default
        bool resultFound = false;
        foreach (Recipe recipe in allrecipes)
        {
            Debug.Log(recipe);
            if (recipe.compareRecipes(current_crafting))
            { //IF WE'VE FOUND A MATCH
                Debug.Log("MATCH FOUND");
                Debug.Log(recipe.result);
                resultbox.GetComponent<Image>().sprite = recipe.result.icon;
                resultRecipe = recipe;
                resultFound = true;
            }
            else
            {
                if (!resultFound)
                {
                    //don't overwrite displayed result; stop at first match found
                    resultbox.GetComponent<Image>().sprite = player.GetComponent<PlayerInventory>().empty;
                }
            }

        }

    }


    public void onInventorySlotPress(int index){

        if (firstButtonPress.type == null){ //If nothing has been selected yet
            //ignore selections of nothing - swapping when selecting nothing first is buggy and confusing behavior
            if (inventory[index].isEmpty)
            {
                firstButtonPress.type = null;
                firstButtonPress.index = -1;
            } else
            {
                firstButtonPress.type = "inv";
                firstButtonPress.index = index;
                //when an item is selected, move the item and its data to the itemInHand container.
                //one item is picked up at a time with left click.

                //instead of changing color, we can set the sprite to be rendered around the player's cursor when selected (like minecraft & terraria)
                itemInCursorObj.GetComponent<Image>().sprite = inv[index].GetComponent<Image>().sprite;
                itemInCursorObj.GetComponent<Image>().enabled = true;
                selectItem("inv", index);
            }  
        }
        else if (secondButtonPress.type == null){
            secondButtonPress.type = "inv";
            secondButtonPress.index = index;

            //Since we have two things selected , we can swap inventory places now. 
            handleItemInCursor();
            //swapOrAdd(); 

            //reset back to nothing.
            secondButtonPress.type = null; 
            secondButtonPress.index = -1;
            updateRecipeResult();
        }
    }

    public void onCraftingGridSlotPress(int index)
    {
        if (firstButtonPress.type == null)
        {
            Debug.Log("one");
            //ignore selections of nothing
            if (crafting[index].isEmpty)
            {
                firstButtonPress.type = null;
                firstButtonPress.index = -1;
            }
            else
            {
                firstButtonPress.type = "craft";
                firstButtonPress.index = index;
                //same behavior as inventory; when an item is selected, move the item and its data to the itemInHand container.
                itemInCursorObj.GetComponent<Image>().sprite = craft[index].GetComponent<Image>().sprite;
                itemInCursorObj.GetComponent<Image>().enabled = true;
                selectItem("craft", index);
            }
            updateRecipeResult();

        }
        else if (secondButtonPress.type == null)
        {
            Debug.Log("two");
            secondButtonPress.type = "craft";
            secondButtonPress.index = index;
            //determine whether first button was in crafting grid or not
            //if (firstButtonPress.type == "craft")
            //{
            //    //if in craft then clear first crafting slot
            //    craft[firstButtonPress.index].GetComponent<Image>().color = new Color32(255, 255, 225, 100);
            //}
            //else
            //{
            //    //dont try to clear crafting slot because first click was not in crafting slot
            //}

            //swap selection with cursor
            handleItemInCursor();

            //reset back to nothing.
            secondButtonPress.type = null;
            secondButtonPress.index = -1;




            //Since we have two things selected , we can swap inventory places now. 

            //swapOrAdd();

            ////reset back to nothing.
            //firstButtonPress.type = null;
            //firstButtonPress.index = -1;

            //secondButtonPress.type = null;
            //secondButtonPress.index = -1;

        }

    }

    //handler for picking up an item with the cursor
    void selectItem(string type, int index)
    {

        //add an instance of the selected item to itemInCursor
        if (type == "inv")
        {
            itemInCursor = inventory[index].item;
        }
        else
        {
            itemInCursor = crafting[index].item;
        }
        itemInCursorQuantity = 1;
        //remove item from slot
        RemoveItemInSlot(type, index);
        Cursor.visible = false;
    }

    void clearSelectedItem()
    {
        itemInCursor = null;
        itemInCursorQuantity = 0;
        //stop displaying image over cursor
        itemInCursorObj.GetComponent<Image>().enabled = false;
        Cursor.visible = true;
    }

    //This differs from AddItem in PLayerInventory, because this is adding an item to a specific slot, not just the first avaliable one. Type is if we're adding to the crafting grid or inventory grid.
    //Type and Index are the type and index of the destination.
    void AddItemInSlot(Item item, string type, int index, int quantity = 1){

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
            destination.quantity = quantity; 

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
            if (destination.quantity <= 1)
            {
                current_crafting[index] = emptyitem; //Changes the current item in this index to empty when removing a lone item
            }
        }
        else{
            //oops!
            destination = null;
        }
        if (destination.isEmpty)
        {
            //dont remove anything if empty
            return;
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

    //when player clicks while they have an item in the cursor, decide what to do with it; drop item in empty/same item box or swap itemInCursor with item in box
    void handleItemInCursor()
    {
        InventorySlot target;
        if (secondButtonPress.type == "inv") target = inventory[secondButtonPress.index];
        else target = crafting[secondButtonPress.index];

        //if target is empty then drop the currently selected item in it
        if (target.isEmpty)
        {
            AddItemInSlot(itemInCursor, secondButtonPress.type, secondButtonPress.index, itemInCursorQuantity);
            clearSelectedItem();
            //reset first selected item
            firstButtonPress.type = null;
            firstButtonPress.index = -1;
        } else
        {
            //otherwise we want to swap the selected item with the item in target
            //if they are the same items then increment the quantity of the second item
            if (target.item == itemInCursor)
            {
                AddItemInSlot(itemInCursor, secondButtonPress.type, secondButtonPress.index, itemInCursorQuantity);
                clearSelectedItem();
                Cursor.visible = true;
                //reset first selected item
                firstButtonPress.type = null;
                firstButtonPress.index = -1;

            } else
            {
                //set sprite for cursor
                //swap item in cursor with selected item
                int q;
                Item i;
                if (secondButtonPress.type == "inv")
                {
                    itemInCursorObj.GetComponent<Image>().sprite = inv[secondButtonPress.index].GetComponent<Image>().sprite;
                    q = inventory[secondButtonPress.index].quantity;
                    i = inventory[secondButtonPress.index].item;
                    inventory[secondButtonPress.index].item = itemInCursor;
                    inventory[secondButtonPress.index].quantity = itemInCursorQuantity;
                } else if(secondButtonPress.type == "craft")
                {
                    Debug.Log("craft swap");
                    itemInCursorObj.GetComponent<Image>().sprite = craft[secondButtonPress.index].GetComponent<Image>().sprite;
                    q = crafting[secondButtonPress.index].quantity;
                    i = crafting[secondButtonPress.index].item;
                    crafting[secondButtonPress.index].item = itemInCursor;
                    crafting[secondButtonPress.index].quantity = itemInCursorQuantity;
                    current_crafting[secondButtonPress.index] = crafting[secondButtonPress.index].item;
                } else
                {
                    Debug.Log("invalid type for item destination");
                    return;
                }

                itemInCursor = i;
                itemInCursorQuantity = q;
                //act as if this item was clicked by a first press
                firstButtonPress.type = secondButtonPress.type;
            }
        }
        updateRecipeResult();
    }


    public void onCraftingResultPress()
    {
        //check if there is an item in the panel (successful recipe found)
        Item resultItem = resultRecipe.result;
        if(resultItem != null)
        {
            //put item in cursor
            itemInCursor = resultItem;
            itemInCursorQuantity = 1;
            itemInCursorObj.GetComponent<Image>().sprite = resultItem.icon;
            firstButtonPress.type = "craft";
            firstButtonPress.index = 9; //necessary?
            itemInCursorObj.GetComponent<Image>().enabled = true;
            //clear result box
            resultbox.GetComponent<Image>().sprite = player.GetComponent<PlayerInventory>().empty;
            //clear grid corresponding to how many items are made
            for (int i = 0; i < crafting.Count; i++)
            {
                RemoveItemInSlot("craft", i);
            }
            //more of the same items might still be craftable; check again 
        }
    }
    


    /*---------------------------------------------------------------------DEPRECATED; KEPT FOR POSTERITY-----------------------------------------------------------------------------------*/


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



  
}
