using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnimation : MonoBehaviour {

    [SerializeField]
    private Animation clip = null;

	public void Play()
    {
        bool played = false;

        if (clip)
            played = clip.Play();

        Debug.Log(played);
    }
}
