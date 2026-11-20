using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System.Linq;

/// <summary>
/// A class with static methods <c>InventoryIntoByteArr</c> and <c>ByteArrIntoInventory</c>,
/// which allow conversion of a player's Inventory field into a byte array in a special format. 
/// </summary>
/// <remarks>
/// 
/// Format
/// ______
///
///             1 byte 
///          +-----------+
///          |     0     |
///          +-----------+
///
///
/// --------------------- <ITEM NAME DICTIONARY> ---------------------
///
///                     1 byte
///          +---------------------------+
///          |  Dictionary Length < 255  |
///          +---------------------------+
///
///                1 byte              string       
///          +-----------------+---------------------+
///          |   Name Length   |   Item Name ASCII   |
///          +-----------------+---------------------+
///          |   Name Length   |   Item Name ASCII   |
///          +-----------------+---------------------+
///          |   Name Length   |   Item Name ASCII   |
///          +---------------- ...
///
/// --------------------- </ITEM NAME DICTIONARY> --------------------
///
///
///             1 byte
///          +-----------+
///          |     0     |
///          +-----------+
///
///
/// ----------------------------------- <INVENTORY> -----------------------------------
///
///
///        Inventory Slot Numbering: Top left 1, moves right then down like a book. 
///        Hotbar is the first row of the Inventory.
///
///
///                  1 byte
///          +----------------------+
///          |   Inventory Length   |
///          +----------------------+
///
///            1 byte               1 byte
///          +---------+--------------------------------+
///          |  Count  |   Item Name Dictionary Index   |
///          +---------+--------------------------------+
///          |  Count  |   Item Name Dictionary Index   |
///          +---------+--------------------------------+
///          |  Count  |   Item Name Dictionary Index   |
///          +----- (Repeat until Inventory full) ------+
///
///             1 byte
///          +-----------+
///          |    255    |
///          +-----------+
///
/// ----------------------------------- </INVENTORY> ----------------------------------
/// </remarks>
public class InventorySaveLoad {
    /// <summary>
    /// Method <c>InventoryIntoByteArr</c> takes a <c>List<InventorySlot></c>
    /// which represents a player's Inventory and turns it into a byte array.
    /// </summary>
    /// <param name="inventory">The player's Inventory as a <c>List<InventorySlot></c></param>
    /// <returns>A byte array representing <paramref name="inventory"/>/returns>
    public static byte[] InventoryIntoByteArr(List<InventorySlot> inventory)
    {
        List<byte> retval = new () {0};
        Item[] allItems = Resources.LoadAll<Item>("Items");
        Dictionary<string,byte> itemDict = new();
        byte index = 0;

        retval.Add((byte) allItems.Length);
        foreach (Item i in allItems)
        {
            itemDict.Add(i.itemName,index++);
            retval.Add((byte) i.itemName.Length);
            retval.AddRange(Encoding.ASCII.GetBytes(i.itemName));
        }
        retval.Add(0);

        retval.Add((byte) inventory.Count);
        foreach (InventorySlot slot in inventory)
        {
            retval.Add((byte) slot.quantity);
            retval.Add(itemDict[slot.item.itemName]);
        }
        retval.Add(255);
        
        return retval.ToArray();
    }


    /// <summary>
    /// Method <c>ByteArrIntoInventory</c> takes a <c>byte[]</c>
    /// representing a player's Inventory and turns it into a <c>List<InventorySlot></c>,
    /// which is the type of inventory field in <c>PlayerInventory</c>
    /// </summary>
    /// <param name="arr">The byte array representing a player's Inventory</param>
    /// <returns>
    /// <c>null</c> if <paramref name="arr"/> is a byte array that has invalid format 
    /// according to this class's specifications.
    /// Otherwise, A <c>List<InventorySlot></c> that can be used as a field for a 
    /// player's inventory field.
    /// </returns>
    public static List<InventorySlot> ByteArrIntoInventory(byte[] arr)
    {
        Item[] allItems = Resources.LoadAll<Item>("Items");
        Dictionary<string,Item> itemDict = allItems.ToDictionary(x => x.name, x => x);
        List<string> itemStrs = new();

        int index = 0;
        if (arr[index++] != 0) { return null; }
        for (int i = arr[index++]; i > 0; --i)
        {
            itemStrs.Add(Encoding.ASCII.GetString(arr,index+1,arr[index]));
            index += arr[index]+1;
        }
        if (arr[index++] != 0) { return null; }

        List<InventorySlot> retval = new();
        for (int i = arr[index++]; i > 0; --i)
        {
            InventorySlot thisSlot = new()
            {
                quantity = arr[index],
                isEmpty = arr[index] == 0,
                item = arr[index++] == 0 ? null : itemDict[itemStrs[index++]]
            };
            retval.Add(thisSlot);
        }
        if (arr[index++] != 255) { return null; }
        return retval;
    }
    
}