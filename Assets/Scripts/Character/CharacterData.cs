using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Class
{
    Mage = 0,
    Ranger = 1,
    Warrior = 2
}

[Serializable]
public class CharacterData
{
    public Vector2 position;

    public Attack.Type type = Attack.Type.Normal;
    public string CurrentMapName = "MainMain";
    public Class characterClass = Class.Mage;
    public int hp = 100;
    public int maxHp = 100;
    public int level = 1;
    public int xp = 0;
    public int mana = 20;
    public int maxMana = 20;
    public int skillPoints = 0;
    public List<int> unlockedTreeSkills = new List<int>();
    public List<Attack> atacks = new List<Attack>();
    public List<ItemObject> equippedItems = new List<ItemObject>();
    public int ItemCombinedStr = 0;
    public int ItemCombinedDex = 0;
    public int ItemCombinedInt = 0;
    public int ItemCombinedLight = 0;
    public float ItemCombinedCritMultip = 0.0f;
    public float ItemCombinedCritChance = 0.0f;

    public int strength = 3;
    public int dexterity = 3;
    public int intellect = 3;
    public int light = 3;
    public float critDamageMultiplier = 1.0f;
    public float critChance = 0.0f;
}
