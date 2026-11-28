using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
 
 
public class PlayerInventory : MonoBehaviour
{
 
    public List<InventorySlot> inventory; 
 
    public GameObject gameUI;
 
    private string gameUITag = "GameUI";
    
    //Parent objects.
    public GameObject hotbarPanel;
    private string hotbarPanelTag = "HotbarPanel";
 
 
    public GameObject fullInventory;
    private string fullInventoryTag = "FullInventory";
 
    private PlayerInfo playerInfo;
 
 
    //List of all the panels in Hotbar and Fullnav.
    public List<GameObject> hotbar = new List<GameObject>(); 
    public List<GameObject> fullInv = new List<GameObject>(); 
 
 
    //The hotbar slot each hand has selected, indexed with (int)Hand. -1 = nothing selected.
    //If both hands select the same slot, the item is held by the left hand and the right hand is empty
    //(see RefreshHeldItems), but both selections are remembered.
    private readonly int[] selectedSlots = { -1, -1 };
 
    //left hand's selected slot, kept so older scripts can still read it
    public int selectedItem { get { return selectedSlots[(int)Hand.Left]; } }
 
    //used to detect the moment a number key goes down
    private readonly bool[] hotKeyWasDown = new bool[10];
 
    private static readonly Color32 hotbarDefaultColor = new Color32(255, 255, 225, 100);
    private static readonly Color32 leftHandColor = new Color32(50, 255, 225, 100);   //teal
    private static readonly Color32 rightHandColor = new Color32(255, 160, 50, 100);  //orange
 
    int inventorySize = 40; 
 
    public Sprite empty; 
 
    public GameObject CraftingUI;
    private string CraftingUITag = "CraftingUI";
 
    int hotbarSize = 8; //amount of panels in hotbar so this can be adjusted
 
    private EffectManager effectManager;
    private EffectsDict effectDict;
 
    private bool isConnectedWithUI = false;
 
    void Start(){
 
        inventory = new List<InventorySlot>();
        //Get panels as hotbar.
 
        effectManager = this.gameObject.GetComponent<EffectManager>();
        effectDict = this.gameObject.GetComponent<EffectsDict>();
 
        playerInfo = GetComponent<PlayerInfo>();
        
        //Get all children of hotbarPanel (So every hotbar button) and add them to Hotbar.
 
 
    }
 
    private void OnEnable() {
        EventManager.LocalGameObjectPlayerAddedToScene += grabUI;
    }
 
    private void OnDisable() {
        EventManager.LocalGameObjectPlayerAddedToScene -= grabUI;
    }
 
    private void grabUI(GameObject player) {
        gameUI = GameObject.FindGameObjectWithTag(gameUITag);
        hotbarPanel = GameObject.FindGameObjectWithTag(hotbarPanelTag);
        fullInventory = GameObject.FindGameObjectWithTag(fullInventoryTag);
        CraftingUI = GameObject.FindGameObjectWithTag(CraftingUITag);
        CraftingUI.SetActive(false);
 
        for (int i = 0; i < hotbarPanel.transform.childCount; i++) {
            hotbar.Add(hotbarPanel.transform.GetChild(i).gameObject);
        }
 
        //Do the same for FullInventory and fullInv
        for (int i = 0; i < fullInventory.transform.childCount; i++) {
            fullInv.Add(fullInventory.transform.GetChild(i).gameObject);
 
 
        }
 
        fullInventory.SetActive(false);
        for (int i = 0; i < inventorySize; i++) {
            InventorySlot slot = new InventorySlot();
            slot.isEmpty = true;
            inventory.Add(slot);
        }
 
 
        //Making all the quantity texts blank for both the hotbar and the fullinv.
 
        foreach (GameObject panel in hotbar) {
            if (panel.transform.childCount > 0) // Ensure it has a child
            {
                Transform textChild = panel.transform.GetChild(0); // Get the first child
                TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();
 
                tmp.text = "";
                tmp.color = new Color32(255, 255, 225, 100);
 
            }
 
        }
 
        foreach (GameObject panel in fullInv) {
            if (panel.transform.childCount > 0) // Ensure it has a child
            {
                Transform textChild = panel.transform.GetChild(0); // Get the first child
                TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();
 
                tmp.text = "";
                tmp.color = new Color32(255, 255, 225, 100);
 
 
            }
 
        }
 
        isConnectedWithUI = true;
    }
 
    void Update()
    {
        if (!isConnectedWithUI) {
            return;
        }
 
        HandleHotbarKeys();
 
        //keep what each hand is holding in sync with the inventory (covers equipping, picking up
        //into a selected slot, and a held stack running out)
        RefreshHeldItems();
 
        //Let's assume E is Inventory button, because it usually is.... //setting to P for testing as E is the interact key so it conflicts
 
        if (InputHandler.Instance.InventoryOpenTriggered)
        {
 
            if (fullInventory != null && !CraftingUI.activeSelf)
            {
 
                bool isActive = fullInventory.activeSelf;
 
                fullInventory.SetActive(!isActive);
                gameUI.SetActive(isActive);
 
 
 
                PauseTimeHandler.TogglePauseGame();
 
            }
 
        }
 
        //Weird disabling hotbar code idk
        if (InputHandler.Instance.CraftingTriggered) {
            if (CraftingUI != null && !fullInventory.activeSelf){
                 bool isActive = CraftingUI.activeSelf;
 
                //if disabling the crafting menu then make the hotbar match the first row of the crafting ui
                if (isActive)
                {
                    fillHotbar();
                }
 
                CraftingUI.SetActive(!isActive);
                gameUI.SetActive(isActive);
 
 
                PauseTimeHandler.TogglePauseGame();
 
            }
 
        }
 
        //E (Interact) is no longer used to drink potions, consumables use the holding hand's click
        HandleConsumableUse();
    }
 
    //number keys equip into the left hand, shift + number keys equip into the right hand
    private void HandleHotbarKeys()
    {
        InputHandler input = InputHandler.Instance;
 
        //keys 1 .. hotbarSize map to hotbar slots 0 .. hotbarSize-1 (0 is not a slot)
        for (int key = 1; key <= hotbarSize && key < input.IsInventoryKey.Length; key++)
        {
            bool isDown = input.IsInventoryKey[key];
 
            if (isDown && !hotKeyWasDown[key])
            {
                SelectSlot(input.IsShiftHeld ? Hand.Right : Hand.Left, key - 1);
            }
 
            hotKeyWasDown[key] = isDown;
        }
    }
 
    public int GetSelectedSlot(Hand hand)
    {
        return selectedSlots[(int)hand];
    }
 
    private void SelectSlot(Hand hand, int slot)
    {
        int oldLeft = selectedSlots[(int)Hand.Left];
        int oldRight = selectedSlots[(int)Hand.Right];
 
        selectedSlots[(int)hand] = slot;
 
        int left = selectedSlots[(int)Hand.Left];
        int right = selectedSlots[(int)Hand.Right];
 
        //clear the old highlights, then paint the new ones (left is painted last so it wins on a shared slot)
        SetSlotColor(oldLeft, hotbarDefaultColor);
        SetSlotColor(oldRight, hotbarDefaultColor);
        SetSlotColor(right, rightHandColor);
        SetSlotColor(left, leftHandColor);
    }
 
    private void SetSlotColor(int slot, Color32 color)
    {
        if (slot >= 0 && slot < hotbar.Count)
        {
            hotbar[slot].GetComponent<Image>().color = color;
        }
    }
 
    private void RefreshHeldItems()
    {
        int left = selectedSlots[(int)Hand.Left];
        int right = selectedSlots[(int)Hand.Right];
 
        ApplyHand(Hand.Left, left);
 
        //same slot in both hands means the item is only in the left hand, right hand is empty
        ApplyHand(Hand.Right, right == left ? -1 : right);
    }
 
    private void ApplyHand(Hand hand, int slotIndex)
    {
        Item item = null;
 
        if (slotIndex >= 0 && slotIndex < inventory.Count && !inventory[slotIndex].isEmpty)
        {
            item = inventory[slotIndex].item;
        }
 
        int effectiveSlot = item != null ? slotIndex : -1;
 
        if (playerInfo.GetHeldItem(hand) != item || playerInfo.GetHeldItemIndex(hand) != effectiveSlot)
        {
            playerInfo.SetHeldItem(hand, item, effectiveSlot);
        }
 
        //0 = None
        InputHandler.setSelectedContext(hand, item != null ? item.useFlag : 0);
    }
 
    private void HandleConsumableUse()
    {
        //don't drink anything while a menu is open
        if ((fullInventory != null && fullInventory.activeSelf) || (CraftingUI != null && CraftingUI.activeSelf))
        {
            return;
        }
 
        for (int h = 0; h < 2; h++)
        {
            Hand hand = (Hand)h;
 
            if (InputHandler.Instance.WasAttackPressed(hand)
                && InputHandler.GetSelectedContext(hand) == InputHandler.SelectedContext.Consumable)
            {
                UseHeldItem(hand);
            }
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
            if (inventory[i].quantity <= 1) tmp.text = "";
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
 
            if(slot.item == item && slot.quantity < item.maxStack){
 
                //Item already exists in inventory, therefore, we increment its amount.
                slot.quantity++; 
 
                 if(isInHotbar < hotbarSize){
                    
 
                   Transform textChild = hotbar[isInHotbar].transform.GetChild(0); // Get the first child
                   TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();
                   tmp.text = slot.quantity.ToString(); 
                   if(slot.quantity <= 1)
                   {
                        tmp.text = "";
                   }
 
                }
 
                  fullInv[isInHotbar].GetComponent<Image>().sprite = slot.item.icon; 
 
                   Transform textChild_inv = fullInv[isInHotbar].transform.GetChild(0); // Get the first child
                   TextMeshProUGUI tmp_inv = textChild_inv.GetComponent<TextMeshProUGUI>();
                   tmp_inv.text = slot.quantity.ToString();
                    if (slot.quantity <= 1)
                    {
                        tmp_inv.text = "";
                    }
 
 
 
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
 
            //a hand holding this slot lets go of it on the next RefreshHeldItems
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
 
    //use whatever the given hand is holding (only consumables do anything)
    public void UseHeldItem(Hand hand)
    {
        Item item = playerInfo.GetHeldItem(hand);
        int slotIndex = playerInfo.GetHeldItemIndex(hand);
 
        if (item == null || slotIndex < 0 || slotIndex >= inventory.Count)
            return;
 
        if (inventory[slotIndex].isEmpty)
            return;
 
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
                effectManager.addEffect(effectDict.getEffect("DamagePotion"), consumable.icon);
                used = true;
            }
 
            if (used)
            {
                RemoveItem(slotIndex);
                //Debug.Log($"Used {consumable.itemName}");
            }
        }
    }
 
}