using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillTreeButton : MonoBehaviour, ISelectHandler, IDeselectHandler{

    public int treeNodeId;
    public SkillTreeHandler treeHandler;
    public Sprite unlockedSprite;
    public Sprite NotUnlockedSprite;
    public Sprite UnavailableSprite;
    public Color highlightedColour;

    private Image ButtonImage;
    private Button Button;
    private List<SkillTreeLine> outboundLines = new List<SkillTreeLine>();
    private List<SkillTreeLine> inboundLines = new List<SkillTreeLine>();

    private void Awake()
    {
        this.ButtonImage = this.gameObject.GetComponent<Image>();
        this.Button = this.gameObject.GetComponent<Button>();
        this.Button.onClick.AddListener(this.BuySkill);
    }

    void Start()
    {
        this.UpdateSkillState();
    }

    public void AddOutLine(SkillTreeLine outLine)
    {
        this.outboundLines.Add(outLine);
    }

    public void AddInLine(SkillTreeLine inLine)
    {
        this.inboundLines.Add(inLine);
    }

    public void UpdateSkillState()
    {
        if (this.treeHandler.IsSkillUnlocked(this.treeNodeId))
        {
            this.ButtonImage.sprite = this.unlockedSprite;
            foreach (SkillTreeLine skl in this.inboundLines)
            {
                if(skl != null)
                    skl.SetLineStatus(SkillTreeLine.eLineStatus.Unlocked);
            }
            foreach (SkillTreeLine skl in this.outboundLines)
            {
                if (skl != null)
                    skl.SetLineStatus(SkillTreeLine.eLineStatus.Enabled);
            }
        }
        else if (!this.treeHandler.CanSkillBeUnlocked(this.treeNodeId))
        {
            this.ButtonImage.sprite = this.UnavailableSprite;
        }
        else
        {
            this.ButtonImage.sprite = this.NotUnlockedSprite;
        }
    }

    public void BuySkill()
    {
        if (!this.treeHandler.CanSkillBeUnlocked(this.treeNodeId))
            return;
        if (this.treeHandler.UnlockSkill(this.treeNodeId))
        {
            treeHandler.RefreshTreeButtons();
        }

        Button.Select();
    }

    public void OnSelect(BaseEventData eventData)
    {
        AttacksDatabase ad = AttacksDatabase.Instance;
        TreeNode node = this.treeHandler.GetTreeNode(this.treeNodeId);
        Attack attack = ad.GetAttack(node.Skill_ID);
        if (node == null || attack == null)

        ButtonImage.color = this.highlightedColour;
        this.treeHandler.CenterSkillTreeAt(gameObject.transform.localPosition);
        string tooltipText = "<color=#00ff00ff><b>" + attack.Name + "</b></color>" + "\n\n" + attack.Description + "\n\nUNLOCKED";
        if (!node.IsUnlocked) { 
            tooltipText = "<color=#800000ff><b>" + attack.Name + "</b></color>" + "\n\n" + attack.Description + "\n\nCosts: " + node.Cost + " skillpoints" + "\n\n";
            if (node.Skill_Dependency.Length > 0) {
                tooltipText += "Requirements:\n";
                foreach (int dependency in node.Skill_Dependency)
                {
                    TreeNode dependencyNode = this.treeHandler.GetTreeNode(dependency);
                    Attack dependencyAttack = ad.GetAttack(dependencyNode.Skill_ID);
                    if (node == null || dependencyAttack == null)
                        continue;
                    if (dependencyNode.IsUnlocked)
                        tooltipText += "<color=#00ff00ff>";
                    else
                        tooltipText += "<color=#ff0000ff>";
                    tooltipText += dependencyAttack.Name;
                    if (dependencyNode.IsUnlocked)
                        tooltipText += "</color> - Unlocked";
                    else
                        tooltipText += "</color>- Not Unlocked";
                    tooltipText += "\n\n";
                }
            }
            if(this.treeHandler.CanSkillBeUnlocked(this.treeNodeId))
            {
                tooltipText += "<i>Press Enter to Unlock.</i>";
            } else
            {
                tooltipText += "<i>You dont have enough skillpoints to unlock this spell.</i>";
            }
        }
        this.treeHandler.SetTooltipText(tooltipText);
        this.treeHandler.ShowTooltipAt(true, this.transform);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        this.treeHandler.ShowTooltipAt(false,null);
        ButtonImage.color = Color.white;
    }
}
