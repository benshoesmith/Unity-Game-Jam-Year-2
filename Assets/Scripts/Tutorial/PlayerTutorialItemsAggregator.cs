using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTutorialItemsAggregator : MonoBehaviour {

	// Use this for initialization
	void Start () {
        InventoryHandler.Instance.AddItem(33, 3);
        InventoryHandler.Instance.AddItem(69);
        InventoryHandler.Instance.AddItem(72);
        InventoryHandler.Instance.AddItem(15);
    }
}
