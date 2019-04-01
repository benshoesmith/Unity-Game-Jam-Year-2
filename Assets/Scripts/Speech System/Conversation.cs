using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Conversation : MonoBehaviour {

    [SerializeField]
    private Speech beginingSpeech_ = null;

    public virtual void StartSpeech()
    {

        if(!beginingSpeech_)
        {
            Debug.LogError("beginingSpeech_ has not been set in the Inspector.");
            return;
        }

        DialogHandler dh = DialogHandler.Instance;

        if(!dh)
        {
            Debug.LogError("There is no Dialog Handler instance in the current scene/s");
            return;
        }


        dh.StartConversation(beginingSpeech_);
    }
}
