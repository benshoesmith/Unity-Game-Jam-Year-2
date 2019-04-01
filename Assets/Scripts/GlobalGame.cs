using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GlobalGame : MonoBehaviour {

    public static GlobalGame singleton_ = null;

    private Character player_ = null;

    //Get the singleton.
    public static GlobalGame Instance
    {
        get { return singleton_; }
    }

    public enum GameState
    {
        Normal = 0,
        Paused = 1,
        InCombat = 2,
        InDialog = 3,
        InTransition = 4
    }

    public enum PlayerState
    {
        Normal = 0,
        InMenu = 1
    }

    private GameState currentGameState_ = GameState.Normal;
    private PlayerState currentPlayerState_ = PlayerState.Normal;

    public void SaveGame()
    {
        player_.SaveCharacter();
    }

    private void Awake()
    {

        //Make sure there is only one instance of GlobalGame per game.
        if (!singleton_)
            singleton_ = this;
        else
            Destroy(this);

        DontDestroyOnLoad(this.gameObject);
    }

    // Use this for initialization
    void Start ()
    {

        GameObject go = GameObject.FindGameObjectWithTag("Player");

        if(go)
            player_ = go.GetComponent<Character>();

        if(!player_)
        {
            Debug.LogError("Could not find the Player Character. The game will not work as intended.");
            return;
        }

        //player_.LoadCharacter();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public GameState CurrentGameState
    {
        get { return currentGameState_; }
        set
        {
            if (currentGameState_ == value)
                return;

            currentGameState_ = value;

            if (GameStateSwitched != null)
                GameStateSwitched.Invoke();
        }
    }

    public PlayerState CurrentPlayerState
    {
        get { return currentPlayerState_; }
        set
        {
            if (currentPlayerState_ == value)
                return;

            currentPlayerState_ = value;

            if (PlayerStateSwitched != null)
                PlayerStateSwitched.Invoke();
        }
    }

    public Character Player
    {
        get { return player_; }
    }

    public delegate void GlobalGameEventHandler();
    public event GlobalGameEventHandler GameStateSwitched;
    public event GlobalGameEventHandler PlayerStateSwitched;
}
