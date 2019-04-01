using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TreeNode {

    public int Tree_ID;
    public int Skill_ID;
    public int[] Skill_Dependency;
    public bool IsUnlocked;
    public int Cost;
}
