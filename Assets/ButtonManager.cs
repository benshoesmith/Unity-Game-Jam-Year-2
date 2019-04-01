using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ButtonManager : MonoBehaviour {
    public void NewGameBtn()
    {
        SceneManager.LoadScene(2);
    }
	public void ExitGameBtn()
    {
        Application.Quit();
        Debug.Log("The Application Has been Closed");
    }
}
