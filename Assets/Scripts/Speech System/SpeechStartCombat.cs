using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechStartCombat : Speech {

    [SerializeField]
    private bool combatStarted_ = false;

    [SerializeField]
    private Character player, speaker;

    // Use this for initialization
    void Start()
    {

        if (DialogHandler.Instance)
        { 
                DialogHandler.Instance.OnSpeechSpoke += Instance_OnSpeechSpoke;
        }
        else
        {
            Debug.LogError("No dialog system in the scene.");
        }


    }

    private void Instance_OnSpeechSpoke(Speech speech)
    {
        if (speech == this && !combatStarted_)
        {
            CombatSystemSetup.InitialiseFight(player, speaker);
        }
    }
}
