using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestKillEnemys : SubQuest {

    [SerializeField]
    private Attack.Type typeToKill_ = Attack.Type.Normal;
    [SerializeField]
    private int amountToKill_ = 3;

    private int amountKilled_ = 0;

    bool waitingOnCombatToEnd_ = false;

	// Use this for initialization
	void Start () {
        doesReset_ = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (!waitingOnCombatToEnd_ && IsUnlocked && !IsCompleted && CombatSystem.Instance)
        {
            CombatSystem.Instance.CombatEnd += Instance_CombatEnd;
            waitingOnCombatToEnd_ = true;
        }
	}

    private void Instance_CombatEnd()
    {
        waitingOnCombatToEnd_ = false;

        CombatSystem.Instance.CombatEnd -= Instance_CombatEnd;

        foreach(Character character in CombatSystem.Instance.Team2)
        {
            if(character.IsDead && character.Type == typeToKill_)
            {
                amountKilled_++;
            }

            if(amountKilled_ >= amountToKill_)
            {
                CompleteQuest();
            }
        }
    }
}
