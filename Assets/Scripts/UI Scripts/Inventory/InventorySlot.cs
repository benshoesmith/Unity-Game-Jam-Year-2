using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, ISelectHandler, IDeselectHandler  {
    public ItemObject Item;
    public int SlotID;
    public System.UInt32 Amount;
    public InventoryHandler invH;
    private Toggle toggle;

    private void Awake()
    {
        this.toggle = gameObject.GetComponent<Toggle>();
        this.toggle.onValueChanged.AddListener(setActive);
    }


    public void setActiveNoInventoyCall(bool active)
    {
        this.toggle.onValueChanged.RemoveListener(setActive);
        this.toggle.isOn = active;
        ColorBlock cb = toggle.colors;
        if (active)
        {
            cb.normalColor = cb.pressedColor;
        }
        else
        {
            cb.normalColor = Color.white;
        }
        this.toggle.colors = cb;
        this.toggle.onValueChanged.AddListener(setActive);
    }


    public void setActive(bool active)
    {
        this.setActiveNoInventoyCall(active);
        invH.SlotToggled(this.SlotID);
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
            if (Amount > 1)
            {
                transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text = Amount.ToString();
            }
            else if (Amount <= 0)
            {
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                Item = new ItemObject();
            }
            else
            { 
                transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
        } else {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
    }
}
