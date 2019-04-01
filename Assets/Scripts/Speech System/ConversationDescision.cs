using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationDescision : Conversation {

    [SerializeField]
    private SubQuest quest = null;

    [SerializeField]
    private Conversation beforeUnlockedConversation = null;
    [SerializeField]
    private Conversation afterUnlockedConversation = null;
    [SerializeField]
    private Conversation afterCompletedConversation = null;


    override public void StartSpeech()
    {

        if (!quest)
        {
            Debug.LogError("There is no quest set in the inspector for the Conversation Descision.");
            return;
        }

        DialogHandler dh = DialogHandler.Instance;

        if (!dh)
        {
            Debug.LogError("There is no Dialog Handler instance in the current scene/s");
            return;
        }

        if (!quest.IsUnlocked)
        {
            beforeUnlockedConversation.StartSpeech();
        }else if(!quest.IsCompleted)
        {
            afterUnlockedConversation.StartSpeech();
        }
        else
        {
            afterCompletedConversation.StartSpeech();
        }
       
    }
}
