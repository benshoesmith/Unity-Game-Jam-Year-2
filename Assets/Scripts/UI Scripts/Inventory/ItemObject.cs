using System;
using Microsoft.CSharp;
using UnityEngine;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Collections.Generic;

public enum eItemType
{ 
    EQUIPMENT, CONSUMABLE, TRASH
}

public enum eItemRarity
{
    COMMON, UNCOMMON, RARE, EPIC, LEGENDARY
}

public enum eTargetLimit
{
    SELF_ONLY, ONLY_ALLIES, ONLY_ALIES_NO_SELF, ONLY_ENEMIES, ALL, NO_TARGET
}

public enum eEquipmentType
{
    HEAD, CHEST, HANDS, LEGS, FEET, INVALID
}

[System.Serializable]
public class ItemsStats
{
    public int Strenght;
    public int Dexterity;
    public int Intellect;
    public int Holy;
    public eEquipmentType EquipmentType;

    public bool GotStats()
    {
        return this.Strenght > 0 || this.Dexterity > 0 || this.Intellect > 0 || this.Holy > 0;
    }
}

[System.Serializable]
public class ItemObject{
    #region Variables
    public int ID;
    public eItemType Type;
    public eItemRarity Rarity;
    public ItemsStats Stats;
    public System.UInt32 MaxStackSize;
    public string Title;
    public string UsageDescription;
    public string Description;
    public eTargetLimit TargetingLimit;
    public string UsageScript;
    public Sprite Sprite { get; private set; }
    public bool IsStackeable { get { return this.MaxStackSize > 1; } }
    public string Slug;
    #endregion
    private Action<Character, Character, Character[], Character[]> m_usageFunction;
    private bool m_builded = false;


    //Constructors
    public ItemObject()
    {
        ID = -1;
    }

    public void BuildItem()
    {
        if (m_builded)
            return;
        this.Sprite = Resources.Load<Sprite>("InventoryIcons/" + this.Slug);
        CompileUsageScript();
        m_builded = true;
    }

    public void UseItem(Character self, Character target, Character[] enemy_team, Character[] ally_team)
    {
        m_usageFunction(self, target, enemy_team, ally_team);
    }

    private void CompileUsageScript()
    {
        //If no code was provided create a simple debug code so we know that the item does not have any functionality
        if (this.UsageScript.Equals(string.Empty))
        {
            //Use fallback function
            m_usageFunction = FallBackNoScriptFunction;
            return;
        }

        //Create the CSharCodeProvider with the version 2.0
        CSharpCodeProvider provider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v2.0" } });
        //Create the compiler parameters so it does not generate a exetubale and loads the script only into memory
        CompilerParameters param = new CompilerParameters
        {
            GenerateExecutable = false,       // Create a dll
            GenerateInMemory = true,          // Create it in memory
            WarningLevel = 3,                 // Default warning level
            CompilerOptions = "/optimize",    // Optimize code
            TreatWarningsAsErrors = false     // Better be false to avoid break in warnings
        };

        //Add all references the current script is using
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            param.ReferencedAssemblies.Add(assembly.Location);
        }

        //Constant string to define how the class and function will look like, as in the .json only the function body will be provided
        const string FunctionHeader = @"
            using UnityEngine;


            public class JITCCompiledCode
            {
                public static void UseItem(Character self, Character target, Character[] enemy_team, Character[] ally_team)
                {
            ";
        const string FunctionEnd = @"
                }    
            }";

        //Compile the code given
        CompilerResults result = provider.CompileAssemblyFromSource(param, FunctionHeader + this.UsageScript + FunctionEnd);

        //Check if compiled correclty
        if (result.Errors.HasErrors)
        {
            //The compiler could not compile the provided code, output the error and use fallback function
            Debug.Log("Got error when compiling the item script (ID: "+ID+"): ");
            string msg = "";
            foreach (var error in result.Errors)
            {
                msg += error + "\r\n";
            }
            Debug.Log(msg);
            Debug.Log("Compiling default fall back function");

            //Use fallback function
            m_usageFunction = FallBackErrorFunction;
            return;
        }

        //Get the assembly code from the compiler
        Assembly assemblyCode = result.CompiledAssembly;
        //Assign the just created function to a delegate so we can call it later
        m_usageFunction = (Action<Character, Character, Character[], Character[]>)Delegate.CreateDelegate(typeof(Action<Character, Character, Character[], Character[]>), assemblyCode.GetType("JITCCompiledCode").GetMethod("UseItem"));
    }

    private void FallBackErrorFunction(Character self, Character target, Character[] enemy_team, Character[] ally_team)
    {
        Debug.Log("No function got executed due to compilation erros. ItemID:" + this.ID);
    }
    private void FallBackNoScriptFunction(Character self, Character target, Character[] enemy_team, Character[] ally_team)
    {
        Debug.Log("No usage script provided! ItemID:" + this.ID);
    }

    public string BuildTooltip()
    {
        string finalString = "<color=#";
        switch (this.Rarity)
        {
            case eItemRarity.COMMON:
                finalString += "ffffffff";
                break;
            case eItemRarity.UNCOMMON:
                finalString += "1eff00ff";
                break;
            case eItemRarity.RARE:
                finalString += "0070ddff";
                break;
            case eItemRarity.EPIC:
                finalString += "a335eeff";
                break;
            case eItemRarity.LEGENDARY:
                finalString += "ff8000ff";
                break;
        }
        finalString += "><b>" + this.Title + "</b></color>\n\n";
        if (string.Empty != this.UsageDescription)
        {
            finalString += "<color=#08ff00ff>Use: " + this.UsageDescription + "</color>\n\n";
        }

        if (this.Stats.GotStats())
        {
            if (this.Stats.Strenght > 0)
                finalString += "<color=#000000ff>+" + this.Stats.Strenght + " Strenght</color>\n";
            if(this.Stats.Intellect > 0)
                finalString += "<color=#000000ff>+" + this.Stats.Intellect + " Intellect</color>\n";
            if(this.Stats.Dexterity > 0)
                finalString += "<color=#000000ff>+" + this.Stats.Dexterity + " Dexterity</color>\n";
            if(this.Stats.Holy > 0)
                finalString += "<color=#000000ff>+" + this.Stats.Holy + " Holy</color>\n";
            finalString += "\n";
        }

        finalString += "<color=#000000ff>" + this.Description + "</color>\n";
        return finalString;
    }
}
