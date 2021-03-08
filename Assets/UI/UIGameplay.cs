using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplay : MonoBehaviour
{

    public Gameplay gameplay;

    public Text textRemaining;
    public GameObject markerVictory;
    public GameObject markerDefeat;

    // Start is called before the first frame update
    void Start()
    {
        gameplay.callbackRemainingMeshblock += ResetRemainingCount;
        gameplay.callbackVictory += Victory;
        gameplay.callbackDefeat += Defeat;
    }

    void ResetRemainingCount(int count)
    {
        if (count > 1)
            textRemaining.text = string.Format("{0} items\nremaning", count);
        else if (count == 1)
            textRemaining.text = "1 item\nremaning";
        else
            textRemaining.text = "No item\nremaning";
    }

    void Victory()
    {
        markerVictory.SetActive(true);
    }

    void Defeat()
    {
        markerDefeat.SetActive(true);
    }

}
