using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest : SubQuest
{
    [SerializeField]
    private bool completeQuestsInOrder_ = true;

    [SerializeField]
    private List<SubQuest> subQuests_ = new List<SubQuest>();


    [SerializeField]
    private int xpWhenCompleted = 100;

	// Use this for initialization
	void Start () {
		
	}

	void FixedUpdate ()
    {
        if (IsUnlocked)
            CheckSubQuests();
	}

    public override void UnlockQuest(bool notify = true)
    {
        questLocked_ = false;

        if (NotifcationManager.Instance && notify)
            NotifcationManager.Instance.AddNotification("New Quest!");

        if (subQuests_.Count > 0)
        {
            if (completeQuestsInOrder_)
            {
                subQuests_[0].UnlockQuest(false);
            }
            else
            {
                foreach (SubQuest sub in subQuests_)
                {
                    
                    sub.UnlockQuest();
                }
            }


        }
    }

    private void CheckSubQuests()
    {

        if (IsCompleted)
            return;

        for (int i = 0; i < subQuests_.Count; i++)
        {
            SubQuest quest = subQuests_[i];

            if (quest && quest.IsCompleted)
            {
                if (i + 1 < subQuests_.Count && !subQuests_[i+1].IsUnlocked)
                {
                    subQuests_[i+1].UnlockQuest(true);

                    return;
                }


                continue;
            }
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player)
        {
            Character c = player.GetComponent<Character>();
            if(c)
            {
                c.Xp = c.Xp + xpWhenCompleted;
            }
        }

        questCompleted_ = true;

        CallOnTriggered();
        questCompleted_ = true;

        if (NotifcationManager.Instance)
            NotifcationManager.Instance.AddNotification("Quest Complete!");

        if (OnQuestCompleted != null)
            OnQuestCompleted.Invoke(this);
    }

    public bool CompleteSubquestInOrder
    {
        get { return completeQuestsInOrder_; }
    }

    public List<SubQuest> SubQuests
    {
        get { return subQuests_; }
    }


    public delegate void QuestEventSystem(Quest quest);
    public event QuestEventSystem OnQuestCompleted;

}
