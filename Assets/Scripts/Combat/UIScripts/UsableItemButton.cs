using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UsableItemButton : MonoBehaviour {

    public InventorySlot ItemtoUse;
    public CombatUIHandler cuiHandler;

    private void Start()
    {
        UpdateButton();
        GetComponent<Toggle>().onValueChanged.AddListener(ChangedValue);
    }

    public void ChangedValue(bool status)
    {
        if(status)
            cuiHandler.EnableTargetingForItem(ItemtoUse.Item.TargetingLimit, ExecuteItem);
    }
   
    public void ExecuteItem(Character self, Character target, Character[] enemies, Character[] allies)
    {
        ItemtoUse.Item.UseItem(self, target, enemies, allies);
        ItemtoUse.Amount--;
        ItemtoUse.UpdateStatus();
        UpdateButton();
    }


    private void UpdateButton()
    {
        transform.GetChild(0).GetComponent<Text>().text = ItemtoUse.Item.Title + ((ItemtoUse.Amount > 1) ? " x " + ItemtoUse.Amount : "");
        if (ItemtoUse.Amount <= 0)
        {
            Destroy(gameObject);
        }
    }
}
