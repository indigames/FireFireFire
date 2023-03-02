using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor.AddressableAssets;
#endif

public class EditorColdStartup : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] private GameSceneSO _thisScene;
    [SerializeField] private GameSceneSO _mainManagersScene;

    [Header("Raise events")] [SerializeField]
    private AssetReference _coldStartupEvent;

    [SerializeField] private VoidEventChannel _sceneReadyChannel;

    private bool _isStartedCold = true;
    public bool IsColdBoot => _isStartedCold;

    private void Awake()
    {
        if (_mainManagersScene == null) return;
        var managerScene = SceneManager.GetSceneByName(_mainManagersScene.scene.editorAsset.name);
        if (!(managerScene.isLoaded || managerScene.IsValid()))
        {
            Debug.Log("EditorColdStartup::Awake::Loading main managers scene");
            return;
        }

        _isStartedCold = false;
    }

    private void Start()
    {
        if (AddressableAssetSettingsDefaultObject.Settings.ActivePlayModeDataBuilderIndex == 2)
            return;
        if (_isStartedCold)
        {
            Addressables.LoadSceneAsync(_mainManagersScene.scene, LoadSceneMode.Additive).Completed +=
                OnMainManagersSceneLoaded;
        }
    }

    private void OnMainManagersSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        Addressables.LoadAssetAsync<LoadEventChannelSO>(_coldStartupEvent).Completed += OnEventColdStartupLoaded;
    }

    protected virtual void OnEventColdStartupLoaded(AsyncOperationHandle<LoadEventChannelSO> obj)
    {
        if (_thisScene != null)
        {
            obj.Result.RequestLoadScene(_thisScene);
        }
        else
        {
            _sceneReadyChannel.RaiseEvent();
        }
    }
#endif
}