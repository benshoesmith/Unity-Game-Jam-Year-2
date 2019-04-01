using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDecisionNode<T>
{
    DecisionTreeStatus Tick(long tickTime, T contex);
}