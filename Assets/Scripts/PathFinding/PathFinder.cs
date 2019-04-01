using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PathFinder {

    PathGrid pathGrid;

    public PathFinder(PathGrid grid)
    {
        pathGrid = grid;
    }

    public KeyValuePair<Vector3[], bool> FindPathAstar(Vector3 startPos, Vector3 targetPos)
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;
        PathNode startNode = pathGrid.GetPathNodeFromWorldPos(startPos);
        PathNode targetNode = pathGrid.GetPathNodeFromWorldPos(targetPos);
        startNode.parentNode = startNode;

        if (startNode.walkeable && targetNode.walkeable)
        {
            HeapStruct<PathNode> openSet = new HeapStruct<PathNode>(pathGrid.MaxGridSize);
            HashSet<PathNode> closedSet = new HashSet<PathNode>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                PathNode currentNode = openSet.RemoveFirst();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }
                foreach (PathNode neighbour in pathGrid.GetPathNodeNeighbours(currentNode))
                {
                    if (!neighbour.walkeable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                    int movementCost = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementCost;
                    if (movementCost < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = movementCost;
                        neighbour.huristicCost = GetDistance(neighbour, neighbour);
                        neighbour.parentNode = currentNode;
                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        } else {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }

        if (pathSuccess)
        {
            waypoints = ReCreatePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }
        sw.Stop();
        //UnityEngine.Debug.Log("Needed " + sw.ElapsedMilliseconds + "ms to find and recreate the path.");
        KeyValuePair<Vector3[], bool> returnvalue = new KeyValuePair<Vector3[], bool>(waypoints, pathSuccess);
        return returnvalue;
    }

    private Vector3[] ReCreatePath(PathNode startingNode, PathNode endNode)
    {
        List<PathNode> pathToFollow = new List<PathNode>();
        PathNode node = endNode;
        while (node != startingNode)
        {
            pathToFollow.Add(node);
            node = node.parentNode;
        }
        List<Vector3> waypoints = new List<Vector3>();
        if (pathToFollow.Count > 0) {
            Vector2 oldDirection = Vector2.zero;
            waypoints.Add(pathToFollow[0].nodeWorldPos);
            for (int i = 1; i < pathToFollow.Count; i++)
            {
                Vector2 newDirection = new Vector2(pathToFollow[i - 1].gridXPos - pathToFollow[i].gridXPos, pathToFollow[i - 1].gridYPos - pathToFollow[i].gridYPos);
                if (oldDirection != newDirection)
                {
                    waypoints.Add(pathToFollow[i].nodeWorldPos);
                }
                oldDirection = newDirection;
            }

            if (pathToFollow[pathToFollow.Count - 1].nodeWorldPos != waypoints[waypoints.Count-1])
            {
                waypoints.Add(pathToFollow[pathToFollow.Count-1].nodeWorldPos);
            }
        }

        waypoints.Reverse();
        return waypoints.ToArray();
    }

    private int GetDistance(PathNode nodeA, PathNode nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridXPos - nodeB.gridXPos);
        int dstY = Mathf.Abs(nodeA.gridYPos - nodeB.gridYPos);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);

        return 14 * dstX + 10 * (dstY - dstX);
    }
}
