using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public struct PathRequest
{
    public Vector3 startPos;
    public Vector3 endPos;
    public Action<Vector3[], bool> Callback;

    public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> finishedCallBack)
    {
        startPos = start;
        endPos = end;
        Callback = finishedCallBack;
    }
}


public class PathFinderRequestManager : MonoBehaviour {

    static PathFinderRequestManager instance;

    private ConcurrentQueue<CallBackInMainThread> m_queuedCallBacks = new ConcurrentQueue<CallBackInMainThread>();
    private PathFinder m_pathFinder;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        m_pathFinder = new PathFinder(GetComponent<PathGrid>());
    }

    private void Update()
    {
        if (m_queuedCallBacks.Count > 0)
        {
            CallBackInMainThread cb = m_queuedCallBacks.Dequeue();
            cb.Callback(cb.parm1, cb.parm2);
        }
    }

    public static void RequestPath(PathRequest request)
    {
        ThreadStart t = delegate
        {
            instance.ProcessRequest(request);
        };
        t.Invoke();
    }

    private void ProcessRequest(object o)
    {
        PathRequest request = (PathRequest)o;
        KeyValuePair<Vector3[], bool> paths = m_pathFinder.FindPathAstar(request.startPos, request.endPos);
        m_queuedCallBacks.Enqueue(new CallBackInMainThread(paths.Key, paths.Value, request.Callback));
    }

    private struct CallBackInMainThread
    {
        public Vector3[] parm1;
        public bool parm2;
        public Action<Vector3[], bool> Callback;

        public CallBackInMainThread(Vector3[] parmeter1, bool paramenter2, Action<Vector3[], bool> toCall)
        {
            parm1 = parmeter1;
            parm2 = paramenter2;
            Callback = toCall;
        }
    }
}
