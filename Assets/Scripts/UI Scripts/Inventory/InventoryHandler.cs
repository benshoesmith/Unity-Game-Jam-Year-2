using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHandler : MonoBehaviour {
    #region Variables
    public GameObject SlotPrefab;
    public Canvas ItemSlotsCanvas;

    private GameObject ToolTipItem;
    private int amountOfSlots;
    private List<GameObject> slots = new List<GameObject>();
    private int SelectedSlot;
    #endregion

    public static InventoryHandler singleton_ = null;

    private void Awake()
    {

        if (!singleton_ || singleton_ == this)
        {
            singleton_ = this;
        }
        else
        {
            Debug.LogError("Multiple instances of Invetory handler have been created. Removing this one from the scene.");
            Destroy(this);
            return;
        }

        //Get tooltip
        ToolTipItem = GameObject.FindGameObjectWithTag("Tooltip");
    }

    private void Start()
    {
        //Hide the inventory
        //gameObject.SetActive(false);

        //Hide tooltip
        ToolTipItem.gameObject.SetActive(false);
        //Number of Slots
        amountOfSlots = 30;
        //Set default selection
        SelectedSlot = -1;

        //Add the required slots
        for (int i = 0; i < amountOfSlots; i++)
        {
            GameObject slot = Instantiate(SlotPrefab);
            InventorySlot iss = slot.GetComponent<InventorySlot>();
            iss.SlotID = i;
            iss.Item = new ItemObject();
            iss.invH = this;
            slot.transform.SetParent(ItemSlotsCanvas.transform);
            slots.Add(slot);
        }
    }

    public void SlotToggled(int slotID)
    {
        if (-1 == this.SelectedSlot)
        {
            this.SelectedSlot = slotID;
        }
        else if (this.SelectedSlot == slotID)
        {
            this.SelectedSlot = -1;
        }
        else if (this.SelectedSlot != slotID)
        {
            //Get the nececary data from the object
            InventorySlot firstItem = slots[this.SelectedSlot].GetComponent<InventorySlot>();
            InventorySlot secondItem = slots[slotID].GetComponent<InventorySlot>();
            ItemObject tempFirstObject = firstItem.Item;
            System.UInt32 tempFirstAmount = firstItem.Amount;
            Vector3 tempPosFirst = firstItem.transform.GetChild(0).position;
            Vector3 tempPosSecond = secondItem.transform.GetChild(0).position;

            //Update first item
            firstItem.Item = secondItem.Item;
            firstItem.Amount = secondItem.Amount;
            //Update Second item
            secondItem.Item = tempFirstObject;
            secondItem.Amount = tempFirstAmount;

            //Fix positions
            secondItem.transform.GetChild(0).position = tempPosFirst;
            firstItem.transform.GetChild(0).position = tempPosSecond;
            firstItem.transform.GetChild(0).SetParent(secondItem.transform);
            secondItem.transform.GetChild(0).SetParent(firstItem.transform);

            //Reset the selection
            firstItem.setActiveNoInventoyCall(false);
            secondItem.setActiveNoInventoyCall(false);
            this.SelectedSlot = -1;
        }
    }

    public bool EquipmentSlotToggled(EquipmentSlot es)
    {
        if (SelectedSlot == -1)
            return false;

        //Get the data from currently selected item
        InventorySlot firstItem = slots[this.SelectedSlot].GetComponent<InventorySlot>();
        //Check if this is the correct slot and check if we got 
        if (firstItem.Item.Stats.EquipmentType != es.SlotAcceptType)
            return false;

        //Check if both are invalid item, then just dont do anything
        if (es.Item.ID == -1 && firstItem.Item.ID == -1)
        {
            firstItem.setActiveNoInventoyCall(false);
            return false;
        }

        //Create temp variables
        ItemObject tempFirstObject = firstItem.Item;
        Vector3 tempPosFirst = firstItem.transform.GetChild(0).position;
        Vector3 tempPosSecond = es.transform.GetChild(0).position;


        //Check if the selecte inventory slot is empty, if so just swap the items normally
        if (firstItem.Item.ID == -1 || firstItem.Amount == 1)
        {
            //Update first item
            firstItem.Item = es.Item;
            firstItem.Amount = 1;

            //Update equipment item
            es.Item = tempFirstObject;
            //Fix positions
            es.transform.GetChild(0).position = tempPosFirst;
            firstItem.transform.GetChild(0).position = tempPosSecond;
            firstItem.transform.GetChild(0).SetParent(es.transform);
            es.transform.GetChild(0).SetParent(firstItem.transform);
            return true;
        }

        //Check if we currently got an item equiped, if now, just reduce the quantity by 1, and add the item to the slot, if we do, just add the current equiped to the inventory
        if (es.Item.ID == -1 || AddItem(es.Item.ID))
        {
            //Update equipment item
            es.Item = firstItem.Item;
            es.UpdateStatus();

            //Remove one from the amount and update
            firstItem.Amount--;
            firstItem.UpdateStatus();
            return true;
        }

        return false;
    }

    public bool HasItem(int itemId)
    {
        ItemDatabase idb = GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemDatabase>();

        //Go trought all inventory slots
        for (int i = 0; i < slots.Count; i++)
        {
 
            InventorySlot slot = slots[i].GetComponent<InventorySlot>();

            if (slot.Item.ID == itemId)
                return true;

        }

        return false;
    }

    public bool AddItem(int itemID)
    {
        //Get the item we want to add from the database
        ItemDatabase idb = GameObject.FindGameObjectWithTag("ItemDatabase").GetComponent<ItemDatabase>();
        ItemObject newObject = idb.getItem(itemID);
        if (null != idb && null != newObject)
        {
            int freeSlotID = -1;
            //Go trought all inventory slots
            for (int i = 0; i < slots.Count; i++)
            {
                InventorySlot iss = slots[i].GetComponent<InventorySlot>();
                //Check if we have found the first empty slot
                if (-1 == freeSlotID && -1 == iss.Item.ID)
                {
                    //Set the empty slot data
                    freeSlotID = i;
                    if (!newObject.IsStackeable)
                    {
                        //Item is not stakeable so just break out and add the new item
                        break;
                    }
                }
                //Check if item is already in inventory and if it is stackeable
                if (iss.Item.ID == newObject.ID && newObject.IsStackeable)
                {
                    //Item exits and is stackeable so add one to its amount and then return, nothing more needs to be done.
                    InventorySlot slot = slots[i].GetComponent<InventorySlot>();
                    slot.Amount += 1;
                    slot.UpdateStatus();

                    if (OnInventoryAddItem != null)
                        OnInventoryAddItem.Invoke(itemID);
                    NotifcationManager.Instance.AddNotification("New Item. Press I to view.");

                    return true;
                }
            }

            //Add new item to the free slot!
            //Set the new item in the correct position
            InventorySlot slotObject = slots[freeSlotID].GetComponent<InventorySlot>();

            //Set the data to the itemData
            slotObject.Amount = 1;
            slotObject.Item = newObject;

            //Update the slot
            slotObject.UpdateStatus();

            if (OnInventoryAddItem != null)
                OnInventoryAddItem.Invoke(itemID);


            NotifcationManager.Instance.AddNotification("New Item. Press I to view.");

            return true;
        }
        return false;
    }

    //return true if removed. false if not (incase the item does not exists)
    public bool RemoveItem(int itemId)
    {
        if (!HasItem(itemId))
            return false;

        for (int i = 0; i < slots.Count; i++)
        {
            InventorySlot iss = slots[i].GetComponent<InventorySlot>();
            //Check if we have found the first empty slot
            if (itemId == iss.Item.ID)
            {
                if (!iss.Item.IsStackeable || iss.Amount == 1)
                {
                    iss.Item = new ItemObject();
                    iss.Amount = 0;
                }
                else
                {
                    iss.Amount -= 1;
                }
                iss.UpdateStatus();
                return true;

            }
          
        }


        return false;
    }

    public void AddItem(int itemID, int amount)
    {
        for (int i = 0; i < amount; i++)
            AddItem(itemID);
    }

    public void SetTooltipData(string data)
    {
        this.ToolTipItem.gameObject.transform.GetChild(0).GetComponent<Text>().text = data;
    }

    public void ShowTooltip(bool show, Transform where)
    {
        this.ToolTipItem.gameObject.transform.position = where ? where.position: this.transform.position;
        this.ToolTipItem.gameObject.SetActive(show);
    }

    public void CloseInventory()
    {
        this.gameObject.SetActive(false);
    }

    public List<InventorySlot> GetAllConsumableItemsInInventory()
    {
        List<InventorySlot> slotsWithConsumable = new List<InventorySlot>();
        foreach (GameObject slot in this.slots)
        {
            InventorySlot iSlot = slot.GetComponent<InventorySlot>();
            if (iSlot.Item.Type == eItemType.CONSUMABLE)
            {
                slotsWithConsumable.Add(iSlot);
            }
        }
        return slotsWithConsumable;
    }

    public static InventoryHandler Instance
    {
        get { return singleton_; }
    }

    public delegate void InventoryHandlerEventHandler(int itemId);
    public event InventoryHandlerEventHandler OnInventoryAddItem;
}
