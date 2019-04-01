using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItemOnInteract : Interactable {


    public override void Interact(GameObject interacter)
    {
        base.Interact(interacter);

        Character characterWhoInteracted = interacter.GetComponent<Character>();

        if(characterWhoInteracted)
        {
            ///TODO: Add item to inventory.
        }
    }

}
