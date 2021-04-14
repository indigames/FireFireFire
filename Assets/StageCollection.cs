using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class StageCollection : MonoBehaviour
{

    [HideInInspector]
    public UnityEngine.Object targetPrefabFolder;
    [HideInInspector]
    public int startingStage;
    public List<Stage> stagePrefabs;
    int index;

    public Stage CurrentStage => GetCurrentStage();
    public Stage NextStage => GetNextStage();
    public int StageNumber => StageIndex + 1;
    public int StageIndex => index;

    private void Awake()
    {
        index = startingStage;
    }

    private Stage GetCurrentStage()
    {
        return stagePrefabs[index];
    }

    private Stage GetNextStage()
    {
        index = (index + 1) % stagePrefabs.Count;
        return stagePrefabs[index];
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(StageCollection))]
public class StageCollectionEditor : Editor
{
    string[] stageNames = new string[0];

    void ResetStageNames()
    {
        List<string> names = new List<string>();
        StageCollection sc = (StageCollection)target;
        foreach (var stage in sc.stagePrefabs)
            names.Add(stage.gameObject.name);
        stageNames = names.ToArray();
        sc.startingStage = Mathf.Min(sc.startingStage, names.Count - 1);
    }

    private void OnEnable()
    {
        ResetStageNames();
    }

    public override void OnInspectorGUI()
    {
        StageCollection sc = (StageCollection)target;
        base.OnInspectorGUI();

        GUILayout.Space(32);
        sc.startingStage = EditorGUILayout.Popup("Starting stage", sc.startingStage, stageNames);
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
        ResetStageNames();
        EditorUtility.SetDirty(target);
    }
} 

#endif