using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggeringPlatform : ActivePlatform
{
    [Header("Settings")]
    [SerializeField] private float _deActiveTime;
    [SerializeField] private TriggerPlatformEntry triggerPlatformEntry;

    [Space]
    [SerializeField] private GameObject ImageCountDown;
    [SerializeField] private TextMesh txtCountDown;

    [Space]
    [SerializeField] private MeshRenderer[] activeGameObjects;
    [SerializeField] private Collider[] colliders;

    [SerializeField] private Material GameObjectMatOn;
    [SerializeField] private Material GameObjectMatOff;

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
        ImageCountDown.SetActive(true);
        yield return null;
        while (Time.time < _endActiveTime)
        {
            txtCountDown.text = Mathf.FloorToInt(_endActiveTime - Time.time).ToString(); ;
            yield return null;
        }
        triggerPlatformEntry.OnTriggerDeActive?.Invoke();
        ImageCountDown.SetActive(false);
        SetActiveGameObjects(true);
    }

    private void SetActiveGameObjects(bool value)
    {
        foreach (var item in activeGameObjects)
        {
            if (value)
            {
                item.material = GameObjectMatOn;
            }
            else
            {
                item.material = GameObjectMatOff;
            }
        }

        foreach (var item in colliders)
        {
            item.enabled = value;
        }
    }
}
