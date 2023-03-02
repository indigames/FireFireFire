using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BoolEventChannel), editorForChildClasses: true)]
public class BoolEventChannelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUI.enabled = Application.isPlaying;

        var e = target as BoolEventChannel;
        if (GUILayout.Button($"Raise {e.name}"))
            e.RaiseEvent(e.isActive);
    }
}