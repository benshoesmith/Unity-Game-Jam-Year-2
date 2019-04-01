using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyCombinationGame : MiniGame {

    private List<char> potentialCharacters_ = new List<char>() { 'a', 's', 'd', 'f' };

    [SerializeField]
    private Text textUI_ = null;
    [SerializeField]
    private Text playerTextUI_ = null;
    [SerializeField]
    private float timeToComplete_ = 4.0f;
    [Range(0, 1)]
    [SerializeField]
    private float bonusPercent_ = 0.75f;
    [SerializeField]
    private Slider timerSlider_ = null;

    [SerializeField]
    private Color successColour_ = Color.green;
    [SerializeField]
    private Color bonusSuccessColour_ = Color.magenta;
    [SerializeField]
    private Color failColour_ = Color.red;


    [SerializeField]
    private int amountOfRandomCharacters_ = 4;


    private string randomString_ = "";
    private string currentAttempt_ = "";

    private void Awake()
    {

    }

    public override void StartMiniGame()
    {
        randomString_ = GenerateRandomString(amountOfRandomCharacters_, potentialCharacters_);

        if (textUI_)
            textUI_.text = randomString_;


        base.StartMiniGame();
    }


    private void Update()
    {
        if (!Started || Finished)
            return;
        float timeSinceStart = Time.time - TimeOfStart;
        float percentComplete = timeSinceStart / timeToComplete_;

        if (timerSlider_)
            timerSlider_.value = percentComplete;

        if (timeSinceStart > timeToComplete_)
            Fail();

        foreach(char key in potentialCharacters_)
        {
            if (Input.GetKeyDown(key.ToString()))
                currentAttempt_ += key;
        }

        if (playerTextUI_)
            playerTextUI_.text = currentAttempt_;

        int length = currentAttempt_.Length;

        if (currentAttempt_ != randomString_.Substring(0, length))
            Fail();

        //correct random string input
        if(currentAttempt_.Length == randomString_.Length && currentAttempt_ == randomString_)
        {
            if (percentComplete < bonusPercent_)
                SuccessWithinBonus();
            else
                Success();
        }



    }

    public string GenerateRandomString(int length, List<char> library)
    {
        if (length <= 0 || library.Count < 1)
                return "";

        string randomString = "";
        for (int i = 0; i < length; i++)
            randomString += library[Random.Range(0, library.Count)];

        return randomString;
    }

    private void Fail()
    {
        if (textUI_)
            textUI_.color = failColour_;
        score_ = 0.8f;
        Outcome = GameOutcome.Fail;
        EndGame();
    }

    private void Success()
    {
        if (textUI_)
            textUI_.color = successColour_;

        score_ = 1.05f;
        Outcome = GameOutcome.NormalWin;
        EndGame();
    }

    private void SuccessWithinBonus()
    {
        if (textUI_)
            textUI_.color = bonusSuccessColour_;

        score_ = 1.2f;
        Outcome = GameOutcome.BonusWin;
        EndGame();
    }

}
