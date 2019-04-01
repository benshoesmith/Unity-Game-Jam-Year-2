using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestReturnItem : SubQuest {

    [SerializeField]
    private Speech toRemoveItemSpeech_;

    [SerializeField]
    private int itemIdToRemove = 0;

    [SerializeField]
    private int amountToHandIn = 10;

    private int amountHandedIn = 0;

	// Use this for initialization
	void Start () {
        DialogHandler.Instance.OnSpeechSpoke += Instance_OnSpeechSpoke;
	}

    private void Instance_OnSpeechSpoke(Speech speech)
    {
        if(!IsCompleted && speech == toRemoveItemSpeech_)
        {
            while (true)
            {
                if(amountToHandIn == amountHandedIn)
                {
                    CompleteQuest();
                    return;
                }

                //if item was removed then add to the counter.
                if (InventoryHandler.Instance.RemoveItem(itemIdToRemove))
                {
                    amountHandedIn++;
                }
                //else return as the player needs more of that item.
                else
                {
                    NotifcationManager.Instance.AddNotification("You do not have the item/s required");
                    return;
                }
            }

               
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
