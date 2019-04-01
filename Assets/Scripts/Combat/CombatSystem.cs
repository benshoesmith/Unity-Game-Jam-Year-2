using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(MiniGameHandler))]
public class CombatSystem : MonoBehaviour
{
    public class CombatSystemSettings
    {
        public enum AttackOrder
        {
            //e.g. Let all of team 1 attack then let all team 2 attack. then repeat.
            AlternateTeamEveryTeamTurn = 0
        }

        //How the next character to attack is picked. Default is every character in a team attacks before switching.
        public AttackOrder attackOrder_ = AttackOrder.AlternateTeamEveryTeamTurn;

        //False == team 1 attacks first, true is team 2 attacked first.
        public bool teamAttacksFirst = false;

    }

    public static CombatSystem singleton_ = null;
    /// <summary>
    /// This holds the multipliers of damge between types of attacks.
    /// </summary>
    [SerializeField]
    private Dictionary<Attack.Type, Dictionary<Attack.Type, float>> effectivenessAgainst = new Dictionary<Attack.Type, Dictionary<Attack.Type, float>>();

    public Dictionary<Attack.Type, Dictionary<Attack.Type, float>> EffectivenessAgainst
    {
        get { return effectivenessAgainst; }
    }

    [SerializeField]
    private CombatSystemSettings settings_ = null;

    [SerializeField]
    private AudioSource attackClipp_;

    //false is team1, true is team2.
    private bool currentTeamsTurn_ = false;

    //The character of the current characters turn.
    private Character currentCharactersTurn_ = null;

    private List<Character> team1_ = null, team2_ = null;

    private MiniGameHandler mgh_ = null;


    //Returns the singleton instance of CombatSystem.
    public static CombatSystem Instance
    {
        get { return singleton_; }
    }


    void Awake()
    {
        //Make sure there is only 1 instance of CombatSystem.
        if (!singleton_ || singleton_ == this)
            singleton_ = this;
        else
        {
            Debug.LogError("Attempt to create another instance of CombatSystem.");
            Destroy(this);
        }

        mgh_ = GetComponent<MiniGameHandler>();

        //TODO: Update values once all the weapon types/stats have been created with actual values..
        effectivenessAgainst = new Dictionary<Attack.Type, Dictionary<Attack.Type, float>>
        {
            ///NORMAL Multiplers
            {
                Attack.Type.Normal,
                new Dictionary<Attack.Type, float>
                {
                    ///NORMAL TYPE MULTIPLIER VALUES.
                    { Attack.Type.Normal, 1.0f },
                    { Attack.Type.Magic, 0.8f },
                    { Attack.Type.Water, 0.9f },
                    { Attack.Type.Fire, 0.8f }
                }
            },
            //Magic Multipliers
            {
                Attack.Type.Magic,
                new Dictionary<Attack.Type, float>
                {
                    { Attack.Type.Normal, 1.1f },
                    { Attack.Type.Magic, 1.0f },
                    { Attack.Type.Water, 0.9f },
                    { Attack.Type.Fire, 0.9f }
                }   
            },
            //Fire Mutlipliers
            {
            Attack.Type.Fire,
                new Dictionary<Attack.Type, float>
                {
                    { Attack.Type.Normal, 1.2f },
                    { Attack.Type.Magic, 0.9f },
                    { Attack.Type.Water, 0.5f },
                    { Attack.Type.Fire, 1.0f }
                }
            },
            //Water Multipliers
            {
                Attack.Type.Water,
                new Dictionary<Attack.Type, float>
                {
                     { Attack.Type.Normal, 1.1f },
                    { Attack.Type.Magic, 0.9f },
                    { Attack.Type.Water, 1.0f },
                    { Attack.Type.Fire, 1.5f }
                }
            }
        };

        gameObject.SetActive(false);
    }

    public bool Initialise(List<Character> team1, List<Character> team2, CombatSystemSettings settings = null)
    {
        DialogHandler.Instance.EndConversation();

        GlobalGame.Instance.CurrentGameState = GlobalGame.GameState.InTransition;

        //if settings not provided then use the default settings.
        if (settings == null)
            settings_ = new CombatSystemSettings();
        else
            settings_ = settings;

        currentTeamsTurn_ = settings_.teamAttacksFirst;

        team1_ = team1;
        team2_ = team2;

        //loop through current team to find the first valid character (not null and is alive.)
        for (int i = 0; !currentCharactersTurn_ && i < ((currentTeamsTurn_) ? team2_ : team1_).Count; i++)
        {
            if (currentCharactersTurn_ && !currentCharactersTurn_.IsDead)
                break;
            else
                currentCharactersTurn_ = ((currentTeamsTurn_) ? team2_ : team1_)[i];
        }

        //If there isnt a valid character on starting team then return.
        if (!currentCharactersTurn_ || currentCharactersTurn_.IsDead)
            return false;

        CombatEnd += DistributeLoot;
        CombatEnd += StopEffects;

        if (CombatStart != null)
            CombatStart.Invoke();

        StartCoroutine(StartTansition());

        return true;
    }

    private IEnumerator StartTansition()
    {
        yield return new WaitForSeconds(Camera.main.GetComponent<TransitionFade>().StartFade(1.5f, eTranitionStart.END));
        if (CombatTransitionFinished != null)
            CombatTransitionFinished.Invoke();

        GlobalGame.Instance.CurrentGameState = GlobalGame.GameState.InCombat;

        AudioSource audio = gameObject.GetComponent<AudioSource>();
        if (audio)
            audio.Play();

        currentCharactersTurn_.CharacterTurnStarts();
    }

    private void StopEffects()
    {
        AudioSource audio = gameObject.GetComponent<AudioSource>();
        if (audio)
            audio.Play();
    }

    private void DistributeLoot()
    {
        List<Character> enemyDeadTeam = (team1_.Contains(GameObject.FindGameObjectWithTag("Player").GetComponent<Character>())) ? team2_ : team1_;
        foreach (Character deadEnemy in enemyDeadTeam)
        {
            IALootTable lootTable = deadEnemy.gameObject.GetComponent<IALootTable>();
            if (lootTable != null && deadEnemy.IsDead)
            {
                Dictionary<int, int> loot = lootTable.GetLoot();
                foreach (KeyValuePair<int, int> Item in loot)
                {
                    InventoryHandler.Instance.AddItem(Item.Key, Item.Value);
                }
            }
        }
    }

    private IEnumerator AttackWithoutMinigame(Attack attack, Character[] targets)
    {

        yield return new WaitForSeconds(GetComponent<CombatSpriteSetup>().AnimateCharacter(currentCharactersTurn_));

        attackClipp_.Play();

        List<KeyValuePair<Character, int>> damagesDone = attack.UsedSpell(CurrentCharacterTurn, targets);
        CombatStatusSpeech lastSpeech = null;
        foreach (KeyValuePair<Character, int> charDamaged in damagesDone)
        {
            Debug.Log("Character: " + currentCharactersTurn_ + " attacks " + charDamaged.Key + " for " + charDamaged.Value + "damage.");
            Debug.Log("New Healh of Character: " + charDamaged.Key + charDamaged.Key.Health);
            CombatStatusSpeech s = new CombatStatusSpeech(currentCharactersTurn_.name + " attacks " + charDamaged.Key.name + " for " + charDamaged.Value + " damage.", lastSpeech);
            lastSpeech = s;
        }
        CombatStatusDialogHandler.Instance.ConversationEnd += NextTurnAfterSpeech;
        CombatStatusDialogHandler.Instance.StartConversation(lastSpeech);
        CombatStatusDialogHandler.Instance.SetButtonInteractions(true);
    }

    private IEnumerator AttackCharacterAfterMinigame(Attack attack, Character[] targets)
    {
        if (!mgh_ || !mgh_.CurrentMiniGame)
            yield return null;

        while (!mgh_.CurrentMiniGame.Finished)
            yield return null;

        MiniGame.GameOutcome outcome = mgh_.CurrentMiniGame.Outcome;
        float score = mgh_.CurrentMiniGame.Score;

        yield return new WaitForSeconds(GetComponent<CombatSpriteSetup>().AnimateCharacter(currentCharactersTurn_));

        attackClipp_.Play();

        List<KeyValuePair<Character, int>> damagesDone = attack.UsedSpell(CurrentCharacterTurn, targets, score);

        CombatStatusSpeech lastSpeech = null;
        foreach (KeyValuePair<Character, int> charDamaged in damagesDone)
        {
            Debug.Log("Character: " + currentCharactersTurn_ + " attacks " + charDamaged.Key + " for " + charDamaged.Value + "damage.");
            Debug.Log("New Healh of Character: " + charDamaged.Key + charDamaged.Key.Health);
            CombatStatusSpeech s = new CombatStatusSpeech(currentCharactersTurn_.name + " attacks " + charDamaged.Key.name + " for " + charDamaged.Value + " damage.", lastSpeech);
            lastSpeech = s;
        }
        CombatStatusDialogHandler.Instance.ConversationEnd += NextTurnAfterSpeech;
        CombatStatusDialogHandler.Instance.StartConversation(lastSpeech);
        CombatStatusDialogHandler.Instance.SetButtonInteractions(true);

        mgh_.CurrentMiniGame.SetToNotNeeded();
        yield break;
    }

    public bool AttackCharacter(Attack attack, Character[] charactersToAttack)
    {
        if (attack == null || charactersToAttack.Length == 0)
            return false;

        //can not attack dead characters.
        List<Character> listToAtack = new List<Character>();
        foreach(Character c in charactersToAttack)
        {
            if (!c.IsDead)
            {
                listToAtack.Add(c);
            }
        }

        //Currently 50/50 chance to have a minigame.
        int chance = Random.Range(0, 100);
        //if chance is over 50 and the current characters turn in a player then start a minigame.
        if (chance > 50 && CurrentCharacterTurn.GetComponent<PlayerController>() != null)
        {
            //load random minigame
            mgh_.LoadMiniGame(-1);
            StartCoroutine(AttackCharacterAfterMinigame(attack, listToAtack.ToArray()));
        }else
        {
            StartCoroutine(AttackWithoutMinigame(attack, listToAtack.ToArray()));
        }
        return true;
    }
  
    public void UseItem(Character target, System.Action<Character, Character, Character[], Character[]> itemFunct)
    {
        List<Character> c_enemy = (team2_.Contains(CurrentCharacterTurn)) ? team1_ : team2_;
        List<Character> c_allies = (c_enemy == team1_) ? team2_ : team1_;

        List<Character> enemies = new List<Character>();
        List<Character> allies = new List<Character>();
        foreach (Character c in c_enemy)
        {
            if (c.IsDead)
                continue;
            enemies.Add(c);
        }
        foreach (Character c in c_allies)
        {
            if (c.IsDead)
                continue;
            allies.Add(c);
        }
        itemFunct(CurrentCharacterTurn, target, enemies.ToArray(), allies.ToArray());
        CombatStatusSpeech s = new CombatStatusSpeech(currentCharactersTurn_.name + " used an item!");
        CombatStatusDialogHandler.Instance.ConversationEnd += NextTurnAfterSpeech;
        CombatStatusDialogHandler.Instance.StartConversation(s);
        CombatStatusDialogHandler.Instance.SetButtonInteractions(true);
    }

    public bool RunAway()
    {
        if (CombatEnd != null)
            CombatEnd.Invoke();


        Debug.Log("Combat Finished.");

        return true;
    }

    private void NextTurnAfterSpeech()
    {
        CombatStatusDialogHandler.Instance.ConversationEnd -= NextTurnAfterSpeech;
        NextTurn();
    }

    public bool NextTurn()
    {
        //if the team attacked is all dead
        if (IsAllTeamDead(((!currentTeamsTurn_) ? team2_ : team1_)))
        {
            if (CombatEnd != null)
                CombatEnd.Invoke();


            Debug.Log("Combat Finished.");

            return true;
        }

        currentCharactersTurn_.CharacterTurnEnds();

        switch (settings_.attackOrder_)
        {
            case CombatSystemSettings.AttackOrder.AlternateTeamEveryTeamTurn:

                if (IsLastAliveInTeamToTakeTurn(((currentTeamsTurn_) ? team2_ : team1_), currentCharactersTurn_))
                {
                    currentTeamsTurn_ = !currentTeamsTurn_;
                    currentCharactersTurn_ = GetFirstCharacterToTakeTurnInTeam(((currentTeamsTurn_) ? team2_ : team1_));

                    if (TeamSwitch != null)
                        TeamSwitch.Invoke();
                }
                else
                {
                    currentCharactersTurn_ = GetNextAliveCharacterInTeam(((currentTeamsTurn_) ? team2_ : team1_), currentCharactersTurn_);
                }

            break;
        }

        if (AttackFinished != null)
            AttackFinished.Invoke();

        currentCharactersTurn_.CharacterTurnStarts();

        return true;
    }


    /// <summary>
    /// Check if a character is the last character alive in its team to take a turn.
    /// </summary>
    /// <param name="team">The team of the current character.</param>
    /// <param name="characterToCheck">The character to check</param>
    /// <returns>returns true if is it the last character to take a turn in the team, else it returns false.</returns>
    private bool IsLastAliveInTeamToTakeTurn(List<Character> team, Character characterToCheck)
    {
        int index = team.IndexOf(characterToCheck);

        if(index<0)
        {
            Debug.LogError("Character is not in team");
            return true;
        }

        for(int i = index+1; i < team.Count; i++)
        {
            if (team[i] && !team[i].IsDead)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Get the next character alive in a team
    /// </summary>
    /// <param name="team">Team to find the next character that is alive and hasnt attacked this team turn.</param>
    /// <param name="current">The current characters turn.</param>
    /// <returns>Either the next Character or null meaning there is no more characters able to attack this team turn.</returns>
    private Character GetNextAliveCharacterInTeam(List<Character> team, Character current)
    {
        int index = team.IndexOf(current);

        if (index < 0)
            return null;

        for(int i = index+1; i < team.Count; i++)
        {
            if (team[i] && !team[i].IsDead)
                return team[i];
        }

        return null;
    }

    /// <summary>
    /// Returns the first character in a team that is able to attack on a new turn.
    /// </summary>
    /// <param name="team">the teams whos turn it is.</param>
    /// <returns>the first character that can attack, or null if none.</returns>
    private Character GetFirstCharacterToTakeTurnInTeam(List<Character> team)
    {
        for(int i = 0; i < team.Count; i++)
        {
            if (team[i] && !team[i].IsDead)
                return team[i];
        }

        return null;
    }

    /// <summary>
    /// Check if a whole team is dead.
    /// </summary>
    /// <param name="team">team to check is dead</param>
    /// <returns>true if all the characters in the team is dead. false if atleast one is alive.</returns>
    private bool IsAllTeamDead(List<Character> team)
    {
        foreach(Character c in team)
        {
            if (c && !c.IsDead)
                return false;
        }

        return true;
    }

    public List<Character> GetAllAliveInTeam(List<Character> team)
    {
        List<Character> alive = new List<Character>();

        for (int i = 0; i < team.Count; i++)
        {
            if (team[i] && !team[i].IsDead)
                alive.Add(team[i]);
        }

        return alive;
    }

    public List<Character> Team1
    {
        get { return team1_; }
    }

    public List<Character> Team2
    {
        get { return team2_; }
    }

    public bool CurrentTeam
    {
        get { return currentTeamsTurn_; }
    }

    public Character CurrentCharacterTurn
    {
        get { return currentCharactersTurn_; }
    }

    public delegate void CombatSystemEventHandler();
    public event CombatSystemEventHandler CombatStart;
    public event CombatSystemEventHandler AttackFinished;
    public event CombatSystemEventHandler TeamSwitch;
    public event CombatSystemEventHandler CombatEnd;
    public event CombatSystemEventHandler CombatTransitionFinished;

    public delegate void CombatSystemAttackEventHandler(Character attacker, List<Character> victims);
}
