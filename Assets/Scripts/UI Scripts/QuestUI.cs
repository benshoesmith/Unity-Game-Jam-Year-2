using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour {



    [SerializeField]
    private Quest quest_ = null;

    [SerializeField]
    private Text questName_ = null;

    [SerializeField]
    private GameObject completedIcon_ = null;

    [SerializeField]
    private Dictionary<SubQuest, Text> subquestTexts = new Dictionary<SubQuest, Text>();

    [SerializeField]
    private GameObject subQuestContent = null;

    [SerializeField]
    private GameObject subQuestPrefab = null;


    public void InitQuestUI(Quest quest)
    {
        quest_ = quest;

        if (!quest_)
        {
            Debug.LogError("Could not create QuestUIElement because quest was not set the inspector.");
            return;
        }

        if (!questName_)
        {
            Debug.LogError("Could not create QuestUIElement because quest name text was not set in the inspector.");
            return;
        }

        questName_.text = quest_.Name;



        foreach (SubQuest subquest in quest.SubQuests)
        {
            if(subquest.IsUnlocked)
            {
                NewSubQuest(subquest);
                continue;
            }

            subquest.OnSubQuestUnlocked += Subquest_OnSubQuestUnlocked;
        }

    }

    private void Subquest_OnSubQuestUnlocked(SubQuest subQuest)
    {
        NewSubQuest(subQuest);
    }

    public void Update()
    {

        foreach (KeyValuePair<SubQuest, Text> subQuest in subquestTexts)
        {
            if (subQuest.Key.IsCompleted)
                subQuest.Value.color = Color.grey;
        }

        if(quest_.IsCompleted)
        {
            subquestTexts.Clear();
            Destroy(subQuestContent);
            completedIcon_.SetActive(true);
        }

    }

    public Text NewSubQuest(SubQuest subQuest)
    {
        GameObject subQuestGO = Instantiate(subQuestPrefab, subQuestContent.transform);
        Text text = subQuestGO.GetComponent<Text>();

        if (text)
        {
            text.text = subQuest.Name;
        }
        else
        {
            Debug.LogError("SubQuest UI Element Prefab does not have a text component.");
        }

        subquestTexts.Add(subQuest, text);

        return text;
    }

}
