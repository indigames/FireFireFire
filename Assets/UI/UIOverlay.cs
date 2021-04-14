using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOverlay : MonoBehaviour
{

    public Gameplay gameplay;
    public Animator animator;

    bool firstTime = true;
    // Start is called before the first frame update
    IEnumerator Start()
    {
        gameplay.callbackRestart += Show;
        gameplay.callbackWaitForConfirmStart += Hide;

        yield return new WaitForSeconds(0.2f);
        Hide();
    }

    void Show()
    {
        if (firstTime)
        {
            firstTime = false;
            return;
        }

        gameObject.SetActive(true);

        animator.Play("Show", 0, 0);
    }

    void Hide()
    {
        animator.Play("Hide", 0, 0);
    }
}
