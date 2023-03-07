using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class KantanManager : MonoBehaviour
{
    public enum TitleState
    {
        WaitStartButton,
        WaitGameStart,
        EndTitle
    }

    public enum GameState
    {
        PlayGame,
        SaveData,
        GameClear,
        EndCheck,
        EndGame
    }

    public enum AdMode
    {
        NoAd,
        ShowAd,
        EndAd
    }

    [SerializeField] private TitleState titleState;
    [SerializeField] private GameState gameState;
    [SerializeField] private AdMode adMode;

    [Header("List of Event Channels")]
    [SerializeField]
    private VoidEventChannel gameStartEvent;

    [SerializeField] private GameStateEventChannel gameStateEvent;
    [SerializeField] private AdModeEventChannel adModeEvent;

    /// <summary>
    /// If your game dont use score, you can delete this.
    /// </summary>
    [SerializeField] private IntEventChannel gameEndEvent;
    [SerializeField] private VoidEventChannel OnRequestShowAdsEvent;
    [SerializeField] private BoolEventChannel OnShowAdsRewardEvent;
    private float delayTime = 0.5f;
    private float _clearTimer;
    private static int score = 0;

    // Start is called before the first frame update
    void Start()
    {

        titleState = TitleState.WaitStartButton;
    }

    private void Awake()
    {
        if (gameStartEvent != null) gameStartEvent.OnEventRaised += OnGameStart;
        if (gameStateEvent != null) gameStateEvent.OnEventRaised += OnGameState;
        // if (adModeEvent != null) adModeEvent.OnEventRaised += OnAdMode;
        if (gameEndEvent != null) gameEndEvent.OnEventRaised += OnGameEnd;
        if (OnRequestShowAdsEvent != null) OnRequestShowAdsEvent.OnEventRaised += OnShowRewardAdsRequest;

    }
    private void OnDestroy()
    {
        if (gameStartEvent != null) gameStartEvent.OnEventRaised -= OnGameStart;
        if (gameStateEvent != null) gameStateEvent.OnEventRaised -= OnGameState;
        // if (adModeEvent != null) adModeEvent.OnEventRaised -= OnAdMode;
        if (gameEndEvent != null) gameEndEvent.OnEventRaised -= OnGameEnd;
        if (OnRequestShowAdsEvent != null) OnRequestShowAdsEvent.OnEventRaised -= OnShowRewardAdsRequest;
    }

    private void OnGameEnd(int _score)
    {
        Debug.Log($"SendScore: {_score}");
        KantanGameBox.GameEnd(_score);
        // SetScore(_score);
    }

    private void OnGameState(GameState _gameState)
    {
        if (_gameState == GameState.PlayGame)
        {
            UpdateScore();
            gameState = _gameState;
        }
        else if (_gameState == GameState.SaveData)
        {
            if (KantanGameBox.IsGameSaveFinish())
            {
                _clearTimer = 2.0f;
                gameStateEvent.RaiseEvent(GameState.GameClear);
            }
        }
        else if (_gameState == GameState.EndCheck)
        {
            if (KantanGameBox.IsGameEndFinish())
            {
                // Load Title Scene
                gameStateEvent.RaiseEvent(GameState.EndGame);
            }
        }
    }
    public void OnShowRewardAdsRequest()
    {
        if (adMode == AdMode.NoAd)
        {
            delayTime = 0.5f;
            KantanGameBox.ShowRewardAd();
            adMode = AdMode.ShowAd;
            // adModeEvent.RaiseEvent(AdMode.ShowAd);
        }
    }

    private void OnAdMode(AdMode _adMode)
    {
        if (_adMode == AdMode.NoAd)
        {
            KantanGameBox.ShowRewardAd();
            // adModeEvent.RaiseEvent(AdMode.ShowAd);
        }
        else if (adMode == AdMode.ShowAd)
        {
            if (KantanGameBox.IsShowRewardAdFinish())
            {
                // reward success
            }
            else
            {
                // reward fail
            }

            adModeEvent.RaiseEvent(AdMode.EndAd);
        }
    }

    private void OnGameStart()
    {
        KantanGameBox.GameStart();
        titleState = TitleState.WaitGameStart;
        adMode = AdMode.NoAd;
        KantanGameBox.IsGameStartFinish();
    }

    // Update is called once per frame
    void Update()
    {
        if (titleState == TitleState.WaitGameStart)
        {
            if (KantanGameBox.IsGameStartFinish())
            {
                // request load game scene
                titleState = TitleState.EndTitle;
            }
        }


        if (gameState == GameState.GameClear)
        {
            if (_clearTimer > 0)
            {
                _clearTimer -= Time.deltaTime;
                if (_clearTimer <= 0)
                {
                    // KantanGameBox.GameEnd();
                    gameStateEvent.RaiseEvent(GameState.EndCheck);
                }
            }
        }
        if (adMode == AdMode.ShowAd)
        {
            delayTime -= Time.deltaTime;
            if (delayTime > 0) return;
            if (KantanGameBox.IsShowRewardAdFinish())
            {
                    OnShowAdsRewardEvent.RaiseEvent(true);
                    Debug.Log("Show ads success");
            }
            else
            {

                    OnShowAdsRewardEvent.RaiseEvent(false);
                    Debug.Log("Show ads fail");
            }
            adMode = AdMode.EndAd;
            adModeEvent.RaiseEvent(AdMode.EndAd);
        }
    }

    private void UpdateScore()
    {
        Debug.Log($"UpdateScore: {score}");
        // KantanGameBox.GameSave(SetJson());
    }

    // public static void GetJson(string json)
    // {
    //     Debug.Log(json.ToString());
    //     var user = JsonUtility.FromJson<SaveStructReference>(json);
    //     score = user.Value.coin;
    // }

    // public static string SetJson()
    // {
    //     // SaveStructReference saveRef = new SaveStructReference();
    //     // saveRef.Value.coin = score;
    //     return JsonUtility.ToJson(saveRef);
    // }

    public static int GetScore()
    {
        return score;
    }

    public static void SetScore(int _score)
    {
        score = _score;
    }
}