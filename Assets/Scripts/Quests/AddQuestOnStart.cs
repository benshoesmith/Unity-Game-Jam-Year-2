using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddQuestOnStart : MonoBehaviour {

    [SerializeField]
    private Quest questToAdd_ = null;

	// Use this for initialization
	void Start () {
		if(!QuestManager.Instance)
        {
            Debug.LogError("Quest can not be added as there is not quest manager in the scene.");
            return;
        }

        if(!questToAdd_)
        {
            Debug.LogError("Quest to be added has not been set in the inspector.");
            return;
        }

        QuestManager.Instance.ActivateQuest(questToAdd_);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
