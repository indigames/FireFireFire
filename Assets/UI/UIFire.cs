using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFire : MonoBehaviour
{
    [SerializeField] private ParticleSystem _fireParticle;

    [Header("Events")]
    [SerializeField] private VoidEventChannel _OnFireStartEvent;
    [SerializeField] private VoidEventChannel _OnFireEndEvent;

    private void OnEnable()
    {
        _OnFireStartEvent.OnEventRaised += StartParticle;
        _OnFireEndEvent.OnEventRaised += EndParticle;
    }

    void OnDisable()
    {
        _OnFireStartEvent.OnEventRaised -= StartParticle;
        _OnFireEndEvent.OnEventRaised -= EndParticle;
    }
    private void StartParticle()
    {
        _fireParticle.Play();
    }

    private void EndParticle()
    {
        _fireParticle.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
}
