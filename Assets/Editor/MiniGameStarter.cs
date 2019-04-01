using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MiniGame), true)]
public class MiniGameStarter : Editor {

	public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MiniGame mg = (MiniGame)target;
        if (GUILayout.Button("Start Mini Game"))
        {
            mg.StartMiniGame();
        }
    }
}
