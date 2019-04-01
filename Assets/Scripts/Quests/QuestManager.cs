using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    private static QuestManager singleton_ = null;

    [SerializeField]
    private List<Quest> quests_ = new List<Quest>();

    private List<Quest> completedQuests_ = new List<Quest>();

    private void Awake()
    {
        if (!singleton_ || singleton_ == this)
        {
            singleton_ = this;
        }
        else
        {
            Destroy(this);
            Debug.LogError("More than one Quest Manager was created. This is not supported. Remove duplicate from scene.");
        }
    }

    public void ActivateQuest(Quest newQuest)
    {
        if (!newQuest)
            return;

        foreach(Quest quest in quests_)
        {
            if (quest == newQuest)
            {
                Debug.LogWarning("Quest has already been activated.");
                return;
            }
        }

        foreach (Quest quest in completedQuests_)
        {
            if (quest == newQuest)
            {
                Debug.LogWarning("Quest has already been activated and completed.");
                return;
            }
        }

        newQuest.UnlockQuest();
        quests_.Add(newQuest);

        if (OnNewQuest != null)
            OnNewQuest.Invoke(newQuest);

        newQuest.OnQuestCompleted += OnQuestCompleted;

    }

    private void OnQuestCompleted(Quest quest)
    {
        quests_.Remove(quest);
        completedQuests_.Add(quest);

        if (OnQuestComplete != null)
            OnQuestComplete.Invoke(quest);


    }

    public static QuestManager Instance
    {
        get { return singleton_; }
    }

    public List<Quest> ActiveQuests
    {
        get { return quests_; }
    }

    public delegate void QuestManagerEventManager(Quest quest);
    public event QuestManagerEventManager OnNewQuest;
    public event QuestManagerEventManager OnQuestComplete;

}
