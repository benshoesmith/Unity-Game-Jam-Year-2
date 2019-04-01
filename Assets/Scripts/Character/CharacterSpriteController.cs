using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO:
//
//  This Component will set the sprite of the Character owner.
//  Using the direction and current speed, from character_, it set
//  the correct sprite to be used (idle_up, idle_down, walking_up, etc.)
//
//  This can then be used on any GameObject with the CharacterComponent.
//


[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Character))]
public class CharacterSpriteController : MonoBehaviour {

    //The Character this controller is owned by.
    private Character character_ = null;

    //This is the Animator that controls character_ animations.
    private Animator animator_ = null;

	// Use this for initialization
	void Start ()
    {
        //Find the Character Component on this GameObject.
        if (!character_)
            character_ = GetComponent<Character>();
        if (!character_)
            Debug.LogError("The Character Component could not be found on this GameObject.");
        else
            character_.OnMovementStatusChange += Character__OnMovementStatusChange;

        //Find the Animator Component on this GameObject.
        if (!animator_)
            animator_ = GetComponent<Animator>();
        if (!animator_)
            Debug.LogError("The Animator Component could not be found on this GameObject.");
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(!character_ || !animator_)
            return;

        //send characters direction to the Animator.
        animator_.SetFloat("Direction_X", character_.Direction.x);
        animator_.SetFloat("Direction_Y", character_.Direction.y);

    }

    private void OnEnable()
    {
        if (!character_)
            return;

        //Add Character_OnMovementStatusChange to the OnMovementStatusChange event when the character movement status has been changed.
        character_.OnMovementStatusChange += Character__OnMovementStatusChange;
    }


    private void OnDisable()
    {
        if (!character_)
            return;

        character_.OnMovementStatusChange -= Character__OnMovementStatusChange;
    }

    private void Character__OnMovementStatusChange()
    {
        if (!animator_)
            return;

        animator_.SetTrigger("MovementStatusChanged");
        animator_.SetInteger("MovementStatus", (int) character_.CurrentMovementStatus);
    }




}
