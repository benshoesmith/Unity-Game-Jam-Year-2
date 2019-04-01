using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechGiveItem : Speech {


    [SerializeField]
    private int itemIdToGive_ = 0;

    [SerializeField]
    private int amount = 1;

    private bool itemGiven_ = false;

    // Use this for initialization
    void Start()
    {

        if (DialogHandler.Instance)
            DialogHandler.Instance.OnSpeechSpoke += Instance_OnSpeechSpoke;

    }

    private void Instance_OnSpeechSpoke(Speech speech)
    {
        if (speech == this && !itemGiven_)
        {
            itemGiven_ = true;

            InventoryHandler ih = InventoryHandler.Instance;

            if(ih)
            {
                ih.AddItem(itemIdToGive_, amount);
            }


        }
    }
}
