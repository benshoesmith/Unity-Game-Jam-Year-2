using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This Script is used to test creating a combat scene using CombatSystemSetup.
/// </summary>
public class CombatTesting : MonoBehaviour {

    [SerializeField]
    private List<Character> t1 = null, t2 = null;

	// Use this for initialization
	void Start () {
        if (t1 == null || t2 == null)
            Debug.LogError("Character List variables have not been set for the CombatTestingScript");
        else
        {
            if (CombatSystemSetup.InitialiseMultiFight(t1, t2))
                Debug.Log("Combat System Setup initialise completed.");
            else
                Debug.LogError("Combat System Setup initialise Failed.");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
