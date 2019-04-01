using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniGame : MonoBehaviour {

    public enum GameOutcome
    {
        NormalWin = 0,
        BonusWin = 1,
        FailWithAttempt = 2,
        Fail = 3
    }

    //The value changed by the outcome of the mini game. It will be used as a multiplier/to calculate the multiplier.
    protected float score_ = 0;

    private bool finished_ = false;

    private bool started_ = false;
    private float timeOfStart_ = 0.0f;
    private GameOutcome outcome_ = GameOutcome.Fail;
    private bool miniGameSceneStillNeeded_ = true;

    [SerializeField]
    private Button interactButton = null;

    public virtual void StartMiniGame()
    {
        timeOfStart_ = Time.time;
        started_ = true;

        if (interactButton)
            interactButton.Select();

        if (MiniGameStarted != null)
            MiniGameStarted.Invoke();

       
    }

    public float Score
    {
        get { return score_; }
    }

    protected void EndGame()
    {
        finished_ = true;

        if (MiniGameEnded!=null)
            MiniGameEnded.Invoke();
    }

    public virtual void MiniGameInteract()
    {

    }

    public bool Finished
    {
        get { return finished_; }
    }

    public bool Started
    {
        get { return started_; }
    }

    public float TimeOfStart
    {
        get { return timeOfStart_; }
    }

    

    public GameOutcome Outcome
    {
        get { return outcome_; }
        protected set { outcome_ = value; }
    }

    public void SetToNotNeeded()
    {
        miniGameSceneStillNeeded_ = false;
    }

    public bool IsNeeded
    {
        get
        {
            return miniGameSceneStillNeeded_;
        }
    }

    public delegate void MiniGameEventHandler();
    public event MiniGameEventHandler MiniGameStarted;
    public event MiniGameEventHandler MiniGameEnded;
}
