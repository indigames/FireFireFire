using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIIntro : MonoBehaviour
{
    public Gameplay gameplay;
    public Animator animator;
    public VoidEventChannel gameStartEvent;


    // Start is called before the first frame update
    void Awake()
    {
        gameplay.callbackWaitForConfirmStart += Show;
        gameObject.SetActive(false);
    }

    void Show()
    {
        gameObject.SetActive(true);
        animator.Play("Show");
    }

    public void Confirm()
    {
        animator.Play("Hide");
        StartCoroutine(CoConfirm());
    }

    IEnumerator CoConfirm()
    {
        yield return new WaitForSeconds(0.25f);
        gameplay.ConfirmStart = true;
        gameStartEvent?.RaiseEvent();
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
