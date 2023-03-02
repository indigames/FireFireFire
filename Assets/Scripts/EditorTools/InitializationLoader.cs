using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public class InitializationLoader : MonoBehaviour
{
    [SerializeField] private GameSceneSO _managersScene; // managers for everything in the game (audio, etc.)
    [SerializeField] private GameSceneSO _titleScene; // main menu scene

    [SerializeField] private AssetReference _requestLoadTitleScene;


    [Header("Raise Events")] [SerializeField]
    private FloatEventChannel OnProgress;

    private LoadEventChannelSO _requestLoadSceneEventChannel;

    private void Awake()
    {
        LoadManagerScene();
    }

    #region Class

    /// <summary>
    /// You must to load the managers scene first.
    /// </summary>
    private void LoadManagerScene()
    {
        Addressables.LoadSceneAsync(_managersScene.scene, LoadSceneMode.Additive).Completed += OnManagerSceneLoaded;
    }

    /// <Summary>
    /// This function is used to download the scene.
    /// </Summary>
    private void OnManagerSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        StartCoroutine(DownloadScene());
    }

    /// <Summary>
    /// This function is used to download the scene.
    /// </Summary>
    private IEnumerator DownloadScene()
    {
        var downloadScene = Addressables.LoadAssetAsync<LoadEventChannelSO>(_requestLoadTitleScene);
        downloadScene.Completed += OnRequestLoadSceneEventAssetLoaded;

        while (!downloadScene.IsDone)
        {
            var status = downloadScene.GetDownloadStatus(); // Get the status of the download
            float progress = status.Percent; //get current progress

            OnProgress.RaiseEvent(progress);

            yield return null;
        }

        OnProgress.RaiseEvent(1);
    }

    /// <summary>
    /// This function is used to load the scene.
    /// </summary>
    /// <param name="obj"> The scene that you want to load. </param>
    private void OnRequestLoadSceneEventAssetLoaded(AsyncOperationHandle<LoadEventChannelSO> obj)
    {
        _requestLoadSceneEventChannel = obj.Result;

        _requestLoadSceneEventChannel.RequestLoadScene(_titleScene);
        SceneManager.UnloadSceneAsync(0);
    }

    #endregion
}