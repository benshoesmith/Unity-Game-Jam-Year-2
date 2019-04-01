using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyCharacter : Character
{
    private int myTurns = 0;

    [SerializeField]
    private SubQuest tutoiralSubQuestKillBoss = null;

    [SerializeField]
    private Conversation questNotUnlockedSpeech_ =  null;

    override public void Start()
    {
        base.Start();

        NotifcationManager.Instance.AddNotification("Press f to Interact");
        NotifcationManager.Instance.AddNotification("Press Q to Open Quest Menu");

        OnCharacterTurnStart += TutorialEnemyCharacter_OnCharacterTurnStart;
    }

    public override void Interact(GameObject interacter)
    {
        if (tutoiralSubQuestKillBoss && tutoiralSubQuestKillBoss.IsUnlocked)
            base.Interact(interacter);
        else
            questNotUnlockedSpeech_.StartSpeech();
    }

    private void attack()
    {
        myTurns++;
        CombatSystem.Instance.AttackCharacter(AttacksDatabase.Instance.GetAttack(myTurns), CombatSystem.Instance.Team1.ToArray());
    }

    private void TutorialEnemyCharacter_OnCharacterTurnStart()
    {
        Debug.Log("Tutorial Boss is about to attack.");
        Invoke("attack", 1.2f);
    }

    public override void Damage(int damage)
    {
        if (damage <= 0)
            return;

        Health -= damage;

        //prevent character from going below 1 hp.
        if (Health <= 0)
            Health = 1;
    }

   

}
