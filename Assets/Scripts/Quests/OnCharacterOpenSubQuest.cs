using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCharacterOpenSubQuest : SubQuest {

    // Use this for initialization
    void Start() {
        UIHandler uih = GameObject.FindGameObjectWithTag("MainUICanvas").GetComponent<UIHandler>();

        if (uih)
        {
            uih.OnCharacterOpen += Uih_OnCharacterOpen;
        }
	}

    private void Uih_OnCharacterOpen()
    {
        if (!IsCompleted && IsUnlocked)
            CompleteQuest();
    }
}
