using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSkillTreeOpenSubquest : SubQuest {

    // Use this for initialization
    void Start() {
        UIHandler uih = GameObject.FindGameObjectWithTag("MainUICanvas").GetComponent<UIHandler>();

        if (uih)
        {
            uih.OnSkillTreeOpen += Uih_OnSkillOpen;
        }
	}

    private void Uih_OnSkillOpen()
    {
        if (!IsCompleted && IsUnlocked)
            CompleteQuest();
    }
}
