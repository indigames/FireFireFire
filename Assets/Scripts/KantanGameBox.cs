using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class KantanGameBox : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void _GameStart();

    [DllImport("__Internal")]
    private static extern void _GameEnd(int score);

    [DllImport("__Internal")]
    private static extern void _GameSave(string data);

    [DllImport("__Internal")]
    private static extern void _GameGetData();

    [DllImport("__Internal")]
    private static extern void _ShowRewardAd();

    [DllImport("__Internal")]
    private static extern void _DebugOut(string str);

    enum Mode{
        ModeIdle,
        ModeGameStart,
        ModeGameStartWait,
        ModeGameStartFinish,

        ModeGameEnd,
        ModeGameEndWait,
        ModeGameEndFinish,

        ModeGameSave,
        ModeGameSaveWait,
        ModeGameSaveFinish,

        ModeGameGetData,
        ModeGameGetDataWait,
        ModeGameGetDataFinish,

        ModeShowRewardAd,
        ModeShowRewardAdWait,
        ModeShowRewardAdFinish,
    }

    private static Mode mode = Mode.ModeIdle;
    # if !UNITY_EDITOR && UNITY_WEBGL
    private static bool ready = false;
    #else
    private static bool ready = true;
    #endif
    private static int _score = 0;
    private static string _data = "{}";
    private static bool _success = false;

    public void Ready(){
        ready = true;
        # if !UNITY_EDITOR && UNITY_WEBGL
        Debug.Log("Ready-Unity");
        #endif
    }

    void Update()
    {
        if(ready){
            if(mode == Mode.ModeGameStart){
                Debug.Log("GameStart-Unity");
                # if !UNITY_EDITOR && UNITY_WEBGL
                mode = Mode.ModeGameStartWait;
                _GameStart();
                #else
                mode = Mode.ModeGameStartFinish;
                #endif
            }
            else if(mode == Mode.ModeGameEnd){
                Debug.Log("GameEnd-Unity");
                # if !UNITY_EDITOR && UNITY_WEBGL
                mode = Mode.ModeGameEndWait;
                _GameEnd(_score);
                #else
                mode = Mode.ModeGameEndFinish;
                #endif
            }
            else if(mode == Mode.ModeGameSave){
                Debug.Log("GameSave-Unity");
                # if !UNITY_EDITOR && UNITY_WEBGL
                mode = Mode.ModeGameSaveWait;
                _GameSave(_data);
                #else
                mode = Mode.ModeGameSaveFinish;
                #endif
            }
            else if(mode == Mode.ModeGameGetData){
                Debug.Log("GameGetData-Unity");
                # if !UNITY_EDITOR && UNITY_WEBGL
                mode = Mode.ModeGameGetDataWait;
                _GameGetData();
                #else
                Debug.Log("mode = Mode.ModeGameGetDataFinish");
                mode = Mode.ModeGameGetDataFinish;
                #endif
            }
            else if(mode == Mode.ModeShowRewardAd){
                Debug.Log("ShowRewardAd-Unity");
                # if !UNITY_EDITOR && UNITY_WEBGL
                mode = Mode.ModeShowRewardAdWait;
                _ShowRewardAd();
                #else
                mode = Mode.ModeShowRewardAdFinish;
                #endif
            }
        }
    }

    ///////////////////////////////////////////
    public void GameStartFinish(){
        Debug.Log("GameStartFinish-Unity");
        if(mode == Mode.ModeGameStartWait){
            mode = Mode.ModeGameStartFinish;
        }
    }

    public static void GameStart(){
        mode = Mode.ModeGameStart;
    }

    public static bool IsGameStartFinish(){
        return (mode == Mode.ModeGameStartFinish);
    }

    ///////////////////////////////////////////
    public void GameEndFinish(){
        Debug.Log("GameEndFinish-Unity");
        if(mode == Mode.ModeGameEndWait){
            mode = Mode.ModeGameEndFinish;
        }
    }

    public static void GameEnd(int score){
        _score = score;
        mode = Mode.ModeGameEnd;
    }

    public static bool IsGameEndFinish(){
        return (mode == Mode.ModeGameEndFinish);
    }

    ///////////////////////////////////////////
    public void GameSaveFinish(){
        Debug.Log("GameSaveFinish-Unity");
        if(mode == Mode.ModeGameSaveWait){
            mode = Mode.ModeGameSaveFinish;
        }
    }

    public static void GameSave(string data){
        _data = data;
        mode = Mode.ModeGameSave;
    }

    public static bool IsGameSaveFinish(){
        return (mode == Mode.ModeGameSaveFinish);
    }

    ///////////////////////////////////////////
    public void GameGetDataFinish(string data){
        Debug.Log("GameGetDataFinish-Unity");
        if(mode == Mode.ModeGameGetDataWait){
            mode = Mode.ModeGameGetDataFinish;
            _data = data;
        }
    }

    public static void GameGetData(){
        mode = Mode.ModeGameGetData;
    }

    public static bool IsGameGetDataFinish(){
        return (mode == Mode.ModeGameGetDataFinish);
    }

    public static string ReadGameData(){
        Debug.Log("ReadGameData-Unity");
        return _data;
    }

    ///////////////////////////////////////////
    public void ShowRewardAdFinish(int success){
        Debug.Log("ShowRewardAdFinish-Unity");
        if(mode == Mode.ModeShowRewardAdWait){
            mode = Mode.ModeShowRewardAdFinish;
            _success = (success==1);
        }
    }

    public static void ShowRewardAd(){
        mode = Mode.ModeShowRewardAd;
    }

    public static bool IsShowRewardAdFinish(){
        return (mode == Mode.ModeShowRewardAdFinish);
    }

    public static bool IsRewardAdSuccess(){
        Debug.Log("IsRewardAdSuccess-Unity");
        return _success;
    }


}
