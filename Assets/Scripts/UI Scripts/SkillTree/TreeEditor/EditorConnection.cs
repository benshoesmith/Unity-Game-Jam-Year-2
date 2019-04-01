using System;
using UnityEditor;
using UnityEngine;

public class EditorConnection {
    public EditorConnectionPoint inPoint;
    public EditorConnectionPoint outPoint;
    public Action<EditorConnection> OnClickRemoveConnection;

    public EditorConnection(EditorConnectionPoint inPoint, EditorConnectionPoint outPoint, Action<EditorConnection> OnClickRemoveConnection)
    {
        this.inPoint = inPoint;
        this.outPoint = outPoint;
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw()
    {
        Handles.DrawBezier(inPoint.Rect.center, outPoint.Rect.center, inPoint.Rect.center + Vector2.left * 50f,outPoint.Rect.center - Vector2.left * 50f,
            Color.white, null, 2f);

        if (Handles.Button((inPoint.Rect.center + outPoint.Rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            if (OnClickRemoveConnection != null)
            {
                OnClickRemoveConnection(this);
            }
        }
    }
}
