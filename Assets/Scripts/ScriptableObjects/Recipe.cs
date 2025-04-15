using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
[System.Serializable]
public class Recipe : ScriptableObject
{
    public List<Item> recipe_details; //If AnyOrder = true, don't add any nothing spaces. If it is false, do add nothing spaces ot designate shape.
    public Item result; 
    public bool anyOrder; //True if you can make this in any order. False if not. 
    public Item emptyitem; //For ease of comparison. 




    public bool compareRecipes(List<Item> current_crafting){

        if(anyOrder){
            List<Item> filtered_in = current_crafting.Where(item => item != emptyitem).ToList();

            return filtered_in.SequenceEqual(recipe_details);
        }
        else{
            return current_crafting.SequenceEqual(recipe_details);
        }

    } 
}