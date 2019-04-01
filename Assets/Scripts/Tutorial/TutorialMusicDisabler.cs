using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMusicDisabler : MonoBehaviour {

	// Use this for initialization
	void Start () {
        CombatSystem.Instance.CombatStart += StopMusic;
	}

    private void StopMusic()
    {
        GetComponent<AudioSource>().Stop();
    }
}
