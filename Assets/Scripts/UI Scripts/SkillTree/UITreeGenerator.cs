using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITreeGenerator : MonoBehaviour {
    public GameObject buttonPrefab;
    public GameObject linePrefab;
    public SkillTreeHandler handler;

    // Generate tree ui
    void Start () {
        Debug.Log("Generate tree");

        Dictionary<int, Vector2> positionOfNodes = LoadAllPositionData();
        Vector2 startObjectPosition = gameObject.transform.position;
        Vector2 relativeTreePosition = positionOfNodes[0];
        List<SkillTreeButton> allButtons = new List<SkillTreeButton>();
        //Add ID 0 node
        GameObject firstButton = Instantiate(this.buttonPrefab, startObjectPosition, this.transform.rotation, this.transform.parent);
        firstButton.GetComponent<SkillTreeButton>().treeHandler = this.handler;
        firstButton.GetComponent<SkillTreeButton>().treeNodeId = 0;
        allButtons.Add(firstButton.GetComponent<SkillTreeButton>());

        //Add rest of nodes
        Dictionary<int, TreeNode> skillTree = handler.getTree();
        skillTree.Remove(0);
        foreach(KeyValuePair<int, TreeNode> node in skillTree)
        {
            Vector2 currentNodePosition;
            if (positionOfNodes.TryGetValue(node.Key, out currentNodePosition))
            {
                
                GameObject button = Instantiate(this.buttonPrefab, startObjectPosition + ((currentNodePosition - relativeTreePosition)/3), this.transform.rotation, this.transform.parent);
                button.GetComponent<SkillTreeButton>().treeHandler = this.handler;
                button.GetComponent<SkillTreeButton>().treeNodeId = node.Key;
                allButtons.Add(button.GetComponent<SkillTreeButton>());
            }
        }

        //Generate and attach lines
        foreach(SkillTreeButton skb in allButtons)
        {
            TreeNode treeNode;
            if (skillTree.TryGetValue(skb.treeNodeId, out treeNode))
            {
                int[] dependencies = treeNode.Skill_Dependency;
                foreach(int dependency in dependencies)
                {
                    foreach (SkillTreeButton skbDependency in allButtons)
                    {
                        if (skbDependency.treeNodeId == dependency)
                        {
                            GameObject line = Instantiate(this.linePrefab, skbDependency.gameObject.GetComponent<RectTransform>().position, Quaternion.identity, skbDependency.transform.parent);
                            line.GetComponent<SkillTreeLine>().targetLocation = new Vector2(skb.gameObject.GetComponent<RectTransform>().localPosition.x, skb.gameObject.GetComponent<RectTransform>().localPosition.y);
                            line.transform.SetAsFirstSibling();
                            skbDependency.AddOutLine(line.GetComponent<SkillTreeLine>());
                            skb.AddInLine(line.GetComponent<SkillTreeLine>());
                            break;
                        }
                    }
                }
            }
        }
        Destroy(gameObject);
	}

    private Dictionary<int, Vector2> LoadAllPositionData()
    {
        Dictionary<int, Vector2> positionOfNodes = new Dictionary<int, Vector2>();
        TextAsset NodesDataTextAsset = Resources.Load<TextAsset>("EditorTreeData");
        //Create an array of Items
        EditorNodeData[] NodesData = JsonHelperClass.getJsonArray<EditorNodeData>(NodesDataTextAsset.text, true);
        positionOfNodes.Clear();
        foreach(EditorNodeData en in NodesData)
        {
            if (!positionOfNodes.ContainsKey(en.id_Node))
            {
                positionOfNodes.Add(en.id_Node, en.position);
            } else
            {
                Debug.Log("Position of node with ID: " + en.id_Node + " already exits, ignoring this duplicated position.");
            }
        }
        return positionOfNodes;
    }
}
