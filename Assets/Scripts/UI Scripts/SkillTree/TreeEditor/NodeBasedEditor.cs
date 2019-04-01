using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class NodeBasedEditor : EditorWindow {

    private List<EditorNode> nodes;
    private List<EditorConnection> connections;

    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    private EditorConnectionPoint selectedInPoint;
    private EditorConnectionPoint selectedOutPoint;

    private Rect rectButtonClear;
    private Rect rectButtonSave;
    private Rect rectButtonLoad;

    private Vector2 offset;
    private Vector2 drag;

    [MenuItem("Window/Node Based Editor")]
    private static void OpenWindow()
    {
        NodeBasedEditor window = GetWindow<NodeBasedEditor>();
        window.titleContent = new GUIContent("Node Based Editor");
    }

    private void OnEnable()
    {
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 16, 16);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 16, 16);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 16, 16);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 16, 16);

        rectButtonClear = new Rect(new Vector2(10, 10), new Vector2(60, 20));
        rectButtonSave = new Rect(new Vector2(80, 10), new Vector2(60, 20));
        rectButtonLoad = new Rect(new Vector2(150, 10), new Vector2(60, 20));

        LoadNodes();
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);
        DrawButtons();

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed) Repaint();
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawNodes()
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    ClearConnectionSelection();
                }

                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }

    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].EventProcess(e);

                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.Rect.center,
                e.mousePosition,
                selectedInPoint.Rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.Rect.center,
                e.mousePosition,
                selectedOutPoint.Rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        if (nodes == null)
        {
            nodes = new List<EditorNode>();
        }
        int id = 0;
        foreach(EditorNode n in this.nodes)
        {
            if (n.treeNode.Tree_ID >= id)
            {
                id = n.treeNode.Tree_ID;
                id++;
            }
        }
        nodes.Add(new EditorNode(mousePosition, 200, 100, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, id, false, 0, null));
    }

    private void OnClickInPoint(EditorConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;

        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.EditorNode != selectedInPoint.EditorNode)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickOutPoint(EditorConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null)
        {
            if (selectedOutPoint.EditorNode != selectedInPoint.EditorNode)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickRemoveNode(EditorNode node)
    {
        if (connections != null)
        {
            List<EditorConnection> connectionsToRemove = new List<EditorConnection>();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                connections.Remove(connectionsToRemove[i]);
            }

            connectionsToRemove = null;
        }

        nodes.Remove(node);
    }

    private void OnClickRemoveConnection(EditorConnection connection)
    {
        connections.Remove(connection);
    }

    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<EditorConnection>();
        }

        connections.Add(new EditorConnection(selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    private void SaveTree()
    {
        if(this.nodes.Count > 0)
        {
            TreeNode[] treeNodes = new TreeNode[this.nodes.Count];
            int[] dependencies;
            List<int> dependenciesList = new List<int>();

            for (int i = 0; i < nodes.Count; i++)
            {
                if (this.connections != null)
                {
                    List<EditorConnection> connectionsToRemove = new List<EditorConnection>();
                    List<EditorConnectionPoint> connectionPointsToCheck = new List<EditorConnectionPoint>();
                    for (int j = 0; j < this.connections.Count; j++)
                    {
                        if (this.connections[j].inPoint == this.nodes[i].inPoint)
                        {
                            for (int k = 0; k < this.nodes.Count; k++)
                            {
                                if (this.connections[j].outPoint == this.nodes[k].outPoint)
                                {
                                    dependenciesList.Add(k);
                                    break;
                                }
                            }
                            connectionsToRemove.Add(this.connections[j]);
                            connectionPointsToCheck.Add(this.connections[j].outPoint);
                        }
                    }
                }
                dependencies = dependenciesList.ToArray();
                dependenciesList.Clear();
                treeNodes[i] = this.nodes[i].treeNode;
                treeNodes[i].Skill_Dependency = dependencies;
            }

            //string json = JsonUtility.ToJson(treeNodes, true);
            string json = JsonHelperClass.ToJsonArray<TreeNode>(treeNodes);

            using (FileStream fs = new FileStream("Assets/Resources/treeDatabase.json", FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    writer.Write(json);
                }
            }
            this.SaveNodes();
            UnityEditor.AssetDatabase.Refresh();
        }
    }

    private void SaveNodes()
    {
        EditorNodeData[] nodeData = new EditorNodeData[this.nodes.Count];

        for(int i = 0; i < this.nodes.Count; i++)
        {
            nodeData[i] = new EditorNodeData();
            nodeData[i].id_Node = this.nodes[i].treeNode.Tree_ID;
            nodeData[i].position = this.nodes[i].rect.position;
        }

        //string json = JsonUtility.ToJson(nodeData, true);
        string json = JsonHelperClass.ToJsonArray<EditorNodeData>(nodeData);
        using (FileStream fs = new FileStream("Assets/Resources/EditorTreeData.json", FileMode.Create))
        {
            using (StreamWriter writter = new StreamWriter(fs))
            {
                writter.Write(json);
            }
        }
        UnityEditor.AssetDatabase.Refresh();
    }

    private void ClearNode()
    {
        if (this.nodes != null && this.nodes.Count > 0)
        {
            while (this.nodes.Count > 0)
            {
                OnClickRemoveNode(nodes[0]);
            }
        }
    }

    private void LoadNodes()
    {
        this.ClearNode();
        if(File.Exists("Assets/Resources/EditorTreeData.json")){
            EditorNodeData[] loadedNodeData = JsonHelperClass.getJsonArray<EditorNodeData>(File.ReadAllText("Assets/Resources/EditorTreeData.json"), true);
            TreeNode[] treeNodes;
            List<TreeNode> originalNodes = new List<TreeNode>();
            Dictionary<int, TreeNode> skillDictionary = new Dictionary<int, TreeNode>();
            string JsonData;
            Vector2 pos = Vector2.zero;
            if (File.Exists("Assets/Resources/treeDatabase.json"))
            {
                JsonData = File.ReadAllText("Assets/Resources/treeDatabase.json");
                treeNodes = JsonHelperClass.getJsonArray<TreeNode>(JsonData, true);

                for (int i = 0; i < treeNodes.Length; i++)
                {
                    if (this.nodes == null)
                    {
                        this.nodes = new List<EditorNode>();
                    }

                    for (int j = 0; j < loadedNodeData.Length; j++)
                    {
                        if (loadedNodeData[j].id_Node == treeNodes[i].Tree_ID)
                        {
                            pos = loadedNodeData[j].position;
                            break;
                        }
                    }

                    this.nodes.Add(new EditorNode(pos, 200, 100, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, treeNodes[i].Tree_ID, treeNodes[i].IsUnlocked, treeNodes[i].Cost, treeNodes[i].Skill_Dependency));

                    if (treeNodes[i].Skill_Dependency.Length == 0)
                    {
                        originalNodes.Add(treeNodes[i]);
                    }
                    skillDictionary.Add(treeNodes[i].Tree_ID, treeNodes[i]);
                }

                TreeNode outSkill;
                EditorNode outNode;
                for (int i = 0; i < this.nodes.Count; i++)
                {
                    for (int j = 0; j < this.nodes[i].treeNode.Skill_Dependency.Length; j++)
                    {
                        if (skillDictionary.TryGetValue(this.nodes[i].treeNode.Skill_Dependency[j], out outSkill))
                        {
                            for (int k = 0; k < this.nodes.Count; k++)
                            {
                                if (this.nodes[k].treeNode.Tree_ID == outSkill.Tree_ID)
                                {
                                    outNode = this.nodes[k];
                                    OnClickOutPoint(outNode.outPoint);
                                    break;
                                }
                            }
                            OnClickInPoint(this.nodes[i].inPoint);
                        }
                    }
                }
            }
        }
    }

    private void DrawButtons()
    {
        if (GUI.Button(rectButtonClear, "Clear"))
            this.ClearNode();
        if (GUI.Button(rectButtonSave, "Save"))
            this.SaveTree();
        if (GUI.Button(rectButtonLoad, "Load"))
            this.LoadNodes();
    }
}
