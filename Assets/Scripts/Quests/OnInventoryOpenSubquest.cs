using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnInventoryOpenSubquest : SubQuest {

    // Use this for initialization
    void Start() {
        UIHandler uih = GameObject.FindGameObjectWithTag("MainUICanvas").GetComponent<UIHandler>();

        if (uih)
        {
            uih.OnInventoryOpen += Uih_OnInventoryOpen;
        }
	}

    private void Uih_OnInventoryOpen()
    {
        if (!IsCompleted && IsUnlocked)
            CompleteQuest();
    }
}
