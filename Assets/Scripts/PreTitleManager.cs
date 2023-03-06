using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class PreTitleManager : MonoBehaviour
{
    [SerializeField] private AssetReference _requestLoadGameplayScene;
    [SerializeField] private GameSceneSO _gameplayScene;
    private LoadEventChannelSO _requestLoadSceneEventChannel;
    [SerializeField]
    private FloatEventChannel OnProgress;
    bool isStartGame;
    // Start is called before the first frame update
    void Start()
    {
        //Request KantanGameBox to acquire saved data
        KantanGameBox.GameGetData();
    }

    void Update()
    {
        //Wait until save data acquisition is complete
        if (KantanGameBox.IsGameGetDataFinish())
        {
            //Read save data
            PlayerInfo.FromJSON(KantanGameBox.ReadGameData());
            if (!isStartGame)
            {
                Addressables.LoadSceneAsync(_gameplayScene.scene, LoadSceneMode.Additive);
                isStartGame = true;
            }
        }
    }
    private IEnumerator DownloadScene()
    {
        var downloadScene = Addressables.LoadAssetAsync<LoadEventChannelSO>(_requestLoadGameplayScene);
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
    private void OnRequestLoadSceneEventAssetLoaded(AsyncOperationHandle<LoadEventChannelSO> obj)
    {
        _requestLoadSceneEventChannel = obj.Result;

        _requestLoadSceneEventChannel.RequestLoadScene(_gameplayScene);
        // SceneManager.UnloadSceneAsync(0);
    }

}
