using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSpeakToCharacter : SubQuest
{

    [SerializeField]
    private Speech speechToCompleteQuest = null;

    private bool speechSpoke_ = false;

    // Use this for initialization
    void Start()
    {

        doesReset_ = false;

        if (!speechToCompleteQuest)
        {
            Debug.LogError("Speech has not been asigned in the inspector.");
            Destroy(this);
            return;
        }
        DialogHandler.Instance.OnSpeechSpoke += Instance_OnSpeechSpoke;

        //if there is no custom name
        if (name.Length == 0)
            name = "Go speak to " + speechToCompleteQuest.name;

        //if there is no custom description
        if (description.Length == 0)
            name = "Find and Speak to " + speechToCompleteQuest.name;

    }

    private void Instance_OnSpeechSpoke(Speech speech)
    {
        if(speech == speechToCompleteQuest && IsUnlocked)
        {
            speechSpoke_ = true;
            DialogHandler.Instance.OnSpeechSpoke -= Instance_OnSpeechSpoke;
        }
    }

    override protected bool CheckQuestConditionsComplete()
    {
        return speechSpoke_;
    }
}
