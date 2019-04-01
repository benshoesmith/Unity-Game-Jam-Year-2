using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : IHeapItem<PathNode>
{
    //Variables to store peerNode
    public bool walkeable;
    public Vector3 nodeWorldPos;
    public int movementCost;
    public int huristicCost;
    public int gCost;
    public PathNode parentNode;
    public int gridXPos;
    public int gridYPos;

    public PathNode(bool walkable, Vector3 nodeworldPos, int gridXPos, int gridYPos, int movCost)
    { 
        this.walkeable = walkable;
        this.nodeWorldPos = nodeworldPos;
        this.gridXPos = gridXPos;
        this.gridYPos = gridYPos;
        this.movementCost = movCost;
    }

    public int fCost
    {
        get
        {
            return gCost + huristicCost;
        }
    }

    private int heapIndex;
    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(PathNode other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if (compare == 0)
        {
            compare = huristicCost.CompareTo(other.huristicCost);
        }
        return -compare;
    }
}
