using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestGoToPoint : SubQuest {

    [SerializeField]
    private GameObject placeToFind_ = null;
    [SerializeField]
    private float radius_ = 5.0f;
    private float sqrdRadius_ = 0.0f;

    [SerializeField]
    private Character characterQuestOwner_ = null;

    // Use this for initialization
    void Start () {
        doesReset_ = false;

        sqrdRadius_ = radius_ * radius_;

        if(!characterQuestOwner_)
        {
            Debug.LogError("The GoToPoint quest does not have a character set in the inspector to check the distance.");
        }

        if (!placeToFind_)
        {
            Debug.LogError("The GoToPoint quest does not have a point set in the inspector to check the distance.");
        }
    }
	
	void FixedUpdate ()
    {

        if (IsUnlocked && !HasTriggered && characterQuestOwner_ && placeToFind_)
        {
            float sqrdDistance = Mathf.Abs(Vector3.SqrMagnitude(characterQuestOwner_.transform.position - placeToFind_.transform.position));

            if(sqrdDistance <= sqrdRadius_)
            {
                CompleteQuest();
            }

        }
	}
}
