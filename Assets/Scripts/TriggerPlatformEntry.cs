using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlatformEntry : MonoBehaviour
{
    public Action OnTriggerActive;
    public Action OnTriggerDeActive;
    [SerializeField] private string triggerTag;

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

    public void ShowTrigger()
    {
        this.gameObject.SetActive(true);
    }

    public void HideTrigger()
    {
        this.gameObject.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(triggerTag) && !_ignoreGameObject.Contains(other.transform.parent))
        {
            _ignoreGameObject.Add(other.transform.parent);
            HideTrigger();
            OnTriggerActive?.Invoke();
        }
    }
}
