using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsabeSpellButton : MonoBehaviour {
    public Attack AtackToUse;
    public CombatUIHandler cuiHandler;

    private void Start()
    {
        GetComponent<Toggle>().onValueChanged.AddListener(ChangedValue);
    }

    public void ChangedValue(bool status)
    {
        if (CombatSystem.Instance.CurrentCharacterTurn.Mana < AtackToUse.ResourceCost) {
            CombatStatusSpeech s = new CombatStatusSpeech("You dont have enough mana to execute that spell!");
            CombatStatusDialogHandler.Instance.StartConversation(s);
            return;
        }
        if (status && AtackToUse != null)
            cuiHandler.EnableTargetingForAttack(AtackToUse.TargeLimit, AtackToUse);
    }

}
