using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MiniGameHandler : MonoBehaviour {

    [SerializeField]
    private List<string> minigameSceneNames_ = new List<string>();

    private Scene currentScene_;
    private MiniGame currentMinigame_ = null;

    private bool startedMiniGame_ = false;
    private string currentLoadedSceneName_ = "";

    public bool LoadMiniGame(int index)
    {
        //if there are no minigame scenes then return false.
        if (minigameSceneNames_.Count < 1)
        {
            Debug.LogError("No minigame names found. Set the names in the inspector.");
            return false;
        }

        //if index provieded is too large then set to use random minigame.
        if (index >= minigameSceneNames_.Count)
            index = -1;

        //get random minigame.
        if (index < 0)
            index = Random.Range(0, minigameSceneNames_.Count);

        Debug.Log("Loading minigame '" + minigameSceneNames_[index] + "'");

        startedMiniGame_ = true;
        SceneManager.LoadScene(minigameSceneNames_[index], LoadSceneMode.Additive);
        currentLoadedSceneName_ = minigameSceneNames_[index];

        return true;
    }

    public void Update()
    {
        if (startedMiniGame_ && !currentMinigame_)
        {
            GameObject go = GameObject.FindGameObjectWithTag("MiniGame");
            if (go)
                currentMinigame_ = go.GetComponent<MiniGame>();
        }

        if (currentMinigame_)
        {
            if (!currentMinigame_.Started)
                currentMinigame_.StartMiniGame();
            else if (currentMinigame_.Finished && !currentMinigame_.IsNeeded)
                CloseMiniGame();
        }
    }

    private void CloseMiniGame()
    {
        if (!currentMinigame_)
            return;

        SceneManager.UnloadSceneAsync(currentLoadedSceneName_);
        currentLoadedSceneName_ = "";
    }

    public MiniGame CurrentMiniGame
    {
        get { return currentMinigame_; }
        protected set { currentMinigame_ = value;}
    }

}
