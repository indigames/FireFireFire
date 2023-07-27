using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageItemInfo : MonoBehaviour
{
    [SerializeField] private int _score;
    [SerializeField] private int _unusedScore;

    public int GetScore()
    {
        return _score;
    }

    public int GetUnusedScore()
    {
        return _unusedScore;
    }
}
