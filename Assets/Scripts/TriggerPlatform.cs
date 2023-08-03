using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlatform : ActivePlatform
{
    [Header("Settings")]
    [SerializeField] private float _deActiveTime;
    [SerializeField] private TriggerPlatformEntry triggerPlatformEntry;
    [SerializeField] private GameObject[] activeGameObjects;

    protected override void OnObjectActive()
    {
        triggerPlatformEntry.OnTriggerActive += SetObjectActive;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        triggerPlatformEntry.OnTriggerActive -= SetObjectActive;
    }

    private void SetObjectActive()
    {
        StartCoroutine(CoActive());
    }

    private float _endActiveTime;
    protected override IEnumerator CoActive()
    {
        _endActiveTime = Time.time + _deActiveTime;
        SetActiveGameObjects(false);
        yield return null;
        while (Time.time < _endActiveTime)
        {
            yield return null;
        }
        triggerPlatformEntry.OnTriggerDeActive?.Invoke();
        SetActiveGameObjects(true);
    }

    private void SetActiveGameObjects(bool value)
    {
        foreach (var item in activeGameObjects)
        {
            item.SetActive(value);
        }
    }
}