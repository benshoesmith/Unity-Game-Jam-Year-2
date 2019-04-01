using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine;

public class FunctionOnSpeech : Speech {

    [SerializeField]
    protected UnityEvent onSpeechUnityEvents;

    public void Start()
    {
        DialogHandler.Instance.OnSpeechSpoke += Instance_OnSpeechSpoke;
    }

    private void Instance_OnSpeechSpoke(Speech speech)
    {
       if(speech == this)
        {
            onSpeechUnityEvents.Invoke();
        }
    }
}
