using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;


[CreateAssetMenu(fileName = "GameScene", menuName = "ScriptableObjects/Game Scene")]
public class GameSceneSO : ScriptableObject
{
    public SceneAssetReference scene;
    public SceneType sceneType;
    public bool UnloadPreviousScene = true;

    [HideInInspector] public AsyncOperationHandle<SceneInstance> handle;

    public enum SceneType
    {
        Title,
        Init,
        Managers,
        Gameplay,
    }
}