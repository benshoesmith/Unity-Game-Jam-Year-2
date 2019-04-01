using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechGiveQuest : Speech {

    [SerializeField]
    private Quest questToGive_ = null;

    private bool questGiven_ = false;

	// Use this for initialization
	void Start () {
        if (questToGive_)
        {
            if(DialogHandler.Instance)
                DialogHandler.Instance.OnSpeechSpoke += Instance_OnSpeechSpoke;
        }
        else
        {
            Debug.LogError("Quest to be given has not been assigned in the inspector.");
        }

        
	}

    private void Instance_OnSpeechSpoke(Speech speech)
    {
        if (speech == this && !questGiven_)
        {
            questGiven_ = true;
            QuestManager.Instance.ActivateQuest(questToGive_);
        }
    }
}
