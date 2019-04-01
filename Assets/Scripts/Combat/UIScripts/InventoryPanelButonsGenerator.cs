using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryPanelButonsGenerator : MonoBehaviour {

    public GameObject SpellButtonPrefab;
    public CombatUIHandler cuihandler;


    private void Awake()
    {
        CombatSystem.Instance.CombatStart += SetUpItemsButtons;
        CombatSystem.Instance.CombatEnd += RemoveAllItemsButtons;
    }

    private void SetUpItemsButtons()
    {
        List<InventorySlot> inventorySlotsScripts = InventoryHandler.Instance.GetAllConsumableItemsInInventory();

        foreach (InventorySlot invSlot in inventorySlotsScripts)
        {
            GameObject entityFrame = Instantiate(SpellButtonPrefab, gameObject.transform);
            entityFrame.GetComponent<UsableItemButton>().ItemtoUse = invSlot;
            entityFrame.GetComponent<UsableItemButton>().cuiHandler = cuihandler;
            entityFrame.GetComponent<Toggle>().interactable = true;
        }
    }

    private void RemoveAllItemsButtons()
    {
        foreach (GameObject o in transform)
        {
            Destroy(o);
        }
    }
}
