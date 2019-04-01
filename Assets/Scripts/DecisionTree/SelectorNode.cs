using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectorTreeNode<T> : IDecisionNode<T>
{

    private readonly IDecisionNode<T>[] m_nodes;
    private IDecisionNode<T> m_CurrentRunningNode;

    public SelectorTreeNode(IDecisionNode<T>[] allNodes)
    {
        m_nodes = allNodes;
    }

    public DecisionTreeStatus Tick(long TickTime, T context)
    {
        IEnumerable<IDecisionNode<T>> searchNodes = (m_CurrentRunningNode == null) ? m_nodes : m_nodes.SkipWhile(node => node != m_CurrentRunningNode);

        DecisionTreeStatus accumulator = DecisionTreeStatus.Failure;
        foreach (IDecisionNode<T> currentNode in searchNodes)
        {
            if (accumulator != DecisionTreeStatus.Success)
            {
                DecisionTreeStatus NodeDecisionResult = currentNode.Tick(TickTime, context);
                if (NodeDecisionResult == DecisionTreeStatus.Running)
                    m_CurrentRunningNode = currentNode;
                else
                    m_CurrentRunningNode = null;

                accumulator = NodeDecisionResult;
            }
        }

        return accumulator;
    }
}