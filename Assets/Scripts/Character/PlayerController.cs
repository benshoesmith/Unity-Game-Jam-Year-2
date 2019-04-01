using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class PlayerController : MonoBehaviour
{

    //The Character this controller is owned by.
    private Character character_ = null;

    //When true the controller will not check input or modify character_ until paused_ is false.
    private bool paused_ = false;

    // Use this for initialization
    void Start()
    {
        //Find the Character Component on this GameObject.
        if (!character_)
            character_ = GetComponent<Character>();
        if (!character_)
            Debug.LogError("The Character Component could not be found on this GameObject.");
    }

    // Update is called once per frame
    void Update()
    {
        if (!character_)
            return;

        if (GlobalGame.Instance.CurrentGameState == GlobalGame.GameState.Paused || GlobalGame.Instance.CurrentPlayerState == GlobalGame.PlayerState.InMenu)
            Pause();
        else if(GlobalGame.Instance.CurrentGameState == GlobalGame.GameState.Normal && GlobalGame.Instance.CurrentPlayerState == GlobalGame.PlayerState.Normal)
            Resume();

        //if paused then do not check input.
        if (paused_)
            return;

        //Get the users input from WASD or Arrow keys.
        //Raw input so there is no smoothing.
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        input.Normalize();

        //if no keys are down then set the Character to no longer trying moving.
        if (input.sqrMagnitude == 0)
            character_.SetMovementStatus(Character.MovementStatus.None);
        //Else tell the Character Component that the user is trying to move the Player.
        else
            character_.SetMovementStatus(Character.MovementStatus.Walk);

        //Set the direction of the Character to be the same as the input. (If no input then the direction will be (0, 0) and the CharacterComponent will ignore the SetDirection call.)
        character_.SetDirection(input);

        if(Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.Return))
        {
            character_.InteractWithObjectsInRadius();
        }

    }

    public void Pause()
    {
        paused_ = true;
    }

    public void Resume()
    {
        paused_ = false;
    }

}