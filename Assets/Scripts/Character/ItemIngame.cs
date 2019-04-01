using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemIngame : Interactable {

    [SerializeField]
    private int itemId_ = 0;

    override public void Interact(GameObject go)
    {
        base.Interact(go);

        if (!go)
            return;

        Character character = go.GetComponent<Character>();
        InventoryHandler ih = InventoryHandler.Instance;

        if (character && ih)
        {
            Debug.Log("adding item");
            ih.AddItem(itemId_);
            Destroy(gameObject);
        }

    }

    public int ItemId
    {
        get { return itemId_; }
    }

}
