using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellsButtonGenerator : MonoBehaviour {

    public GameObject SpellButtonPrefab;
    public CombatUIHandler cuihandler;

    private void Awake()
    {
        CombatSystem.Instance.CombatStart += SetUpSpells;
        CombatSystem.Instance.CombatEnd += RemoveAllSpellsButtons;
    }

    private void SetUpSpells()
    {
        Character player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();


        if (!player)
        {
            return;
        }

        foreach (Attack attack in player.Attacks)
        {
            if (attack == null)
                continue;
            GameObject entityFrame = Instantiate(SpellButtonPrefab, gameObject.transform);
            entityFrame.transform.GetChild(0).GetComponent<Text>().text = attack.Name;
            entityFrame.GetComponent<UsabeSpellButton>().AtackToUse = attack;
            entityFrame.GetComponent<UsabeSpellButton>().cuiHandler = cuihandler;
            entityFrame.GetComponent<Toggle>().interactable = true;
        }
    }

    private void RemoveAllSpellsButtons()
    {
        foreach(GameObject o in transform)
        {
            Destroy(o);
        }
    }
}
