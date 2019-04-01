using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class QuestManagerUI : MonoBehaviour
{
    [SerializeField]
    private GameObject questUIElementPrefab = null;
    [SerializeField]
    private GameObject questUIScrollViewContent = null;

    [SerializeField]
    private Dictionary<Quest, QuestUI> questUIElements_ = new Dictionary<Quest, QuestUI>();

    public void Start()
    {
        QuestManager.Instance.OnNewQuest += Instance_OnNewQuest;
       // QuestManager.Instance.OnQuestComplete += Instance_OnQuestComplete;
    }

    private void Instance_OnQuestComplete(Quest quest)
    {
        throw new System.NotImplementedException();
    }

    private void Instance_OnNewQuest(Quest quest)
    {
        if(!questUIElementPrefab)
        {
            Debug.LogError("Prefab for QuestUI has not been set in the inspector.");
            return;
        }

        if (!questUIScrollViewContent)
        {
            Debug.LogError("Parent for QuestUI Elements has not been set in the inspector.");
            return;
        }

        GameObject questUIElement = Instantiate(questUIElementPrefab, questUIScrollViewContent.transform);

        QuestUI questUI = questUIElement.GetComponent<QuestUI>();

        if(!questUI)
        {
            Debug.LogError("Quest UI Element prefab does not have the QuestUI component attached.");
            return;
        }


        questUIElements_.Add(quest, questUI);
        questUI.InitQuestUI(quest);

    }

    public void UpdateQuestUI()
    {

    }

}
