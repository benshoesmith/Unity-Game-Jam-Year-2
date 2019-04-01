using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Reflection;

[System.Serializable]
public class Attack
{

    public enum Type
    {
        Normal = 0,
        Magic = 1,
        Fire = 2,
        Water = 3
    }

    #region Variables
    public int ID;
    public string Name;
    public string Description;
    public int ResourceCost;
    public eTargetLimit TargeLimit;
    public Attack.Type AtackType;
    public float BaseDamage;
    public string DamageCalculationFormula;
    public int BaseCritChance;
    public int BaseCritDamageMultiplier;
    #endregion
    private bool m_spellBuild = false;
    private Func<Character, Character, float> m_compiledDamageCalculation;


    public List<KeyValuePair<Character, int>> UsedSpell(Character caster, Character[] targets, float minigameMultiplyer = 1.0f)
    {
        List<KeyValuePair<Character, int>> Damages = new List<KeyValuePair<Character, int>>();
        caster.Mana -= ResourceCost;
        foreach (Character target in targets)
        {
            int final_damage = Mathf.RoundToInt(this.BaseDamage);
            if(m_compiledDamageCalculation != null)
                final_damage += Mathf.RoundToInt(m_compiledDamageCalculation(caster, target));
            final_damage = Mathf.RoundToInt(final_damage * minigameMultiplyer);
            final_damage += CalculateCriticDamage(final_damage, caster.CritCance, caster.CritDamageMult);
            final_damage = Mathf.RoundToInt(final_damage * CombatSystem.Instance.EffectivenessAgainst[caster.Type][target.Type]);
            target.Damage(final_damage);
            Damages.Add(new KeyValuePair<Character, int>(target, final_damage));
        }
        return Damages;
    }

    public int calulateDamageOutputOn(Character caster,Character target)
    {
        int final_damage = Mathf.RoundToInt(this.BaseDamage);
        if (m_compiledDamageCalculation != null)
            final_damage += Mathf.RoundToInt(m_compiledDamageCalculation(caster, target));
        final_damage += CalculateCriticDamage(final_damage, caster.CritCance, caster.CritDamageMult);
        final_damage = Mathf.RoundToInt(final_damage * CombatSystem.Instance.EffectivenessAgainst[caster.Type][target.Type]);
        target.Damage(final_damage);
        return final_damage;
    }

    public void BuildAttack()
    {
        if (m_spellBuild)
            return;

        CompileCalculationScriptSript();
        m_spellBuild = true;
    }

    private int CalculateCriticDamage(int damage, float chance, float CrtiDamageMultiplier)
    {
        if (UnityEngine.Random.value <= (chance + BaseCritChance))
        {
            return Mathf.RoundToInt(damage * (UnityEngine.Random.Range(0.0f, 0.05f) + CrtiDamageMultiplier + Mathf.Clamp01(1-BaseCritDamageMultiplier)));
        }
        return 0;
    }

    private void CompileCalculationScriptSript()
    {
        //If no code was provided create a simple debug code so we know that the item does not have any functionality
        if (this.DamageCalculationFormula.Equals(string.Empty))
        {
            //Use fallback function
            m_compiledDamageCalculation = FallBackNoScriptFunction;
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


            public class JITCCompiledCodeSpells
            {
                public static float CalculateDamage(Character caster, Character target)
                {
            ";
        const string FunctionEnd = @"
                }    
            }";

        this.DamageCalculationFormula = "return " + this.DamageCalculationFormula + ";";

        //Compile the code given
        CompilerResults result = provider.CompileAssemblyFromSource(param, FunctionHeader + this.DamageCalculationFormula + FunctionEnd);

        //Check if compiled correclty
        if (result.Errors.HasErrors)
        {
            //The compiler could not compile the provided code, output the error and use fallback function
            Debug.Log("Got error when compiling the item script: ");
            string msg = "";
            foreach (var error in result.Errors)
            {
                msg += error + "\r\n";
            }
            Debug.Log(msg);
            Debug.Log("Compiling default fall back function");

            //Use fallback function
            m_compiledDamageCalculation = FallBackErrorFunction;
            return;
        }

        //Get the assembly code from the compiler
        Assembly assemblyCode = result.CompiledAssembly;
        //Assign the just created function to a delegate so we can call it later
        m_compiledDamageCalculation = (Func<Character, Character, float>)Delegate.CreateDelegate(typeof(Func<Character, Character, float>), assemblyCode.GetType("JITCCompiledCodeSpells").GetMethod("CalculateDamage"));
    }

    private float FallBackErrorFunction(Character self, Character targets)
    {
        Debug.Log("The damage formla could not get executed, due to compilation erros.SpellID:" + this.ID);
        return 0.0f;
    }
    private float FallBackNoScriptFunction(Character self, Character targets)
    {
        Debug.Log("No damage calculation script!SpellID:" + this.ID);
        return 0.0f;
    }
}
