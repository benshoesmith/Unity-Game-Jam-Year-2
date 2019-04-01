using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class CombatSystemSetup
{

   /* [SerializeField]
    static private string combatSceneName = "CombatLevelUI";
    [SerializeField]
    static private string combatTutorialSceneName = "CombatLevel-Tutorial";

    private static IEnumerator IELoadScene(string sceneName, List<Character> team1, List<Character> team2)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        async.allowSceneActivation = false;

        //Wait until level is loaded
        while (async.progress < 0.89)
        {
            yield return 1;
        }

        Camera camera = Camera.main;

        //Then wait until level has initialised.
        while (!async.isDone)
        {
            // Check if the load has finished
            if (async.progress >= 0.9f)
            {
                GlobalGame.Instance.CurrentGameState = GlobalGame.GameState.InTransition;
                camera.GetComponent<TransitionFade>().StartFade(1.5f);
                yield return new WaitForSeconds(camera.GetComponent<TransitionFade>().GetTimeRemaining());
                async.allowSceneActivation = true;
                camera.gameObject.SetActive(false);
            }
            yield return null;
        }

        CombatSystem.Instance.Initialise(team1, team2);

        GlobalGame.Instance.CurrentGameState = GlobalGame.GameState.InCombat;
        if (OnFinishedLoadingCombatScene != null)
            OnFinishedLoadingCombatScene.Invoke();

        if(sceneName != combatTutorialSceneName)
            CombatSystem.Instance.CombatEnd += Instance_CombatEnd;

        yield return async;
    }*/

    private static IEnumerator StartCombat(List<Character> team1, List<Character> team2)
    {

        GlobalGame.Instance.CurrentGameState = GlobalGame.GameState.InTransition;

        Camera camera = Camera.main;

        camera.GetComponent<TransitionFade>().StartFade(1.5f);
        yield return new WaitForSeconds(camera.GetComponent<TransitionFade>().GetTimeRemaining());

        CombatSystem.Instance.gameObject.SetActive(true);
        camera.gameObject.SetActive(false);
        CombatSystem.Instance.gameObject.transform.SetAsFirstSibling();

        CombatSystem.Instance.Initialise(team1, team2);

        if (OnFinishedLoadingCombatScene != null)
            OnFinishedLoadingCombatScene.Invoke();

        CombatSystem.Instance.CombatEnd += Instance_CombatEnd;
    }

    private static void Instance_CombatEnd()
    {
        CombatSystem.Instance.gameObject.SetActive(false);
        GameObject.FindGameObjectWithTag("Player").transform.Find("Main Camera").gameObject.SetActive(true);
        Camera.main.gameObject.SetActive(true);
        GlobalGame.Instance.CurrentGameState = GlobalGame.GameState.Normal;
    }

    /// <summary>
    /// Initialise a Fight between 2 characters.
    /// </summary>
    /// <param name="team1_c1">Character that will be on team 1 (Left side) </param>
    /// <param name="team2_c1">Character that will be on team 2 (Right side) </param>
    /// <returns> If returns true then initialised successfully, else there is a problem with the provided Character parameters.</returns>
	public static bool InitialiseFight(Character team1_c1, Character team2_c1)
    {
        List<Character> team1, team2;

        if (!team1_c1 || !team2_c1)
            return false;

        team1 = new List<Character>() { team1_c1 };
        team2 = new List<Character>() { team2_c1 };

        //need monobehaviour to start a coroutine.
        team1_c1.StartCoroutine(StartCombat(team1, team2));
        return true;
    }

    /// <summary>
    /// Initialise a fight between a 2 teams of 2
    /// </summary>
    /// <param name="team1_c1">Character 1 for team 1 (Left side)</param>
    /// <param name="team1_c2">Character 2 for team 1 (Left side)</param>
    /// <param name="team2_c1">Character 1 for team 2 (Right side)</param>
    /// <param name="team2_c2">Character 2 for team 2 (Right side)</param>
    /// <returns> If returns true then initialised successfully, else there is a problem with the provided Character parameters.</returns>
    public static bool InitialiseDoubleFight(Character team1_c1, Character team1_c2, Character team2_c1, Character team2_c2)
    {
        if (!team1_c1 || !team1_c2 || !team2_c1 || !team2_c2)
            return false;

        InitialiseMultiFight(new List<Character>() { team1_c1, team2_c2 }, new List<Character>() { team2_c1, team2_c2 });
        return true;
    }

    /// <summary>
    /// Intialise a fight between 2 teams of characters.
    /// </summary>
    /// <param name="team1"> A List of Characters to be in team 1 (Left side)</param>
    /// <param name="team2"> A list of Charactesr to be in team 2 (Right side)</param>
    /// <returns>If returns true then initialised successfully, else there is a problem with the provided Character Lists. (Either 1 or more teams contain less than 1 character or a Character is null.)</returns>
    public static bool InitialiseMultiFight(List<Character> team1, List<Character> team2)
    {
        //Check if teams are valid lists and are large than 0.
        if ((team1 == null || team1.Count < 1) || (team2 == null || team2.Count < 1))
            return false;

        //check if any characters in team1 are null.
        foreach (Character c in team1)
        {
            if (!c)
                return false;
        }

        //check if any characters in team2 are null.
        foreach (Character c in team2)
        {
            if (!c)
                return false;
        }


        //need monobehaviour to start a coroutine.
        team1[0].StartCoroutine(StartCombat(team1, team2));


        return true;
    }


    public delegate void CombatSystemSetupEventHandler();
    static public event CombatSystemSetupEventHandler OnFinishedLoadingCombatScene;


}
