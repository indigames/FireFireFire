using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlatformEntry : MonoBehaviour
{
    public Action OnTriggerActive;
    public Action OnTriggerDeActive;
    [SerializeField] private string _triggerTag;
    [SerializeField] private MeshRenderer _meshRenderer;

    private HashSet<Transform> _ignoreGameObject;

    void Awake()
    {
        OnTriggerDeActive += ShowTrigger;
        _ignoreGameObject = new();
    }

    void OnDestroy()
    {
        OnTriggerDeActive = null;
    }

    private bool isTriggered;
    public void ShowTrigger()
    {
        isTriggered = false;
        _meshRenderer.enabled = true;
    }

    public void HideTrigger()
    {
        isTriggered = true;
        _meshRenderer.enabled = false;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(_triggerTag))
        {
            Debug.Log(isTriggered);
            if (!_ignoreGameObject.Contains(other.transform.parent) && !isTriggered)
            {
                HideTrigger();
                OnTriggerActive?.Invoke();
            }
            _ignoreGameObject.Add(other.transform.parent);
        }
    }
}
