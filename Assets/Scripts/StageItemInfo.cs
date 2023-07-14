using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageItemInfo : MonoBehaviour
{
    [SerializeField] private int _score;

    public int GetScore()
    {
        return _score;
    }
}
