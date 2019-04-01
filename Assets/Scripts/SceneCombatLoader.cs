using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneCombatLoader : MonoBehaviour
{

    private string name;
    public void Awake()
    {
        name = SceneManager.GetActiveScene().name;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }


    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "CombatLevelUI")
        {
            SceneManager.LoadScene("CombatLevelUI", LoadSceneMode.Additive);
            return;
        }
        

        SceneManager.MergeScenes(SceneManager.GetSceneByName("CombatLevelUI"), SceneManager.GetSceneByName(name));

        CombatSystem.Instance.transform.SetAsFirstSibling();

        SceneManager.sceneLoaded -= OnSceneLoaded;

        CombatSystem.Instance.gameObject.SetActive(false);

        Destroy(gameObject);
    }
}
