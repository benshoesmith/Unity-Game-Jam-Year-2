﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BtnManager : MonoBehaviour {
    public void PlayGameBtn( string newGameLevel)
    {
        SceneManager.LoadScene(newGameLevel);
    }
	public void ExitGameBtn()
    {
        Application.Quit();
    }
}
