using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class StartCombatWhenClose : MonoBehaviour {

    [SerializeField]
    private float radius_ = 5.0f;

    bool inFight_ = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (inFight_)
            return;

        Collider2D[] collidersNear = Physics2D.OverlapCircleAll(transform.position, radius_);

        foreach(Collider2D collider in collidersNear)
        {
            Character c = collider.GetComponent<Character>();

            if(c && c.gameObject != gameObject)
            {
                Fight(c);
                return;
            }
        }

	}

    void Fight(Character c)
    {
        inFight_ = true;


        CombatSystemSetup.InitialiseFight(c, GetComponent<Character>());
    }
}
