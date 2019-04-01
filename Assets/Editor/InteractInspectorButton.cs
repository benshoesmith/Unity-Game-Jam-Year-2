using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Interactable), true)]
public class InteractInspectorButton : Editor {

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Interactable inter = (Interactable)target;
        if (GUILayout.Button("Interact"))
        {
            inter.Interact(null);
        }
    }
}
