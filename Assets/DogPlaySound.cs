using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogPlaySound : MonoBehaviour
{
    public AudioSource audioSource;
    public VoidEventChannel OnObjectBurn;

    void OnEnable()
    {
        if (OnObjectBurn) OnObjectBurn.OnEventRaised += PlaySound;
    }

    private void PlaySound()
    {
        audioSource.Play();
    }

    void OnDisable()
    {

        if (OnObjectBurn) OnObjectBurn.OnEventRaised -= PlaySound;
    }
}
