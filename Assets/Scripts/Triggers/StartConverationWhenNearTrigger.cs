using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Character))]
public class StartConveration : Trigger
{
    [SerializeField]
    private float radius_ = 5.0f;

    [SerializeField]
    private Speech startSpeech_ = null;

    public void FixedUpdate()
    {
        //todo
    }

    public void StartConversation()
    {
        DialogHandler.Instance.StartConversation(startSpeech_);
        DialogHandler.Instance.ConversationEnd += EndConversation;
        CallOnTriggered();

    }

    void EndConversation()
    {
        if (DoesReset)
            CallOnReset();
    }

}
