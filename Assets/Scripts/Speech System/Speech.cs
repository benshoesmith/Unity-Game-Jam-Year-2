using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speech : MonoBehaviour{

    [SerializeField]
    private string text_="...";//The text to show when this speech is on screen.
    [SerializeField]
    private Character speaker_ = null;//The character of the speaker.
    [SerializeField]
    private Speech nextSpeech_ = null;//The next speech after this one. (null means end of conversation.)

    public void SetupSpeech(string textForSpeech, Character speaker = null, Speech nextspeech = null)
    {
        text_ = textForSpeech;
        speaker_ = speaker;
        nextSpeech_ = nextspeech;
    }

    public string Text
    {
        get { return text_; }
    }

    public Speech NextSpeech
    {
        get { return nextSpeech_; }
        protected set { nextSpeech_ = value; }
    }

    public string SpeakerName
    {
        get
        {
            if (speaker_)
                return speaker_.Name;
            return "";
        }
    }

    public Sprite SpeakerSprite
    {
        get
        {
            if (speaker_)
              return  speaker_.SpeakerSprite;
            return null;
        }
    }
}
