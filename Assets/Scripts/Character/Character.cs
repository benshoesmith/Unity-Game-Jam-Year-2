using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

//Character 
//==============================================================
//      How To Use
//      ============================================================
//      
//          Add this script to a GameObject and it will add the 
//          necessary components, if they are not already added,
//          to make it a character.
//
//          It should be used as an interface with a 
//          controller (PlayerController, AIController, etc.)
//          to set its movementstatus, direction, or other future 
//          implementations.
//


[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Character : Interactable {

    [Header("Character Info")]

    //Used with JSON serialisation. Doesnt have to be unique as temp files also use transform instance id.
    [SerializeField]
    private string name_ = "DEFAULT";

    //Sprites of this character used in the combat scene.
    [SerializeField]
    private Sprite combatSpriteLeft_ = null, combatSpriteRight_ = null, speakerImage_ = null;

    [SerializeField]
    private CharacterData data_ = null;

    [Header("Movement and Physics")]

    //The Rigidbody2D of this Character.
    private Rigidbody2D rigidbody_ = null;
    private Collider2D collider_ = null;

    //The current direction the character is facing.
    [SerializeField] private Vector2 direction_ = Vector2.down;

    //The movement speed, in units per second, of this Character.
    public float walkSpeed_ = 80.0f;

    private float currentMovementSpeed_ = 0.0f;

    [Header("Interact Settings")]
    [SerializeField]
    private float interactRadius_ = 1.0f;

    public enum MovementStatus
    {
        None = 0,
        Walk = 1
    }
    //The current movement status of this character.
    [SerializeField] private MovementStatus movementStatus_ = MovementStatus.None;


    private void Awake()
    {
        //if there is no speaker image then use the default image of the character.
        if (!speakerImage_)
            speakerImage_ = GetComponent<SpriteRenderer>().sprite;
    }

    // Use this for initialization
    virtual public void Start ()
    {

        if (!rigidbody_)
            rigidbody_ = GetComponent<Rigidbody2D>();
        if (!rigidbody_)
            Debug.LogError("This Character does not have a Rigidbody2D component.");

        if (!collider_)
            collider_ = GetComponent<Collider2D>();
        if (!collider_)
            Debug.LogError("This Character does not have a Collider component.");
    }

    private void FixedUpdate()
    {


        //do not continue if this character doesnt have a rigidbody.
        if (!rigidbody_ )
            return;

        //dont move character if not in normal game state (dialog, combat, menu etc.)
        if (GlobalGame.Instance.CurrentGameState != GlobalGame.GameState.Normal || GlobalGame.Instance.CurrentPlayerState != GlobalGame.PlayerState.Normal )
        {
            SetMovementStatus(MovementStatus.None);
            rigidbody_.velocity = Vector2.zero;
            return;
        }

        Vector2 targetVelocity = currentMovementSpeed_ * direction_;

        //TODO: Maybe add some drag/acceleration control.
     
        //multiplied by Time.fixedDeltaTime to keep the speed the same independant of physics steps per second and fps.
        rigidbody_.velocity = targetVelocity * Time.fixedDeltaTime;
    }

    public void SetDirection(Vector2 direction)
    {
        if (GlobalGame.Instance.CurrentGameState != GlobalGame.GameState.Normal)
            return;

        //if the provided direction is not a valid direction (e.g. (0, 0)) then do not change the current direction.
        if (direction.sqrMagnitude == 0)
            return;

        //else set the current direction to the normalised vector of direction. (Length of direction vector will be 1).
        direction_ = direction.normalized;
    }

    public void SetMovementStatus(MovementStatus movementStatus)
    {
        //keep movement to None if not in normal mode.
        if (GlobalGame.Instance.CurrentGameState != GlobalGame.GameState.Normal)
            movementStatus = MovementStatus.None;

        //if not changing to a new movement status then stop.
        if (movementStatus_ == movementStatus)
            return;

        movementStatus_ = movementStatus;

        switch(movementStatus_)
        {
            case MovementStatus.None:
                currentMovementSpeed_ = 0;
            break;

            case MovementStatus.Walk:
                currentMovementSpeed_ = walkSpeed_;
            break;
        }

        //Trigger OnMovementStatusChange event
        if(OnMovementStatusChange != null)
            OnMovementStatusChange();
    }

    public MovementStatus CurrentMovementStatus
    {
        get { return movementStatus_; }
    }

    public Vector2 Direction
    {
        get { return direction_; }
    }

    public List<Attack> Attacks
    {
        get { return data_.atacks; }
    }

    public void UnlockTrait(TreeNode tn)
    {
        data_.unlockedTreeSkills.Add(tn.Tree_ID);

        AttacksDatabase attackDatabase = AttacksDatabase.Instance;

        if(!attackDatabase)
        {
            Debug.LogError("No AttackDatabase in this scene or the AttackDatabase has an incorrect tag.");
            return;
        }

        data_.atacks.Add(attackDatabase.GetAttack(tn.Skill_ID));
    }

    public virtual void Damage(int damage)
    {
        //prevent increasing health with negative damage.
        //Could use uint but it would need to be converted to int to take from health_ anyway.
        if (damage <= 0)
        {
            Heal(-damage);
            return;
        }

        data_.hp -= damage;

        if (data_.hp <= 0)
        {
            data_.hp = 0;
            Die();
        }

    }

    public virtual void Heal(int amount)
    {
        if (amount <= 0)
            return;

        data_.hp += amount;
        if (data_.hp > data_.maxHp)
        {
            data_.hp = data_.maxHp;
        }
    }

    public void CharacterTurnStarts()
    {
        if(OnCharacterTurnStart != null)
            OnCharacterTurnStart.Invoke();
    }


    public void CharacterTurnEnds()
    {
        if (OnCharacterTurnEnds != null)
            OnCharacterTurnEnds.Invoke();
    }

    public void InteractWithObjectsInRadius()
    {
        //if in combat then dont handler any interact.
        if (GlobalGame.Instance.CurrentGameState == GlobalGame.GameState.InCombat)
            return;

        //get all colliders in certain radius of this game object.
        Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(transform.position, interactRadius_);

        //find the closest one with a interactable script.
        Collider2D closestCollider = null;
        float closestSqrdDistance = 0;
        foreach (Collider2D collider in collidersInRadius)
        {
            Interactable interactable = collider.GetComponent<Interactable>();

            //if the collider game object has a interactable script and is not the gameobject with the script.
            if (interactable && interactable.isActiveAndEnabled && collider != collider_)
            {
                //calc the sqard distance between this and the current interactable object.
                float sqrdDistance = (transform.position - collider.transform.position).sqrMagnitude;
                //check against current closest.
                if (!closestCollider || sqrdDistance < closestSqrdDistance)
                {
                    closestSqrdDistance = sqrdDistance;
                    closestCollider = collider;
                }
            }
        }

        //at this point if closestCollider is not null then there is atleast on interactable object in range and this is the closest.
        if (closestCollider)
        {
            Interactable interact = closestCollider.GetComponent<Interactable>();

            if (interact.isActiveAndEnabled)
                interact.Interact(gameObject);
        }
    }

    public string Name
    {
        get { return name_; }
    }
    public Attack.Type Type
    {
        get { return data_.type; }
        set { data_.type = value; }
    }

    public List<ItemObject> EquipedItems
    {
        get { return data_.equippedItems; }
    }

    public void AddEquipedItem(ItemObject o)
    {
        data_.equippedItems.Add(o);
        data_.ItemCombinedInt += o.Stats.Intellect;
        data_.ItemCombinedDex += o.Stats.Dexterity;
        data_.ItemCombinedStr += o.Stats.Strenght;
        data_.ItemCombinedLight += o.Stats.Holy;
    }

    public void RemoveEquippedItem(ItemObject o)
    {
        for (int i = 0; i < data_.equippedItems.Count; i++)
        {
            ItemObject ei = data_.equippedItems[i];
            if (o == ei)
            {
                data_.ItemCombinedInt -= ei.Stats.Intellect;
                data_.ItemCombinedDex -= ei.Stats.Dexterity;
                data_.ItemCombinedStr -= ei.Stats.Strenght;
                data_.ItemCombinedLight -= ei.Stats.Holy;
                data_.equippedItems.RemoveAt(i);
                break;
            }
        }
    }

    public int Health
    {
        get { return data_.hp; }
        set
        {
            data_.hp = value;

            if (Health > MaxHealth)
                Health = MaxHealth;

            if (Health <= 0)
                Die();
        }
    }
    public int MaxHealth
    {
        get { return data_.maxHp; }
        set { data_.maxHp = value; }
    }
    public int Xp
    {
        get { return data_.xp; }
        set
        {
            data_.xp = value;
            while (data_.xp > NextLevelXP)
            {
                data_.xp -= NextLevelXP;
                data_.level++;
                LevelUp();
            }


        }
    }

    private void LevelUp()
    {
        NotifcationManager.Instance.AddNotification("Level up");
        data_.intellect++;
        data_.light++;
        data_.maxHp += 20;
        data_.hp = data_.maxHp;
        data_.maxMana += 20;
        data_.mana = data_.maxMana;
        data_.skillPoints += 2;
        data_.strength++;
        data_.dexterity++;

    }

    public int NextLevelXP
    {
        get { return 100*Level + ((Level/2) * 3); }
    }

    public int Level
    {
        get { return data_.level; }
    }
    public int Mana
    {
        get { return data_.mana; }
        set { data_.mana = value; }
    }
    public int MaxMana
    {
        get { return data_.maxMana; }
        set { data_.maxMana = value; }
    }

    public int SkillPoints
    {
        get { return data_.skillPoints; }
        set { data_.skillPoints = value; }
    }

    public int Strength
    {
        get { return data_.strength + data_.ItemCombinedStr; }
        set { data_.strength = value; }
    }
    public int Dexterity
    {
        get { return data_.dexterity + data_.ItemCombinedDex; }
        set { data_.dexterity = value; }
    }
    public int Intellect
    {
        get { return data_.intellect + data_.ItemCombinedInt; }
        set { data_.intellect = value; }
    }
    public int Light
    {
        get { return data_.light + data_.ItemCombinedLight; }
        set { data_.light = value; }
    }
    public Class CharacterClass
    {
        get { return data_.characterClass; }
        set { data_.characterClass = value; }
    }

    public float CritDamageMult
    {
        get { return data_.critDamageMultiplier + data_.ItemCombinedCritMultip;  }
        set { data_.critDamageMultiplier = value; }
    }
    
    public float CritCance
    {
        get { return data_.critChance + data_.ItemCombinedCritChance; }
        set{ data_.critChance = Mathf.Clamp01(value); }
    }

    public bool IsDead
    {
        get { return Health <= 0; }
    }

    private void Die()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        gameObject.SetActive(false);
    }

    public void LoadCharacter()
    {

        string saveFileName = GlobalGame.Instance.Player.Name;

        string pathToThisCharacterFile = Application.dataPath + "/Data/SaveFiles/" + saveFileName + "/" + Name + ".json";

        if(File.Exists(pathToThisCharacterFile))
        {
            LoadCharacterFromFile(pathToThisCharacterFile);
        }

    }

    public bool HasSaveFile()
    {
        string saveFileName = GlobalGame.Instance.Player.Name;

        string pathToThisCharacterFile = Application.dataPath + "/Data/SaveFiles/" + saveFileName + "/" + Name + ".json";

        return File.Exists(pathToThisCharacterFile);

    }

    public void SaveCharacter()
    {
        string saveFileName = GlobalGame.Instance.Player.Name;

        data_.position = transform.position;

        if (!Directory.Exists(Application.dataPath + "/Data/SaveFiles"))
            Directory.CreateDirectory(Application.dataPath + "/Data/SaveFiles");

        if (!Directory.Exists(Application.dataPath + "/Data/SaveFiles/" + saveFileName))
            Directory.CreateDirectory(Application.dataPath + "/Data/SaveFiles/" + saveFileName);

        string pathToThisCharacterFile = Application.dataPath + "/Data/SaveFiles/" + saveFileName + "/" + Name + ".json";

        File.WriteAllText(pathToThisCharacterFile, JsonUtility.ToJson(data_));
    }

    private void ResetCharacter()
    {
        string pathToThisDefaultCharacterFile = Application.dataPath + "/Data/CharacterData/" + "Default_" + Name + ".json";

        if(File.Exists(pathToThisDefaultCharacterFile))
        {
            LoadCharacterFromFile(pathToThisDefaultCharacterFile);
        }  
        else
        {
            Debug.LogError("This character has not default character. Check the name of the character is correct.");
        }
    }

    // expects path to exist.
    private void LoadCharacterFromFile(string path)
    {
        data_ = JsonUtility.FromJson<CharacterData>(File.ReadAllText(path));
        transform.position = data_.position;
    }
    
    public Sprite CombatSpriteLeft
    {
        get { return combatSpriteLeft_; }
    }

    public Sprite CombatSpriteRight
    {
        get { return combatSpriteRight_; }
    }

    public Sprite SpeakerSprite
    {
        get { return speakerImage_; }
    }

    public delegate void MovementStatusHandler();
    public event MovementStatusHandler OnMovementStatusChange;
    public delegate void CharactersTurnsHandler();
    public event CharactersTurnsHandler OnCharacterTurnStart;
    public event CharactersTurnsHandler OnCharacterTurnEnds;


}
