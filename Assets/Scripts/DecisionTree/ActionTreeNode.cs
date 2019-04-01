using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTreeNode<T> : IDecisionNode<T> {

    private readonly Func<long, T, DecisionTreeStatus> m_actionToCall;

    public ActionTreeNode(Func<long, T, DecisionTreeStatus> aciton)
    {
        m_actionToCall = aciton;
    }

    public DecisionTreeStatus Tick(long tickTime, T context)
    {
        return m_actionToCall(tickTime, context);
    }
}
