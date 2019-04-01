using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatSpriteSetup : MonoBehaviour
{

    public SpriteRenderer Ally1;
    public SpriteRenderer Ally2;
    public SpriteRenderer Enemy1;
    public SpriteRenderer Enemy2;

    // Use this for initialization
    void Awake()
    {
        CombatSystem.Instance.CombatStart += SetupCharacterRenderers;
    }

    public float AnimateCharacter(Character c)
    {
        Animator a = null;

        if (CombatSystem.Instance.Team1.Count >= 1 && c == CombatSystem.Instance.Team1[0])
            a = Ally1.GetComponent<Animator>();
        else if (CombatSystem.Instance.Team1.Count >= 2 && c == CombatSystem.Instance.Team1[1])
            a = Ally2.GetComponent<Animator>();
        else if (CombatSystem.Instance.Team2.Count >= 1 && c == CombatSystem.Instance.Team2[0])
            a = Enemy1.GetComponent<Animator>();
        else if (CombatSystem.Instance.Team2.Count >= 2 && c == CombatSystem.Instance.Team2[1])
            a = Enemy2.GetComponent<Animator>();


        a.SetTrigger("Attack");
        return a.GetCurrentAnimatorStateInfo(0).length;
    }

    private void SetupCharacterRenderers()
    {

        if (CombatSystem.Instance.Team1.Count >= 1)
            Ally1.sprite = CombatSystem.Instance.Team1[0].CombatSpriteLeft;

        if (CombatSystem.Instance.Team1.Count >= 2)
            Ally2.sprite = CombatSystem.Instance.Team1[1].CombatSpriteLeft;

        if (CombatSystem.Instance.Team2.Count >= 1)
            Enemy1.sprite = CombatSystem.Instance.Team2[0].CombatSpriteRight;
        if (CombatSystem.Instance.Team2.Count >= 2)
            Enemy2.sprite = CombatSystem.Instance.Team2[1].CombatSpriteRight;
    }
}
