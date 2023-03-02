using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInfo : MonoBehaviour
{
    [Serializable]
    public class User
    {
        [SerializeField] public int totalScore;
        [SerializeField] public int highScore;
        [SerializeField] public int score;
    }

    private static int totalScore = 0;
    private static int highScore = 0;
    private static int score = 0;
    private static bool needUpdate = false; 

    public static void FromJSON(string json){
        Debug.Log(json.ToString());
        //var json1 = "{\"totalScore\":\"0\",\"highScore\":\"0\",\"score\":\"0\"}"; 
        var user = JsonUtility.FromJson<User>(json);
        totalScore = user.totalScore;
        highScore = user.highScore;
        score = user.score;
    }

    public static string ToJSON(){
        User user = new User();
        user.totalScore = totalScore;
        user.highScore = highScore;
        user.score = score;
        return JsonUtility.ToJson(user);
    }

    public static int GetScore(){return score;}
    public static void SetScore(int value){score = value; needUpdate=true;}
    public static int GetHighScore(){return highScore;}
    public static void SetHighScore(int value){highScore = value; needUpdate=true;}
    public static int GetTotalScore(){return totalScore;}
    public static void SetTotalScore(int value){totalScore = value; needUpdate=true;}
    public static bool NeedUpdate(){
        var tmp = needUpdate;
        needUpdate = false;
        return tmp;
    }
}
