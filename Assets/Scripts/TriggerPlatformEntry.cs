using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlatformEntry : MonoBehaviour
{
    public Action OnTriggerActive;
    public Action OnTriggerDeActive;

    private HashSet<Transform> _ignoreGameObject;

    void Awake()
    {
        OnTriggerDeActive += ShowTrigger;
        _ignoreGameObject = new HashSet<Transform>();
        Debug.Log("triggerPlatform:" + this.gameObject.name+":" + this.GetInstanceID());
    }

    void OnDestroy()
    {
        OnTriggerDeActive = null;
    }

    private bool isTriggered = false;
    public void ShowTrigger()
    {
        isTriggered = false;
    }

    public void HideTrigger()
    {
        isTriggered = true;
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == null) return;
        if (other.transform.CompareTag("BurnBlock"))
        {
            Debug.Log(other.transform.parent + "|" + _ignoreGameObject);
            if (!_ignoreGameObject.Contains(other.transform.parent) && !isTriggered)
            {
                Debug.Log("triggered");
                HideTrigger();

                Debug.Log("triggered2");
                OnTriggerActive?.Invoke();
                Debug.Log("triggered3");
            }
            _ignoreGameObject.Add(other.transform.parent);
        }
    }
}
