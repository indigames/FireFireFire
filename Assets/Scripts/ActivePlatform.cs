using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActivePlatform : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] protected VoidEventChannel OnGameStartEvent;
    protected Transform _objTransform;
    protected Vector3 _originPos;

    protected virtual void Awake()
    {
        _objTransform = transform;
        _originPos = _objTransform.position;
    }

    protected virtual void OnEnable()
    {
        if (OnGameStartEvent) OnGameStartEvent.OnEventRaised += OnObjectActive;
    }

    protected virtual void OnDisable()
    {
        if (OnGameStartEvent) OnGameStartEvent.OnEventRaised -= OnObjectActive;
    }

    protected virtual void OnObjectActive()
    {
        StartCoroutine(CoActive());
    }

    protected abstract IEnumerator CoActive();
}
