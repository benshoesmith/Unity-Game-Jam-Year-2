using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EditorconnectionPointType { In, Out};
public class EditorConnectionPoint {
    public Rect Rect;
    public EditorconnectionPointType ConnectionType;
    public EditorNode EditorNode;
    public GUIStyle guiStyle;
    public Action<EditorConnectionPoint> onClick;

    public EditorConnectionPoint(EditorNode node, EditorconnectionPointType type, GUIStyle style, Action<EditorConnectionPoint> OnClickConnectionPoint)
    {
        this.Rect = new Rect(0, 0, 10f, 20f);
        this.EditorNode = node;
        this.ConnectionType = type;
        this.guiStyle = style;
        this.onClick = OnClickConnectionPoint;
    }

    public void Draw()
    {
        this.Rect.y = this.EditorNode.rect.y + (this.EditorNode.rect.height * 0.5f) - this.Rect.height * 0.5f;
        switch (this.ConnectionType)
        {
            case EditorconnectionPointType.In:
                this.Rect.x = this.EditorNode.rect.x - this.Rect.width + 8f;
                break;
            case EditorconnectionPointType.Out:
                this.Rect.x = this.EditorNode.rect.x + this.EditorNode.rect.width - 8f;
                break;
        }

        if (GUI.Button(this.Rect, "", this.guiStyle))
        {
            if (this.onClick != null)
            {
                this.onClick(this);
            }
        }
    }
}
