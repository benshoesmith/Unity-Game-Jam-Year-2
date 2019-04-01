using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeHandler : MonoBehaviour {

    public Canvas buttonsHolder;
    public Text amountText;
    private Character m_player;
    private Dictionary<int, TreeNode> skillTree = new Dictionary<int, TreeNode>();
    private Vector2 centerPosition;
    private GameObject tooltip;

    // Use this for initialization
    void Awake () {
        LoadSkillTree();
        centerPosition = gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).localPosition;
        tooltip = gameObject.transform.GetChild(0).GetChild(1).gameObject;
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Character>();
    }

    private void Start()
    {
        this.tooltip.SetActive(false);
        GiveAlreadyLearnedToPlayer();
    }

    private void Update()
    {
        //Do not update if it is disabled
        if (!gameObject.activeSelf)
            return;
        amountText.text = m_player.SkillPoints.ToString();
    }

    private void GiveAlreadyLearnedToPlayer()
    {
        foreach(KeyValuePair<int, TreeNode> n in skillTree)
        {
            if (n.Value.IsUnlocked)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<Character>().UnlockTrait(n.Value);
            }
        }
    }

    private void LoadSkillTree()
    {
        Debug.Log("skill tree load");

        //First clear the database
        skillTree.Clear();
        //Load tree data
        //Get the database file
        TextAsset treeTextAsset = Resources.Load<TextAsset>("treeDatabase");
        //Create an array of Items
        TreeNode[] treeNodes = JsonHelperClass.getJsonArray<TreeNode>(treeTextAsset.text, true);
        //Add each item to the dictionary
        foreach (TreeNode node in treeNodes)
        {
            //Check if the skill tree node ID already exits
            if (skillTree.ContainsKey(node.Tree_ID))
            {
                //already exits so dont add it.
                string descartedNode = JsonUtility.ToJson(node);
                Debug.Log("Skill with ID " + node.Tree_ID + " already exits in the database descarting node: " + descartedNode);
                continue;
            }
            //Just in case if the node is null
            if (node != null)
            {
                skillTree.Add(node.Tree_ID, node);
            }
        }
    }

    public void RefreshTreeButtons()
    {

        foreach (Transform b in this.buttonsHolder.transform)
        {
            SkillTreeButton buttonScirpt = b.gameObject.GetComponent < SkillTreeButton>();
            if (buttonScirpt)
            {
                buttonScirpt.UpdateSkillState();
            }
        }
    }

    public void SetTooltipText(string text)
    {
        this.tooltip.transform.GetChild(0).GetComponent<Text>().text = text;
    }

    public void ShowTooltipAt(bool show, Transform where)
    {
        this.tooltip.gameObject.transform.position = where ? where.position : this.transform.position;
        this.tooltip.gameObject.SetActive(show);
    }

    public void CenterSkillTreeAt(Vector2 posToCenter)
    {
        this.buttonsHolder.transform.localPosition = centerPosition - posToCenter;
    }

    public bool IsSkillUnlocked(int TreeNodeID)
    {
        TreeNode returnedNode;
        if (skillTree.TryGetValue(TreeNodeID, out returnedNode))
        {
            return returnedNode.IsUnlocked;
        }
        else
        {
            return false;
        }
    }

    public bool CanSkillBeUnlocked(int treeNodeID)
    {
        bool canBeUnlock = true;
        TreeNode returnedNode;
        if (skillTree.TryGetValue(treeNodeID, out returnedNode))
        {
            if (returnedNode.Cost <= m_player.SkillPoints)
            {
                int[] dependencies = returnedNode.Skill_Dependency;
                for (int i = 0; i < dependencies.Length; ++i)
                {
                    if (skillTree.TryGetValue(dependencies[i], out returnedNode))
                    {
                        if (!returnedNode.IsUnlocked)
                        {
                            canBeUnlock = false;
                            break;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

        }
        else
        {
            return false;
        }

        return canBeUnlock;
    }

    public Dictionary<int, TreeNode> getTree()
    {
        Dictionary<int, TreeNode> copy = new Dictionary<int, TreeNode>(this.skillTree);
        return copy;
    }

    public bool UnlockSkill(int TreeNode_ID)
    {
        TreeNode returnedNode;
        if (skillTree.TryGetValue(TreeNode_ID, out returnedNode))
        {
            if (returnedNode.Cost <= m_player.SkillPoints)
            {
                m_player.SkillPoints -= returnedNode.Cost;
                returnedNode.IsUnlocked = true;

                // We replace the entry on the dictionary with the new one (already unlocked)
                skillTree.Remove(TreeNode_ID);
                skillTree.Add(TreeNode_ID, returnedNode);
                m_player.UnlockTrait(returnedNode);
                return true;
            }
            else
            {
                return false;   // The skill can't be unlocked. Not enough points
            }
        }
        else
        {
            return false;   // The skill doesn't exist
        }
    }

    public TreeNode GetTreeNode(int id)
    {
        TreeNode outNode;
        if (skillTree.TryGetValue(id, out outNode))
        {
            return outNode;
        }
        return null;
    }
}
