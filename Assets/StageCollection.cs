using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StageCollection : MonoBehaviour
{

    public UnityEngine.Object targetPrefabFolder;
    public List<Stage> stagePrefabs;
}


#if UNITY_EDITOR
[CustomEditor(typeof(StageCollection))]
public class StageCollectionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(32);

        if (GUILayout.Button("Collect stages")) CollectStages();

    }

    void CollectStages()
    {
        StageCollection sc = (StageCollection)target;
        sc.stagePrefabs.Clear();

        var uids = AssetDatabase.FindAssets("t:Prefab", new string[] { AssetDatabase.GetAssetPath(sc.targetPrefabFolder) });
        foreach (var uid in uids)
        {
            var path = AssetDatabase.GUIDToAssetPath(uid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab.name == "StageEmpty") continue;
            var stage = prefab.GetComponent<Stage>();
            if (stage != null) sc.stagePrefabs.Add(stage);
        }

        sc.stagePrefabs.Sort((s1, s2) => s1.gameObject.name.CompareTo(s2.gameObject.name));
        EditorUtility.SetDirty(target);
    }
} 

#endif