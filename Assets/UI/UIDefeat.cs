using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIDefeat : MonoBehaviour
{
    public Gameplay gameplay;
    public Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        gameplay.callbackDefeat += () => gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Continue()
    {
        gameplay.RestartGame();
        StartCoroutine(Hide());
    }

    IEnumerator Hide()
    {
        yield return true;
        anim.Play("Hide");
    }
}
