using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class Stage : MonoBehaviour
{
    public int IDNumber;
    public float verticalOffset = 0;

    public Vector2Int ItemUseMinMax;

    public List<StageItem> stageItems = new();
    public List<StageObstacle> StageObstacles = new();
    public StageTarget stageTarget;

#if UNITY_EDITOR
    private void OnValidate()
    {
        List<StageItem> stageItems = this.transform.GetComponentsInChildren<StageItem>().ToList();
        if (stageItems.Count != this.stageItems.Count)
        {
            this.stageItems.Clear();
            this.stageItems.AddRange(stageItems);
        }

        List<StageObstacle> stageObstacles = this.transform.GetComponentsInChildren<StageObstacle>().ToList();
        if (StageObstacles.Count != stageObstacles.Count)
        {
            this.StageObstacles.Clear();
            this.StageObstacles.AddRange(stageObstacles);
        }

        StageTarget stageTarget = this.transform.GetComponentInChildren<StageTarget>();
        if (!this.stageTarget)
        {
            this.stageTarget = stageTarget;
        }
        UnityEditor.EditorUtility.SetDirty(this);


    }
#endif
}


#if UNITY_EDITOR
[CustomEditor(typeof(Stage))]
public class StageDesignerEditor : Editor
{
    static string BLOCK_PREFAB_FOLDER = "Assets/Prefabs/Blocks";
    static string TARGET_PREFAB_FOLDER = "Assets/Prefabs/Targets";
    static string OBSTACLE_PREFAB_FOLDER = "Assets/Prefabs/Obstacles";

    new Stage target => (Stage)base.target;
    GenericMenu blockMenu = null;
    GenericMenu targetMenu = null;
    GenericMenu obstacleMenu = null;
    ObjectType objectType;

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

        if (targetMenu == null)
        {
            targetMenu = new GenericMenu();
            var structures = new List<string>();
            var prefabs = AssetDatabase.FindAssets("t:Prefab", new string[] { TARGET_PREFAB_FOLDER });
            foreach (var guid in prefabs)
            {
                var asset_path = AssetDatabase.GUIDToAssetPath(guid);
                var file_name = AssetDatabase.LoadAssetAtPath<GameObject>(asset_path).name;
                targetMenu.AddItem(new GUIContent(file_name), false, SpawnPrefab, asset_path);
            }
        }

        if (obstacleMenu == null)
        {
            obstacleMenu = new GenericMenu();
            var structures = new List<string>();
            var prefabs = AssetDatabase.FindAssets("t:Prefab", new string[] { OBSTACLE_PREFAB_FOLDER });
            foreach (var guid in prefabs)
            {
                var asset_path = AssetDatabase.GUIDToAssetPath(guid);
                var file_name = AssetDatabase.LoadAssetAtPath<GameObject>(asset_path).name;
                obstacleMenu.AddItem(new GUIContent(file_name), false, SpawnPrefab, asset_path);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(32);

        if (GUILayout.Button("Item", EditorStyles.miniButton))
        {
            objectType = ObjectType.Block;
            blockMenu.ShowAsContext();
        }
        if (GUILayout.Button("Target", EditorStyles.miniButton))
        {
            objectType = ObjectType.Target;
            targetMenu.ShowAsContext();
        }

        if (GUILayout.Button("Obstacle", EditorStyles.miniButton))
        {
            objectType = ObjectType.Obstacle;
            obstacleMenu.ShowAsContext();
        }
    }

    private void SpawnPrefab(object o_path)
    {
        string path = (string)o_path;
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, target.transform);
        StageItemInfo info = instance.gameObject.GetComponent<StageItemInfo>();

        if (objectType == ObjectType.Target)
        {
            instance.gameObject.name = "TARGET: " + instance.gameObject.name;
            StageTarget addedTarget = instance.gameObject.GetComponent<StageTarget>();
            target.stageTarget = addedTarget;
            addedTarget.info = info;
        }

        if (objectType == ObjectType.Block)
        {
            StageItem addedComponent = instance.gameObject.GetComponent<StageItem>();
            if (!addedComponent) addedComponent = instance.AddComponent<StageItem>();
            addedComponent.info = info;
            target.stageItems.Add(addedComponent);
        }

        if (objectType == ObjectType.Obstacle)
        {
            StageObstacle addedComponent = instance.gameObject.GetComponent<StageObstacle>();
            if (!addedComponent) addedComponent = instance.AddComponent<StageObstacle>();
            target.StageObstacles.Add(addedComponent);
        }


        var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            EditorUtility.SetDirty(target);
            EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
        else
            EditorUtility.SetDirty(target);
    }
}

public enum ObjectType
{
    Block,
    Target,
    Obstacle
}
#endif