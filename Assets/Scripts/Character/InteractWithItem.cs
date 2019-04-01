using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractWithItem : Interactable {

    [SerializeField]
    private int itemId = 0;

    override public void Interact(GameObject interactee)
    {
        Character character = interactee.GetComponent<Character>();

        if(character && character.GetComponent<PlayerController>())
        {
            InventoryHandler ih = InventoryHandler.Instance;

            if (ih.HasItem(itemId))
            {
                base.Interact(interactee);
            }

        }

    }
}
