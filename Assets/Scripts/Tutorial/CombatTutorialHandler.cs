using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CombatTutorialHandler : CombatUIHandler
{
    public RectTransform GrayPanel;
    private AsyncOperation async_MainMap;
    private int m_currentTurn = 0;
    private Dictionary<string, Action> m_speechFinishActions = new Dictionary<string, Action>();
    private List<CombatStatusSpeech> m_startSpeechPerTurn = new List<CombatStatusSpeech>();

    private new void Awake()
    {
        //Add the handler again as we are ovveriting start
        base.Awake();

        CombatStatusDialogHandler.Instance.OnSpeechSpoke += SpeechFinished;

        //Set up speech and actions
        //First turn speech
        {
            CombatStatusSpeech SpeechTen = new CombatStatusSpeech("Now try to use an attack on Ceejay!");
            m_speechFinishActions.Add(SpeechTen.Text, HightlighteSpells);
            CombatStatusSpeech SpeechNine = new CombatStatusSpeech("After you selected the target, press Enter to execute the spell over you target. Keep in mind that some spells don’t need targets and will execute without needed to target.", SpeechTen);
            CombatStatusSpeech EightSpeech = new CombatStatusSpeech("Depending on the attack, you can target different enemies/allies. To change target use the arrow or W,A,S,D keys on your keyboard.", SpeechNine);
            CombatStatusSpeech SeventhSpeech = new CombatStatusSpeech("Select a Spell from the frame and Hit enter. Then automatically a target will be selected, and an orange arrow will appear over his name.", EightSpeech);
            CombatStatusSpeech SixtSpeech = new CombatStatusSpeech("The spells button will show you all the spells you got. I already clicked it for you, you should now see all you spells.", SeventhSpeech);
            m_speechFinishActions.Add(SixtSpeech.Text, UnHiglightRun);
            CombatStatusSpeech fifthSpeech = new CombatStatusSpeech("The run button is disabled for this combat, as it is a boss, but in small combats, you can run away from the combat.", SixtSpeech);
            m_speechFinishActions.Add(fifthSpeech.Text, HighlightRun);
            CombatStatusSpeech forthSpeech = new CombatStatusSpeech("What is been highlighted now is the Utility Frame. In here you can use spells, items or run away from the enemy. Let’s explain each part in detail.", fifthSpeech);
            m_speechFinishActions.Add(forthSpeech.Text, HighlightUtilityFrame);
            CombatStatusSpeech thirdSpeech = new CombatStatusSpeech("Each team Frame will have information about each character in the team. The name, level, health and mana.", forthSpeech);
            m_speechFinishActions.Add(thirdSpeech.Text, HighlightEntityFrame);
            CombatStatusSpeech secondSpeech = new CombatStatusSpeech("What you can see highlighted now are the team frames. The green one is your team and the Red one the enemy team.", thirdSpeech);
            CombatStatusSpeech firstTurnSpeech = new CombatStatusSpeech("Welcome to the Combat Tutorial. In here we will show you show to use the system to win battles! Let’s start by explaining each section of interface.", secondSpeech);
            m_speechFinishActions.Add(firstTurnSpeech.Text, HighlightTeamFrames);
            m_startSpeechPerTurn.Add(firstTurnSpeech);
        }

        //Second turn
        {
            CombatStatusSpeech secondSpeech = new CombatStatusSpeech("So, I just enabled the Items frame and disabled the Spells one. Try to use an item on yourself!");
            m_speechFinishActions.Add(secondSpeech.Text, HighlightItemsDisable);
            CombatStatusSpeech firstSpeech = new CombatStatusSpeech("Now let’s have a look at Items. The items work in same way as the spells, but they will have different effects, like giving you a turn, give you some Mana back ... and more.", secondSpeech);
            m_speechFinishActions.Add(firstSpeech.Text, HighlightItems);
            m_startSpeechPerTurn.Add(firstSpeech);
        }

        //Third turn

        {
            CombatStatusSpeech firstSpeech = new CombatStatusSpeech("From now I will enable everything for you, I will add all the items and spells you currently got to the lists. Try to defeat him!");
            m_speechFinishActions.Add(firstSpeech.Text, TutorialFinished);
            m_startSpeechPerTurn.Add(firstSpeech);
        }

        CombatSystem.Instance.CombatEnd += SwitchToMainMap;
    }

    IEnumerator StartLoadingMainMap()
    {
        async_MainMap = SceneManager.LoadSceneAsync("MainMap");
        async_MainMap.allowSceneActivation = false;

        yield return null;
    }

    IEnumerator StartMainMapTransition()
    {
        if(async_MainMap == null)
        {
            StartCoroutine(StartLoadingMainMap());
            StartCoroutine(StartMainMapTransition());
            yield break;
        }

        if (async_MainMap.progress < 0.89f)
            yield return null;

        yield return new WaitForSeconds(Camera.main.GetComponent<TransitionFade>().StartFade(1.5f));

        Destroy(GameObject.Find("GlobalGameObject"));
        Destroy(GameObject.Find("AttackDatabse"));
        Destroy(GameObject.Find("ItemDatabase"));

        async_MainMap.allowSceneActivation = true;
    }

    private void SwitchToMainMap()
    {
        StartCoroutine(StartMainMapTransition());
    }

    protected override void ActivateUIForPlayer()
    {
        base.DisableUIForPlayer();
        CombatStatusDialogHandler.Instance.EndConversation();
        CombatStatusDialogHandler.Instance.gameObject.transform.GetChild(0).GetComponent<Button>().interactable = true;
        CombatStatusDialogHandler.Instance.gameObject.transform.GetChild(0).GetComponent<Button>().Select();
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(CombatStatusDialogHandler.Instance.gameObject.transform.GetChild(0).gameObject);
        CombatStatusDialogHandler.Instance.StartConversation(m_startSpeechPerTurn[m_currentTurn]);
        m_currentTurn++;
    }

    private void DisableDialogInteracuable()
    {
        CombatStatusDialogHandler.Instance.gameObject.transform.GetChild(0).GetComponent<Button>().interactable = false;
        CombatStatusDialogHandler.Instance.gameObject.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
    }

    private void TutorialFinished()
    {
        GrayPanel.gameObject.SetActive(false);
        base.ActivateUIForPlayer();
        OnSpellToggle(true);
        SpellButton.Select();
        DisableDialogInteracuable();
    }

    private void HighlightItemsDisable()
    {
        GrayPanel.gameObject.SetActive(false);
        DisableDialogInteracuable();
        foreach (Transform t in ItemsFrame.transform)
        {
            Toggle toggle = t.GetComponent<Toggle>();
            if (toggle)
            {
                toggle.Select();
                break;
            }
        }
    }

    private void HighlightItems()
    {
        GrayPanel.gameObject.SetActive(true);
        Itemsbutton.interactable = true;
        SetInteactuableStatusForSubbButons(false, ItemsFrame);
        foreach (Transform t in ItemsFrame.transform)
        {
            Toggle toggle = t.GetComponent<Toggle>();
            if (toggle)
            {
                toggle.interactable = true;
                break;
            }
        }
        OnItemToggle(true);
    }

    private void HightlighteSpells()
    {
        SpellButton.interactable = true;
        SetInteactuableStatusForSubbButons(false, SpellsFrame);
        foreach (Transform t in SpellsFrame.transform)
        {
            Toggle toggle = t.GetComponent<Toggle>();
            if (toggle)
            {
                toggle.interactable = true;
                toggle.Select();
                break;
            }
        }
        OnSpellToggle(true);
        GrayPanel.gameObject.SetActive(false);
        DisableDialogInteracuable();
    }

    private void UnHiglightRun()
    {
        RunButton.interactable = false;
    }

    private void HighlightRun()
    {
        RunButton.interactable = true;
        RunButton.onClick.RemoveAllListeners();
    }

    private void HighlightUtilityFrame()
    {
        GrayPanel.SetAsLastSibling();
        CombatStatusDialogHandler.Instance.gameObject.transform.SetAsLastSibling();
        Itemsbutton.transform.parent.SetAsLastSibling();
    }

    private void HighlightEntityFrame()
    {
        GrayPanel.SetAsLastSibling();
        CombatStatusDialogHandler.Instance.gameObject.transform.SetAsLastSibling();
        AllyTeamFrame.transform.SetAsLastSibling();
    }

    private void HighlightTeamFrames()
    {
        GrayPanel.SetAsLastSibling();
        CombatStatusDialogHandler.Instance.gameObject.transform.SetAsLastSibling();
        AllyTeamFrame.transform.SetAsLastSibling();
        EnemyTeamFrame.transform.SetAsLastSibling();
    }

    private void SpeechFinished(CombatStatusSpeech s)
    {
        Action actionToExecute;
        if (m_speechFinishActions.TryGetValue(s.Text, out actionToExecute))
        {
            actionToExecute();
        }
    }
}
