using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(VoidEventChannel), editorForChildClasses: true)]
public class VoidEventChannelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = Application.isPlaying;

        var e = target as VoidEventChannel;
        if (GUILayout.Button($"Raise {e.name}"))
            e.OnEventRaised();
    }
}