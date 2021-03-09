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
        gameplay.callbackRestart += Restart;
        gameplay.callbackRemainingMeshblock += ResetRemainingCount;
        gameplay.callbackVictory += Victory;
        gameplay.callbackDefeat += Defeat;
    }

    void Restart()
    {
        markerVictory.SetActive(false);
        markerDefeat.SetActive(false);
    }

    void ResetRemainingCount(int count)
    {
        if (count > 1)
            textRemaining.text = string.Format("{0} items remaning", count);
        else if (count == 1)
            textRemaining.text = "1 item remaning";
        else
            textRemaining.text = "No item remaning";
    }

    void Victory()
    {
        markerVictory.SetActive(true);
    }

    void Defeat()
    {
        markerDefeat.SetActive(true);
    }

    public void RestartGame()
    {
        gameplay.RestartGame();
    }

}
