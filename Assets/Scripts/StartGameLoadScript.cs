using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGameLoadScript : MonoBehaviour
{
    public Image LoadingBar;
    public Sprite LoadingBarColor1;
    public Sprite LoadingBarColor2;
    public Sprite LoadingBarColor3;
    public Text LoadingText;
    private float m_barWidth;

	private void Start ()
    {
        m_barWidth = LoadingBar.GetComponent<RectTransform>().sizeDelta.x;
        StartCoroutine(IELoadScene());
	}

    private IEnumerator IELoadScene()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync("TutorialMainMap");
        async.allowSceneActivation = true;

        while (!async.isDone)
        {
            UpdateUI(async.progress);
            yield return null;
        }

        yield return async;
    }

    private void UpdateUI(float currentMapProgress)
    {
        LoadingBar.GetComponent<RectTransform>().sizeDelta = new Vector2(m_barWidth * currentMapProgress, LoadingBar.GetComponent<RectTransform>().sizeDelta.y);
        LoadingText.text = "Loading the game ... " + Mathf.RoundToInt(currentMapProgress * 100.0f) + "%";
        if (currentMapProgress >= 0.75f)
        {
            LoadingBar.sprite = LoadingBarColor3;
        } else if (currentMapProgress >= 0.35f)
        {
            LoadingBar.sprite = LoadingBarColor2;
        } else
        {
            LoadingBar.sprite = LoadingBarColor1;
        }
    }
}
