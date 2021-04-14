using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIOverlay : MonoBehaviour
{

    public Gameplay gameplay;
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        gameplay.callbackRestart += Show;
        gameplay.callbackWaitForConfirmStart += Hide;
    }

    void Show()
    {
        gameObject.SetActive(true);
        animator.Play("Show", 0, 0);
    }

    void Hide()
    {
        animator.Play("Hide");
    }
}
