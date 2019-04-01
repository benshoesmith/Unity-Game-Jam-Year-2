using System;
using System.Text;
using UnityEditor;
using UnityEngine;

public class EditorNode
{
    public Rect rect;
    public string title;
    public bool isDragged;
    public bool isSelected;

    public EditorConnectionPoint inPoint;
    public EditorConnectionPoint outPoint;

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;

    public Action<EditorNode> onRemove;

    public Rect rectID;

    public Rect rectAttackIDLable;
    public Rect rectAttackID;
    public Rect rectUnlockLabel;
    public Rect rectUnlocked;
    public Rect rectCostLabel;
    public Rect rectCost;

    public GUIStyle styleID;
    public GUIStyle styleField;

    public TreeNode treeNode;
    private bool unlocked = false;
    private StringBuilder nodeTitle;

    public EditorNode(Vector2 position, float width, float height, GUIStyle nodeStyle, GUIStyle selectedStyle, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<EditorConnectionPoint> OnClickInPoint, Action<EditorConnectionPoint> OnClickOutPoint, Action<EditorNode> onRemoveClick, int id, bool unlocked, int cost, int[] dependencies)
    {
        this.rect = new Rect(position, new Vector2(width, height));
        this.style = nodeStyle;
        this.inPoint = new EditorConnectionPoint(this, EditorconnectionPointType.In, inPointStyle, OnClickInPoint);
        this.outPoint = new EditorConnectionPoint(this, EditorconnectionPointType.Out, outPointStyle, OnClickOutPoint);
        this.defaultNodeStyle = nodeStyle;
        this.selectedNodeStyle = selectedStyle;
        this.onRemove = onRemoveClick;

        // Create new Rect and GUIStyle for our title and custom fields
        float rowHeight = height / 7;

        this.rectID = new Rect(position.x, position.y + rowHeight, width, rowHeight);
        this.styleID = new GUIStyle();
        this.styleID.alignment = TextAnchor.UpperCenter;

        this.rectAttackIDLable = new Rect(position.x, position.y + 2 * rowHeight, width / 2, rowHeight);
        this.rectAttackID = new Rect(position.x + width / 2, position.y + 2 * rowHeight, 30, rowHeight);

        this.rectUnlocked = new Rect(position.x + width / 2,
            position.y + 3 * rowHeight, width / 2, rowHeight);

        this.rectUnlockLabel = new Rect(position.x,
            position.y + 3 * rowHeight, width / 2, rowHeight);

        this.styleField = new GUIStyle();
        this.styleField.alignment = TextAnchor.UpperRight;

        this.rectCostLabel = new Rect(position.x,
            position.y + 4 * rowHeight, width / 2, rowHeight);

        this.rectCost = new Rect(position.x + width / 2,
            position.y + 4 * rowHeight, 20, rowHeight);

        this.unlocked = unlocked;

        this.treeNode = new TreeNode();
        this.treeNode.Tree_ID = id;
        this.treeNode.IsUnlocked = unlocked;
        this.treeNode.Cost = cost;
        this.treeNode.Skill_Dependency = dependencies;

        this.nodeTitle = new StringBuilder();
        this.nodeTitle.Append("ID: ");
        this.nodeTitle.Append(id);
    }

    public void Drag(Vector2 dragDelta)
    {
        this.rect.position += dragDelta;
        this.rectID.position += dragDelta;
        this.rectUnlocked.position += dragDelta;
        this.rectUnlockLabel.position += dragDelta;
        this.rectCost.position += dragDelta;
        this.rectCostLabel.position += dragDelta;
        this.rectAttackID.position += dragDelta;
        this.rectAttackIDLable.position += dragDelta;
    }

    public void Draw()
    {
        this.inPoint.Draw();
        this.outPoint.Draw();
        GUI.Box(this.rect, this.title, this.style);

        GUI.Label(this.rectID, this.nodeTitle.ToString(), styleID);
        if (GUI.Toggle(this.rectUnlocked, this.unlocked, ""))
        {
            this.unlocked = true;
        } else
        {
            this.unlocked = false;
        }

        GUI.Label(this.rectAttackIDLable, "Attack ID: ", styleField);
        this.treeNode.Skill_ID = int.Parse(GUI.TextField(this.rectAttackID, this.treeNode.Skill_ID.ToString()));

        GUI.Label(this.rectUnlockLabel, "Unlocked: ", styleField);
        this.treeNode.IsUnlocked = this.unlocked;

        GUI.Label(this.rectCostLabel, "Cost: ", styleField);
        this.treeNode.Cost = int.Parse(GUI.TextField(rectCost, this.treeNode.Cost.ToString()));
    }

    public bool EventProcess(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0)
                {
                    if (this.rect.Contains(e.mousePosition))
                    {
                        this.isDragged = true;
                        GUI.changed = true;
                        this.isSelected = true;
                        this.style = selectedNodeStyle;
                    }
                    else
                    {
                        GUI.changed = true;
                        this.isSelected = false;
                        this.style = defaultNodeStyle;
                    }
                }

                if (e.button == 1 && this.isSelected && this.rect.Contains(e.mousePosition))
                {
                    this.ProcessContextMenu();
                    e.Use();
                }
                break;
            case EventType.mouseUp:
                this.isDragged = false;
                break;
            case EventType.mouseDrag:
                if (e.button == 0 && this.isDragged)
                {
                    this.Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
        return false;
    }

    private void ClickRemoveNode()
    {
        if (this.onRemove != null)
        {
            onRemove(this);
        }
    }

    private void ProcessContextMenu()
    {
        GenericMenu gm = new GenericMenu();
        gm.AddItem(new GUIContent("Eliminate node"), false, this.ClickRemoveNode);
        gm.ShowAsContext();
    }
}
