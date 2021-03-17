using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplay : MonoBehaviour
{
    public Gameplay gameplay;

    public Text textRemaining;

    // Start is called before the first frame update
    void Start()
    {
        gameplay.callbackRestart += Restart;
        gameplay.callbackRemainingMeshblock += ResetRemainingCount;
    }

    void Restart()
    {
    }

    void ResetRemainingCount(int count)
    {

        textRemaining.text = string.Format("x<color=red>{0}</color>", count);


        //if (count > 1)
        //    textRemaining.text = string.Format("<color=red>{0}</color> items remaning", count);
        //else if (count == 1)
        //    textRemaining.text = "<color=red>1</color> item remaning";
        //else
        //    textRemaining.text = "No item remaning";
    }

    public void RestartGame()
    {
        gameplay.RestartGame(false);
    }

}
