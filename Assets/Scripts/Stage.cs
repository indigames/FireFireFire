using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Stage : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(Stage))]
public class StageDesignerEditor : Editor
{
    static string BLOCK_PREFAB_FOLDER = "Assets/Prefabs/Blocks";

    new Stage target => (Stage)base.target;
    GenericMenu blockMenu = null;
    bool spawnAsTarget = false;

    private void OnEnable()
    {
        if (blockMenu == null)
        {
            blockMenu = new GenericMenu();
            var structures = new List<string>();
            var prefabs = AssetDatabase.FindAssets("t:Prefab", new string[] { BLOCK_PREFAB_FOLDER });
            foreach (var guid in prefabs)
            {
                var asset_path = AssetDatabase.GUIDToAssetPath(guid);
                var file_name = AssetDatabase.LoadAssetAtPath<GameObject>(asset_path).name;
                blockMenu.AddItem(new GUIContent(file_name), false, SpawnPrefab, asset_path);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(32);

        if (GUILayout.Button("Item", EditorStyles.miniButton))
        {
            spawnAsTarget = false;
            blockMenu.ShowAsContext();
        }
        if (GUILayout.Button("Target", EditorStyles.miniButton))
        {
            spawnAsTarget = true;
            blockMenu.ShowAsContext();
        }

    }

    private void SpawnPrefab(object o_path)
    {
        string path = (string)o_path;
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, target.transform);

        if (spawnAsTarget)
        {
            instance.gameObject.name = "TARGET: " + instance.gameObject.name;
            instance.gameObject.AddComponent<StageTarget>();
        }
        else instance.gameObject.AddComponent<StageItem>();

        var prefabStage = UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
        else
            EditorUtility.SetDirty(target);
    }
}
#endif