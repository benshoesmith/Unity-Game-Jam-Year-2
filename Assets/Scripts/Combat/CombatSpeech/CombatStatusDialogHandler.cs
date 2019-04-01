using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatStatusDialogHandler : MonoBehaviour {
    private static CombatStatusDialogHandler singleton_ = null;
    public static CombatStatusDialogHandler Instance
    {
        get { return singleton_; }
    }

    [SerializeField]
    private Text textUI_ = null;
    [SerializeField]
    private Animator nextAnimator_ = null;

    private CombatStatusSpeech nextSpeech_ = null;

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
    void Start()
    {
        if (!textUI_)
            Debug.LogError("textUI has not been set in the inspector.");
    }

    public bool StartConversation(CombatStatusSpeech speech)
    {
        Debug.Log("New Combat speech conv");

        //Set the DialogUI to show the first speech.
        SetDialogUI(speech);
        //Set the next speech.
        nextSpeech_ = speech.NextSpeech;

        return true;
    }

    public bool InCombatStatusPrintinr()
    {
        return nextSpeech_ == null;
    }

    public void Update()
    {
        if (fullText.Length != 0 && textUI_ != null)
        {
            float timeSinceStartOfSpeech = Time.time - speechStartTime_;
            float totalCharactersToShow = timeSinceStartOfSpeech * textSpeed_;

            if (totalCharactersToShow >= fullText.Length)
            {
                textUI_.text = fullText;
                nextAnimator_.gameObject.SetActive(true);
                nextAnimator_.SetBool("SpeechFinished", true);
            }
            else
                textUI_.text = fullText.Substring(0, ((int)totalCharactersToShow));
        }
    }

    public void SetButtonInteractions(bool canInteract)
    {
        gameObject.transform.GetChild(0).GetComponent<Button>().interactable = canInteract;
    }

    //Set the DialogUI to have the correct text, images, names etc.
    public void SetDialogUI(CombatStatusSpeech speech)
    {

        if (textUI_)
            textUI_.transform.parent.parent.gameObject.SetActive(true);

        nextAnimator_.SetBool("SpeechFinished", false);
        nextAnimator_.gameObject.SetActive(false);

        gameObject.transform.GetChild(0).GetComponent<Button>().Select();

        ClearDialogUI();
        fullText = speech.Text;

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
        nextAnimator_.SetBool("SpeechFinished", false);
        if (textUI_)
            textUI_.transform.parent.parent.gameObject.SetActive(false);

        if (ConversationEnd != null)
            ConversationEnd.Invoke();
    }

    public void ClearDialogUI()
    {
        if (textUI_)
            textUI_.text = "";

        speechStartTime_ = Time.time;
        fullText = "";
    }


    public delegate void CombatStatusDialogHandlerEventHandler();
    public event CombatStatusDialogHandlerEventHandler ConversationEnd;

    public delegate void CombatStatusSpeechSpokenEventHandler(CombatStatusSpeech speech);
    public event CombatStatusSpeechSpokenEventHandler OnSpeechSpoke;

}
