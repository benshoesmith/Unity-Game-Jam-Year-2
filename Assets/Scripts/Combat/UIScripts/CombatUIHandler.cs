using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatUIHandler : MonoBehaviour {

    public GameObject EntityHpDisplayPrefab;
    public GameObject AllyTeamFrame;
    public GameObject EnemyTeamFrame;
    public Toggle SpellButton;
    public Toggle Itemsbutton;
    public Button RunButton;
    public GameObject ItemsFrame;
    public GameObject SpellsFrame;
    public Text FrameText;

    protected Action<Character, Character, Character[], Character[]> m_ActiveItemToUse;
    protected Attack m_ActiveAttackUsage;

    protected void Awake()
    {
        CombatSystem.Instance.CombatStart += SetUpUI;
    }

    protected void SetUpUI()
    {
        Itemsbutton.onValueChanged.RemoveAllListeners();
        SpellButton.onValueChanged.RemoveAllListeners();
        SpellButton.onValueChanged.AddListener(OnSpellToggle);
        Itemsbutton.onValueChanged.AddListener(OnItemToggle);
        AddTeamEntityInfoToFrame(AllyTeamFrame, CombatSystem.Instance.Team1);
        AddTeamEntityInfoToFrame(EnemyTeamFrame, CombatSystem.Instance.Team2);
        SpellButton.isOn = true;
        Itemsbutton.isOn = false;
        OnSpellToggle(true);
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(false);
        }
        Character player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
        player.OnCharacterTurnStart += ActivateUIForPlayer;
        player.OnCharacterTurnStart += PlayTurnEffect;
        player.OnCharacterTurnEnds += DisableUIForPlayer;
        CombatSystem.Instance.CombatEnd += CleanUpAtend;
        CombatSystem.Instance.CombatTransitionFinished += ShowUI;
    }

    private void ShowUI()
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }
    }

    private void CleanUpAtend()
    {
        Character player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();

        //not dead
        if (player)
        {
            player.OnCharacterTurnStart -= ActivateUIForPlayer;
            player.OnCharacterTurnStart -= PlayTurnEffect;
            player.OnCharacterTurnEnds -= DisableUIForPlayer;
        }
        else
        {
            Debug.LogWarning("Player dead");
        }

        //Clean up the UI
        CleanUPEntityInfoFrame(AllyTeamFrame);
        CleanUPEntityInfoFrame(EnemyTeamFrame);
    }

	private void Update () {
        if (m_ActiveItemToUse != null || m_ActiveAttackUsage != null)
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                SetAllEnityTargeteables(false);
                SetInteactuableStatusForSubbButons(true, SpellsFrame);
                SetInteactuableStatusForSubbButons(true, ItemsFrame);
                RunButton.interactable = true;
                SpellButton.interactable = true;
                Itemsbutton.interactable = true;
            }
        }
	}

    private void PlayTurnEffect()
    {
        if (gameObject.GetComponent<AudioSource>())
            gameObject.GetComponent<AudioSource>().Play();
    }

    protected virtual void ActivateUIForPlayer()
    {
        //Enable buttons
        SpellButton.interactable = true;
        Itemsbutton.interactable = true;
        RunButton.interactable = true;
        SetInteactuableStatusForSubbButons(true, SpellsFrame);
        SetInteactuableStatusForSubbButons(true, ItemsFrame);
        OnSpellToggle(true);
    }

    protected virtual void DisableUIForPlayer()
    {
        //Disable buttns
        SpellButton.interactable = false;
        Itemsbutton.interactable = false;
        RunButton.interactable = false;
        SetInteactuableStatusForSubbButons(false, SpellsFrame);
        SetInteactuableStatusForSubbButons(false, ItemsFrame);
        OnSpellToggle(false);
        SpellButton.isOn = false;
    }


    private void CleanUPEntityInfoFrame(GameObject frame)
    {
        foreach (GameObject o in frame.transform)
        {
            if (o.tag == "CombatSystemEntityInformation")
            {
                Destroy(o);
            }
        }
    }

    private void AddTeamEntityInfoToFrame(GameObject frame, List<Character> entityTeam)
    {
        for (int i = 0; i < entityTeam.Count; i++)
        {
           GameObject o = Instantiate(EntityHpDisplayPrefab, frame.transform);
           o.GetComponent<EntityHelpFrameButton>().ThisEntity = entityTeam[i];
           o.GetComponent<EntityHelpFrameButton>().combatUIHandler = this;
        }
    }

    private void SetAllEnityTargeteables(bool targeteable)
    {
        SetTeamsEntityTargeteable(targeteable, EnemyTeamFrame);
        SetTeamsEntityTargeteable(targeteable, AllyTeamFrame);
        if (!targeteable)
        {
            m_ActiveItemToUse = null;
            m_ActiveAttackUsage = null;
        }
    }

    private void SetTeamsEntityTargeteable(bool targeteable, GameObject teamFrame)
    {
        foreach (Transform go in teamFrame.transform)
        {
            if ("CombatSystemEntityInformation" == go.tag)
            {
                Button b = go.GetComponent<Button>();
                if (b)
                    b.interactable = targeteable;
            }
        }
    }

    protected void SetInteactuableStatusForSubbButons(bool interactuable, GameObject parentFrame)
    {
        foreach (Transform t in parentFrame.transform)
        {
            Toggle toggle = t.GetComponent<Toggle>();
            if (toggle)
            {
                toggle.interactable = interactuable;
            }
        }
    }

    private void SetTargeting(eTargetLimit possible_targets)
    {
        DisableUIForPlayer();
        switch (possible_targets)
        {
            case eTargetLimit.SELF_ONLY:
                EntityClicked(CombatSystem.Instance.CurrentCharacterTurn);
                return;
            case eTargetLimit.ONLY_ALLIES:
                SetTeamsEntityTargeteable(true, AllyTeamFrame);
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(AllyTeamFrame.transform.GetChild(1).gameObject);
                break;
            case eTargetLimit.ONLY_ALIES_NO_SELF:
                SetTeamsEntityTargeteable(true, AllyTeamFrame);
                foreach (Transform eif in AllyTeamFrame.transform)
                {
                    if ("CombatSystemEntityInformation" == eif.tag)
                    {
                        EntityHelpFrameButton entityHelperScript = eif.GetComponent<EntityHelpFrameButton>();
                        if (entityHelperScript)
                        {
                            if (entityHelperScript.ThisEntity == CombatSystem.Instance.CurrentCharacterTurn)
                            {
                                entityHelperScript.gameObject.GetComponent<Button>().interactable = false;
                                break;
                            }
                        }
                    }
                }
                break;
            case eTargetLimit.ONLY_ENEMIES:
                SetTeamsEntityTargeteable(true, EnemyTeamFrame);
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(EnemyTeamFrame.transform.GetChild(1).gameObject);
                break;
            case eTargetLimit.ALL:
                SetAllEnityTargeteables(true);
                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(AllyTeamFrame.transform.GetChild(1).gameObject);
                break;
            case eTargetLimit.NO_TARGET:
                EntityClicked(null);
                return;
        }
    }

    public void EnableTargetingForAttack(eTargetLimit possible_targets, Attack callback)
    {
        m_ActiveItemToUse = null;
        m_ActiveAttackUsage = callback;
        SetTargeting(possible_targets);
    }

    public void EnableTargetingForItem(eTargetLimit possible_targets, Action<Character, Character, Character[], Character[]> itemToUse)
    {
        m_ActiveItemToUse = itemToUse;
        m_ActiveAttackUsage = null;
        SetTargeting(possible_targets);
    }

    public void EntityClicked(Character clicked)
    {
        SetTeamsEntityTargeteable(false, EnemyTeamFrame);
        SetTeamsEntityTargeteable(false, AllyTeamFrame);
        if (m_ActiveItemToUse != null)
        {
            CombatSystem.Instance.UseItem(clicked, m_ActiveItemToUse);
        }
        if (m_ActiveAttackUsage != null)
        {
            CombatSystem.Instance.AttackCharacter(m_ActiveAttackUsage, new Character[] { clicked });
        }
    }

    public void OnSpellToggle(bool status)
    {
        Itemsbutton.onValueChanged.RemoveAllListeners();
        SpellButton.onValueChanged.RemoveAllListeners();
        if (!status)
        {
            if(SpellsFrame.activeSelf)
                SpellButton.isOn = true;
            return;
        }
         FrameText.text = "Spells";
        SpellsFrame.transform.parent.parent.GetComponent<ScrollRect>().content = SpellsFrame.GetComponent<RectTransform>();
        ItemsFrame.SetActive(false);
        SpellsFrame.SetActive(true);
        Itemsbutton.isOn = false;
        SpellButton.onValueChanged.RemoveAllListeners();
        SpellButton.isOn = true;
        SpellButton.onValueChanged.RemoveListener(OnSpellToggle);
        Itemsbutton.onValueChanged.AddListener(OnItemToggle);
        SpellButton.onValueChanged.AddListener(OnSpellToggle);
    }

    public void OnItemToggle(bool status)
    {
        Itemsbutton.onValueChanged.RemoveAllListeners();
        SpellButton.onValueChanged.RemoveAllListeners();
        if (!status)
        {
            if (ItemsFrame.activeSelf)
                Itemsbutton.isOn = true;
            return;
        }
        FrameText.text = "Items";
        SpellsFrame.transform.parent.parent.GetComponent<ScrollRect>().content = ItemsFrame.GetComponent<RectTransform>();
        ItemsFrame.SetActive(true);
        SpellsFrame.SetActive(false);
        SpellButton.isOn = false;
        Itemsbutton.isOn = true;
        Itemsbutton.onValueChanged.AddListener(OnItemToggle);
        SpellButton.onValueChanged.AddListener(OnSpellToggle);
    }

    public void OnClickRun()
    {
        CombatSystem.Instance.RunAway();
    }
}
