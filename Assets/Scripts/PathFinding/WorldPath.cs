using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPath {

    private Vector3[] waypoints;
    private Line[] boundries;
    public int PathSize
    {
        get
        {
            return waypoints.Length;
        }
    }

    public WorldPath(Vector3[] path, Vector3 startpos, float turnDistance)
    {
        boundries = new Line[path.Length];
        waypoints = path;

        Vector2 previusNodePos = (Vector2)startpos;
        for (int i = 0; i < path.Length; i++)
        {
            Vector2 currentNodePos = (Vector2)path[i];
            Vector2 dir = (currentNodePos - previusNodePos).normalized;
            Vector2 boundriePoint = currentNodePos - dir * turnDistance;
            if (i == path.Length-1)
            {
                boundriePoint = currentNodePos;
            }
            boundries[i] = new Line(boundriePoint, previusNodePos - dir * turnDistance);
            previusNodePos = boundriePoint;
        }
    }

    public Vector2 GetDirToNodeFrom(int node, Vector2 pos)
    {
        if (node >= waypoints.Length || node < 0)
            return new Vector2(0, 0);
        return ((Vector2)waypoints[node] - pos).normalized;
    }

    public Vector2 GetNodePos(int node)
    {
        if (node >= waypoints.Length || node < 0)
            return new Vector2(0, 0);
        return (Vector2)waypoints[node];
    }

    public bool HasCrossedNode(int node,Vector2 point)
    {
        if (node >= boundries.Length)
            return true;
        return boundries[node].HasCrossedLine(point);
    }

    public void DebugDraw()
    {
        Gizmos.color = Color.black;
        foreach (Vector3 p in waypoints)
        {
            Gizmos.DrawCube(p, Vector3.one);
        }

        Gizmos.color = Color.white;
        foreach (Line l in boundries)
        {
            l.debugDraw(5);
        }
    }

    private class Line
    {
        const float verticalLineGradient = 148;
        private float gradient;
        private Vector2 point_1;
        private Vector2 point_2;
        private float gradientPerpendicular;
        private bool approachSide;

        public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
        {
            float dx = pointOnLine.x - pointPerpendicularToLine.x;
            float dy = pointOnLine.y - pointPerpendicularToLine.y;

            if (dx == 0)
            {
                gradientPerpendicular = verticalLineGradient;
            }
            else
            {
                gradientPerpendicular = dy / dx;
            }

            if (gradientPerpendicular == 0)
            {
                gradient = verticalLineGradient;
            }
            else
            {
                gradient = -1 / gradientPerpendicular;
            }

            point_1 = pointOnLine;
            point_2 = pointOnLine + new Vector2(1, gradient);

            approachSide = false;
            approachSide = GetSide(pointPerpendicularToLine);
        }

        bool GetSide(Vector2 p)
        {
            return (p.x - point_1.x) * (point_2.y - point_1.y) > (p.y - point_1.y) * (point_2.x - point_1.x);
        }

        public bool HasCrossedLine(Vector2 p)
        {
            return GetSide(p) != approachSide;
        }

        public void debugDraw(int u)
        {
            Vector3 lineDir = new Vector3(1, gradient, 0).normalized;
            Vector3 lineCentre = new Vector3(point_1.x, point_1.y, 1);
            Gizmos.DrawLine(lineCentre - lineDir * u / 2f, lineCentre + lineDir * u / 2f);
        }

    }
}
