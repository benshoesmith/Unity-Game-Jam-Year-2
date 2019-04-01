using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlot : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public eEquipmentType SlotAcceptType;
    public InventoryHandler invH;
    public ItemObject Item;
    private Toggle toggle;

    private void Awake()
    {
        this.toggle = gameObject.GetComponent<Toggle>();
        this.toggle.onValueChanged.AddListener(setActive);
    }

    public void setActive(bool active)
    {
        ItemObject tempObject = Item;
        if (invH.EquipmentSlotToggled(this))
        {
            Character p = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();

            if (tempObject.ID != -1)
                p.RemoveEquippedItem(tempObject);

            if (Item.ID != -1)
                p.AddEquipedItem(Item);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        //Show GUI
        if (-1 != this.Item.ID)
        {
            invH.SetTooltipData(Item.BuildTooltip());
            invH.ShowTooltip(true, this.gameObject.transform);
        }
    }

    public void OnDeselect(BaseEventData eventData)
    {
        //Hide the GUI
        invH.ShowTooltip(false, null);
    }

    public void UpdateStatus()
    {
        if (Item.ID != 0)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(0).GetComponent<Image>().sprite = Item.Sprite;
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
    }
}
