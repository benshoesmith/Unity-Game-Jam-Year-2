using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatStatusSpeech {

    [SerializeField]
    private string text_ = "...";//The text to show when this speech is on screen.
    [SerializeField]
    private CombatStatusSpeech nextSpeech_ = null;//The next speech after this one. (null means end of conversation.)

    public CombatStatusSpeech(string textForSpeech, CombatStatusSpeech nextspeech = null)
    {
        text_ = textForSpeech;
        nextSpeech_ = nextspeech;
    }

    public string Text
    {
        get { return text_; }
    }

    public CombatStatusSpeech NextSpeech
    {
        get { return nextSpeech_; }
        protected set { nextSpeech_ = value; }
    }
}
