using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestFindItem : SubQuest
{

    [SerializeField]
    private int itemIdToFind_ = 1;

    [SerializeField]
    private GameObject itemToFind_ = null;

    public void Awake()
    {
        doesReset_ = false;

        if(itemToFind_)
        {
            ItemIngame iig = itemToFind_.GetComponent<ItemIngame>();

            if(iig)
            {
                itemIdToFind_ = iig.ItemId;
            }

        }
      
    }

    public void Start()
    {
        InventoryHandler.Instance.OnInventoryAddItem += Instance_OnInventoryAddItem;
    }

    public override void UnlockQuest(bool notify = true)
    {
        base.UnlockQuest(notify);

        if(itemToFind_)
            itemToFind_.SetActive(true);

    }

    private void Instance_OnInventoryAddItem(int itemId)
    {
        if(itemId == itemIdToFind_ && IsUnlocked && !IsCompleted)
        {
            CompleteQuest();
        }
    }


}
