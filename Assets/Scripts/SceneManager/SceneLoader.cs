using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneManagerSO _sceneManager;

    [SerializeField] private VoidEventChannel _onSceneReadyChannel;

    [FormerlySerializedAs("_loadTitleEvent")]
    [Header("Listening to")]
    // Should raise in InitializeLoader (correct flow)
    [SerializeField]
    private LoadEventChannelSO _loadMainScene;

    [SerializeField] private LoadEventChannelSO _coldStartupEvent;
    [SerializeField] private GameSceneSO _gameplayManagerSceneSO;

    private GameSceneSO _sceneToLoad;
    private GameSceneSO _currentLoadedScene;
    private SceneInstance _gameplayManagersSceneInstance;
    private AsyncOperationHandle<SceneInstance> _gameplayManagerLoadingOpHandle;
    private AsyncOperationHandle<SceneInstance> _loadingOperationHandle;


    private void OnEnable()
    {
        _loadMainScene.OnLoadSceneRequested += LoadTitleScene;
#if UNITY_EDITOR
        _coldStartupEvent.OnLoadSceneRequested += OnLoadSceneRequested;
#endif
    }

    private void OnDisable()
    {
        _loadMainScene.OnLoadSceneRequested -= LoadTitleScene;
#if UNITY_EDITOR
        _coldStartupEvent.OnLoadSceneRequested -= OnLoadSceneRequested;
#endif
    }
#if UNITY_EDITOR
    private void OnLoadSceneRequested(GameSceneSO roomToLoad)
    {
        _currentLoadedScene = roomToLoad;

        switch (_currentLoadedScene.sceneType)
        {
            // lobby, level, room, boss
            case GameSceneSO.SceneType.Gameplay:
                //Gameplay managers is loaded synchronously
                Debug.Log("ColdStart::Loading gameplay managers");
                _gameplayManagerLoadingOpHandle =
                    Addressables.LoadSceneAsync(_gameplayManagerSceneSO.scene, LoadSceneMode.Additive, true);
                _gameplayManagerLoadingOpHandle.WaitForCompletion();
                _gameplayManagersSceneInstance = _gameplayManagerLoadingOpHandle.Result;

                StartGameplay(1);
                return;
            // title, select stage, UI that doesn't need gameplay managers
            case GameSceneSO.SceneType.Title:
                _onSceneReadyChannel.RaiseEvent();
                return;
        }
    }
#endif

    /// <summary>
    /// 
    /// </summary>
    /// <param name="gameplaySceneToLoad">could be main/lobby/inventory or some room in stage</param>
    private void LoadGameplayScene(GameSceneSO gameplaySceneToLoad)
    {
        _sceneToLoad = gameplaySceneToLoad;

        //In case we are coming from the title/cold boot, we need to load the Gameplay manager scene first
        if (!_gameplayManagersSceneInstance.Scene.IsValid())
        {
            Debug.Log("Loading gameplay managers scene");
            _gameplayManagerLoadingOpHandle =
                Addressables.LoadSceneAsync(_gameplayManagerSceneSO.scene, LoadSceneMode.Additive);
            _gameplayManagerLoadingOpHandle.Completed += OnGameplayManagersLoaded;
        }
        else
        {
            StartCoroutine(UnloadPreviousScene());
        }
    }

    private void OnGameplayManagersLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        _gameplayManagersSceneInstance = _gameplayManagerLoadingOpHandle.Result;

        Debug.Log("gameplay manager scene loaded");
        // unload title
        StartCoroutine(UnloadPreviousScene());
    }

    private void LoadTitleScene(GameSceneSO titleSceneSO)
    {
        // TODO: prevent double loading using a flag

        _sceneToLoad = titleSceneSO;

        // Lobby => Title We will unload gameplay managers
        if (_gameplayManagersSceneInstance.Scene.IsValid())
            Addressables.UnloadSceneAsync(_gameplayManagerLoadingOpHandle);

        StartCoroutine(UnloadPreviousScene());
    }

    private IEnumerator UnloadPreviousScene()
    {
        if (_currentLoadedScene != null &&
            _sceneToLoad.UnloadPreviousScene) //would be null if the game was started in Initialisation
        {
            if (_currentLoadedScene.handle.IsValid())
            {
                //Unload the scene through its AssetReference, i.e. through the Addressable system
                //every assets usage should be through the Addressable system
                Debug.Log($"SceneLoader::UnloadPreviousScene::Unloading scene [{_currentLoadedScene.name}]");
                var handle = Addressables.UnloadSceneAsync(_currentLoadedScene.handle);
                handle.Completed += (AsyncOperationHandle<SceneInstance> a) => { Resources.UnloadUnusedAssets(); };
                // _currentLoadedScene.scene.UnLoadScene();
            }
#if UNITY_EDITOR
            else
            {
                // When cold start gameplay => lobby we will need to unload main/gameplay
                // because OperationHandle is not valid
                Debug.Log($"SceneLoader::UnloadPreviousScene::Unloading scene editor [{_currentLoadedScene.name}]");
                SceneManager.UnloadSceneAsync(_currentLoadedScene.scene.editorAsset.name);
            }
#endif
        }

        LoadNewScene();
        yield break;
    }

    private void LoadNewScene()
    {
        _loadingOperationHandle = Addressables.LoadSceneAsync(_sceneToLoad.scene, LoadSceneMode.Additive, true, 0);
        // Because scene.OperationHandle is invalid
        _sceneToLoad.handle = _loadingOperationHandle;
        _loadingOperationHandle.Completed += OnNewSceneLoaded;
    }

    private void OnNewSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        //Save loaded scenes (to be unloaded at next load request)
        _sceneManager.lastScene = _currentLoadedScene;
        _sceneManager.currentScene = _sceneToLoad;
        _currentLoadedScene = _sceneToLoad;

        Scene s = obj.Result.Scene;
        SceneManager.SetActiveScene(s);
        LightProbes.TetrahedralizeAsync();

        StartGameplay();
    }

    private void StartGameplay(float delay = 0f)
    {
        StartCoroutine(RaiseEventAfterDelay(delay));
    }

    private IEnumerator RaiseEventAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // Only playable scene should broadcast this event
        _onSceneReadyChannel.RaiseEvent();
    }
}