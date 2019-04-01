using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubQuest : Trigger {

    [SerializeField]
    protected string namePreview = "";
    [SerializeField]
    protected string description = "";

    protected bool questLocked_ = true;
    protected bool questCompleted_ = false;

    [SerializeField]
    private GameObject minimapIcon = null;

	// Use this for initialization
	void Start () {
		
	}

    private void Update()
    {
        if (minimapIcon)
        {
            if (IsUnlocked)
                minimapIcon.SetActive(true);
            else
                minimapIcon.SetActive(false);
        }

    }

    void FixedUpdate ()
    {

        if (!IsCompleted || doesReset_)
            SetQuestCompleteState(CheckQuestConditionsComplete());
	}

    protected virtual bool CheckQuestConditionsComplete()
    {
        return false;
    }

    private void SetQuestCompleteState(bool  completed)
    {
        if (completed == questCompleted_)
            return;

        if (completed)
            CompleteQuest();
        else if (DoesReset)
            ResetQuest();

    }

    protected virtual void CompleteQuest()
    {
        if (!IsUnlocked)
            return;

        CallOnTriggered();
        questCompleted_ = true;

        if (NotifcationManager.Instance)
            NotifcationManager.Instance.AddNotification("SubQuest Complete!");

    }

    protected virtual void ResetQuest()
    {
        CallOnReset();
    }

    public virtual void UnlockQuest(bool notify = true)
    {
        if (IsUnlocked)
            return;

        if (NotifcationManager.Instance && notify)
            NotifcationManager.Instance.AddNotification("New Sub Quest!");

        if (OnSubQuestUnlocked != null)
            OnSubQuestUnlocked.Invoke(this);

        questLocked_ = false;
    }

    public void LockQuest()
    {
        if (IsCompleted)
            return;

        questLocked_ = true;
    }

    public bool IsUnlocked
    {
        get { return !questLocked_; }
    }

    public bool IsCompleted
    {
        get { return questCompleted_; }
    }

    public string Description
    {
        get { return description; }
    }

    public string Name
    {
        get { return namePreview; }
    }

    public delegate void SubQuestEventManager(SubQuest subQuest);
    public event SubQuestEventManager OnSubQuestUnlocked;

}
