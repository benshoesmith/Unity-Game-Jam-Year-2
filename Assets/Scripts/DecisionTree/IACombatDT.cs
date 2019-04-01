using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Character))]
public class IACombatDT : MonoBehaviour
{
    private SelectorTreeNode<TreeContext> m_IATree;
    private int m_currentTurn = 0;

    private void Awake()
    {
        GetComponent<Character>().OnCharacterTurnStart += IACombatStarts;
        //Generate the desicion tree

        SelectorTreeNode<TreeContext> healSequencer = new SelectorTreeNode<TreeContext>(
            new IDecisionNode<TreeContext>[] {
                new SequenceTreeNode<TreeContext>(
                    new IDecisionNode<TreeContext>[] {
                        new ActionTreeNode<TreeContext>(NoEnemysHasLow),
                        new ActionTreeNode<TreeContext>(NeedHealing),
                        new ActionTreeNode<TreeContext>(SelectHealingSpell),
                        new ActionTreeNode<TreeContext>(UseAttack)
                    }
                ),
                new SelectorTreeNode<TreeContext>(
                        new IDecisionNode<TreeContext>[]
                        {
                            new SelectorTreeNode<TreeContext>(
                                    new IDecisionNode<TreeContext>[]
                                    {
                                        new ActionTreeNode<TreeContext>(TargetLowHP),
                                        new ActionTreeNode<TreeContext>(TargetRandomAliveEnemy)
                                    }
                                ),
                            new ActionTreeNode<TreeContext>(UseOneShotSpell),
                            new ActionTreeNode<TreeContext>(UseRandomSpell),
                            new ActionTreeNode<TreeContext>(UseFirstAvailabeSpell),
                        }
                    )
            }
        );

        m_IATree = new SelectorTreeNode<TreeContext>(new IDecisionNode<TreeContext>[] { healSequencer });
    }


    private void IACombatStarts()
    {
        TreeContext ctx = new TreeContext();
        DecisionTreeStatus heal = m_IATree.Tick(m_currentTurn, ctx);
    }

    //IDS Attack: 9 6 5 4
    //Action functions
    private DecisionTreeStatus UseOneShotSpell(long TickTime, TreeContext context)
    {
        context.attack = null;
        Attack[] attacks = new Attack[4] { AttacksDatabase.Instance.GetAttack(9), AttacksDatabase.Instance.GetAttack(6), AttacksDatabase.Instance.GetAttack(5), AttacksDatabase.Instance.GetAttack(4) };
        foreach(Attack a in attacks)
        {
            int cadps = a.calulateDamageOutputOn(context.self, context.target);
            if (context.self.Mana >= a.ResourceCost && cadps >= context.target.Health)
            {
                context.attack = a;
                break;
            }
        }

        return (context.attack != null) ? DecisionTreeStatus.Success : DecisionTreeStatus.Failure;
    }

    private DecisionTreeStatus UseRandomSpell(long TickTime, TreeContext context)
    {
        Attack[] attacks = new Attack[4] { AttacksDatabase.Instance.GetAttack(9), AttacksDatabase.Instance.GetAttack(6), AttacksDatabase.Instance.GetAttack(5), AttacksDatabase.Instance.GetAttack(4) };

        context.attack = attacks[UnityEngine.Random.Range(0, attacks.Length)];

        if (context.attack.ResourceCost > context.self.Mana)
        {
            context.attack = null;
        }

        return (context.attack != null) ? DecisionTreeStatus.Success : DecisionTreeStatus.Failure;
    }

    private DecisionTreeStatus UseFirstAvailabeSpell(long TickTime, TreeContext context)
    {
        Attack[] attacks = new Attack[5] { AttacksDatabase.Instance.GetAttack(9), AttacksDatabase.Instance.GetAttack(6), AttacksDatabase.Instance.GetAttack(5), AttacksDatabase.Instance.GetAttack(4), AttacksDatabase.Instance.GetAttack(10) };
        foreach(Attack a in attacks)
        {
            if (a.ResourceCost <= context.self.Mana)
            {
                context.attack = a;
                break;
            }
        }

        return (context.attack != null) ? DecisionTreeStatus.Success : DecisionTreeStatus.Failure;
    }

    private DecisionTreeStatus NeedHealing(long TickTime, TreeContext context)
    {
        if ((context.self.MaxHealth - context.self.Health) < (context.self.MaxHealth * 0.3f))
        {
            return DecisionTreeStatus.Success;
        }
        return DecisionTreeStatus.Failure;
    }

    private DecisionTreeStatus NoEnemysHasLow(long TickTime, TreeContext context)
    {
        bool sucees = false;
        foreach (Character c in context.enemies)
        {
            if (c.Health <= (c.MaxHealth * 0.25f))
            {
                sucees = true;
                break;
            }
        }

        return sucees ? DecisionTreeStatus.Failure : DecisionTreeStatus.Success;
    }

    private DecisionTreeStatus TargetRandomAliveEnemy(long TickTime, TreeContext context)
    {

        context.target = context.enemies[UnityEngine.Random.Range(0, context.enemies.Length)];

        return DecisionTreeStatus.Success;
    }

    private DecisionTreeStatus TargetLowHP(long TickTime, TreeContext context)
    {
        bool sucees = false;
        Character LowerHP = null;
        foreach (Character c in context.enemies)
        {
            if (c.Health <= (c.MaxHealth * 0.25f))
            {
                sucees = true;
                if (LowerHP && LowerHP.Health > c.Health)
                {
                    LowerHP = c;
                } else
                {
                    LowerHP = c;
                }
            }
        }

        context.target = LowerHP;

        return sucees ? DecisionTreeStatus.Success : DecisionTreeStatus.Failure;
    }

    private DecisionTreeStatus SelectHealingSpell(long TickTime, TreeContext context)
    {
        context.target = context.self;

        Attack reju = AttacksDatabase.Instance.GetAttack(7);
        Attack prayer = AttacksDatabase.Instance.GetAttack(8);

        int rejuHeal = reju.calulateDamageOutputOn(context.self, context.self);
        int prayerHeal = reju.calulateDamageOutputOn(context.self, context.self);

        int missingHP = (context.self.MaxHealth - context.self.Health);
        rejuHeal = Math.Min(rejuHeal, missingHP);
        prayerHeal = Math.Min(prayerHeal, missingHP);

        int ManaPerHeltReju = reju.ResourceCost / rejuHeal;
        int ManaPerHeltPrayer = prayer.ResourceCost / prayerHeal;

        context.attack = (ManaPerHeltReju < ManaPerHeltPrayer) ? reju : prayer;

        if (missingHP >= context.self.MaxHealth*0.3f)
        {
            context.attack = prayer;
        }

        int IAMana = context.self.Mana;
        if (context.attack.ResourceCost > IAMana)
        {
            context.attack = (context.attack == prayer) ? reju : null;
        }

        if (reju.ResourceCost > IAMana && prayer.ResourceCost > IAMana)
        {
            return DecisionTreeStatus.Running;
        }

        return DecisionTreeStatus.Success;
    }

    private DecisionTreeStatus UseAttack(long TickTime, TreeContext context)
    {
        if (context.attack != null)
        {
            CombatSystem.Instance.AttackCharacter(context.attack, new Character[] { context.target });
        }
        return DecisionTreeStatus.Success;
    }

    public class TreeContext
    {
        public readonly Character self;
        public readonly Character ally;
        public readonly Character[] enemies;

        public Character target;
        public Attack attack;
    };
}
