using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttacksDatabase : MonoBehaviour
{
    private Dictionary<int, Attack> m_allAttacks = new Dictionary<int, Attack>();


    public static AttacksDatabase singleton_ = null;

    public static AttacksDatabase Instance
    {
        get { return singleton_; }
    }

    private void Awake()
    {

        if (!singleton_ || singleton_ == this)
        {
            singleton_ = this;
        }
        else
        {
            Debug.LogError("Multiple instances of the Attack Database have been created. Removing this one from the scene.");
            Destroy(this);
            return;
        }
    }


    private void Start()
    {
        LoadSpells();
        DontDestroyOnLoad(gameObject);
    }

    private void LoadSpells()
    {
        Debug.Log("Loading Spellls data");

        //Clear the data base JIC
        m_allAttacks.Clear();
        //Load Skills json file
        TextAsset AttacksTextAsset = Resources.Load<TextAsset>("skillsDatabase");
        //Create an array of Atacks
        Attack[] attackArray = JsonHelperClass.getJsonArray<Attack>(AttacksTextAsset.text);
        //Add each Atack to the database
        foreach(Attack s in attackArray)
        {
            //Check if the skill already exits
            if (m_allAttacks.ContainsKey(s.ID))
            {
                //already exits so dont add it.
                string descartedNode = JsonUtility.ToJson(s);
                Debug.Log("Attack with ID " + s.ID + " already exits in the database descarting node: " + descartedNode);
                continue;
            }
            //Just in case if the node is null
            if (s != null)
            {
                s.BuildAttack();
                m_allAttacks.Add(s.ID, s);
            }
        }
    }

    public Attack GetAttack(int ID)
    {
        Attack outAtack;
        if (m_allAttacks.TryGetValue(ID, out outAtack))
        {
            return outAtack;
        }
        return null;
    }
}
