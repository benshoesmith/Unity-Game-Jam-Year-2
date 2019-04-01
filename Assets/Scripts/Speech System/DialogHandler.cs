using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogHandler : MonoBehaviour {

    private static DialogHandler singleton_ = null;
    public static DialogHandler Instance
    {
        get { return singleton_; }
    }

    [SerializeField]
    private Text textUI_ = null;
    [SerializeField]
    private Text nameUI_ = null;
    [SerializeField]
    private Image speakerImage_ = null;

    private Speech nextSpeech_ = null;

    [Header("Text Settings")]
    [SerializeField]
    private float textSpeed_ = 1.0f;   //how many characters to write per second.
    private float speechStartTime_ = 0.0f;//time when the speech is first displayed.
    private string fullText = "";//the full text to show.

    private void Awake()
    {
        if (singleton_ && singleton_ != this)
            Destroy(this);
        else
            singleton_ = this;
    }

    // Use this for initialization
    void Start ()
    {
		if(!textUI_)
            Debug.LogError("textUI has not been set in the inspector.");

        if (!nameUI_)
            Debug.LogError("nameUI has not been set in the inspector.");

        if (!speakerImage_)
            Debug.LogError("speakerImage_ has not been set in the inspector.");
    }
	
	public bool StartConversation(Speech speech)
    {
        Debug.Log("New conv");

        //Already in a dialog then do not start another.
        if (GlobalGame.Instance.CurrentGameState == GlobalGame.GameState.InDialog)
            return false;

        //Set GameState to InDialog.
        GlobalGame.Instance.CurrentGameState = GlobalGame.GameState.InDialog;

        //Set the DialogUI to show the first speech.
        SetDialogUI(speech);
        //Set the next speech.
        nextSpeech_ = speech.NextSpeech;

        return true;
    }

    public void Update()
    {
        if(fullText.Length != 0 && textUI_ != null)
        {
            float timeSinceStartOfSpeech = Time.time - speechStartTime_;
            float totalCharactersToShow = timeSinceStartOfSpeech * textSpeed_;

            if (totalCharactersToShow >= fullText.Length)
                textUI_.text = fullText;
            else
                textUI_.text = fullText.Substring(0, ((int)totalCharactersToShow));
        }
    }

    //Set the DialogUI to have the correct text, images, names etc.
    public void SetDialogUI(Speech speech)
    {
        ClearDialogUI();
        fullText = speech.Text;
        if (nameUI_)
            nameUI_.text = speech.SpeakerName;
        if (speakerImage_)
            speakerImage_.sprite = speech.SpeakerSprite;

        if (OnSpeechSpoke != null)
            OnSpeechSpoke.Invoke(speech);

    }

    public void NextSpeech()
    {
        Debug.Log("Next speech");

        //If there is no more speech then end coversation (clear ui and close it)
        if (nextSpeech_ == null)
        {
            EndConversation();
            return;
        }

        SetDialogUI(nextSpeech_);
        nextSpeech_ = nextSpeech_.NextSpeech;
    }

    public void EndConversation()
    {
        ClearDialogUI();

        if (ConversationEnd != null)
            ConversationEnd.Invoke();

        GlobalGame.Instance.CurrentGameState = GlobalGame.GameState.Normal;
    }

    public void ClearDialogUI()
    {
        if (textUI_)
            textUI_.text = "";

        if (nameUI_)
            nameUI_.text = "";

        if (speakerImage_)
            speakerImage_.sprite = null;

        speechStartTime_ = Time.time;
        fullText = "";
    }


    public delegate void DialogHandlerEventHandler();
    public event DialogHandlerEventHandler ConversationEnd;

    public delegate void SpeechSpokenEventHandler(Speech speech);
    public event SpeechSpokenEventHandler OnSpeechSpoke;


}
