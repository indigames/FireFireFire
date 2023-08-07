using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerPlatformEntry : MonoBehaviour
{
    [SerializeField] private MeshRenderer _switchRenderer;
    [SerializeField] private Color _activeColor = Color.white;
    [SerializeField] private Color _unactiveColor = Color.white;


    public Action OnTriggerActive;
    public Action OnTriggerDeActive;

    private HashSet<Transform> _ignoreGameObject;

    void Awake()
    {
        OnTriggerDeActive += ShowTrigger;
        _switchRenderer.material = new Material(_switchRenderer.material);
        ShowTrigger();
        _ignoreGameObject = new HashSet<Transform>();
    }

    void OnDestroy()
    {
        OnTriggerDeActive = null;
    }

    private bool isTriggered = false;
    public void ShowTrigger()
    {
        isTriggered = false;
        _switchRenderer.material.SetColor("_ButtonColor", _activeColor);

        _CurrentScaleTarget = ScaleActive;
        if (lastCoroutine != null) StopCoroutine(lastCoroutine);
        lastCoroutine = StartCoroutine(playSwitchAnim());
    }

    public void HideTrigger()
    {
        isTriggered = true;
        _switchRenderer.material.SetColor("_ButtonColor", _unactiveColor);

        _CurrentScaleTarget = ScaleUnactive;
        if (lastCoroutine != null) StopCoroutine(lastCoroutine);
        lastCoroutine = StartCoroutine(playSwitchAnim());
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == null) return;
        if (other.transform.CompareTag("BurnBlock"))
        {
            if (!_ignoreGameObject.Contains(other.transform.parent) && !isTriggered)
            {
                HideTrigger();

                OnTriggerActive?.Invoke();
            }
            _ignoreGameObject.Add(other.transform.parent);
        }
    }

    public Transform switchModel;
    public float ScaleUnactive;
    public float ScaleActive;
    public float ScaleSpeed;

    private float _CurrentScaleTarget;
    private Coroutine lastCoroutine;
    IEnumerator playSwitchAnim()
    {
        while (Mathf.Abs(switchModel.localScale.y - _CurrentScaleTarget) > 0.01f)
        {
            switchModel.localScale = new Vector3(switchModel.localScale.x, Mathf.Lerp(switchModel.localScale.y, _CurrentScaleTarget, ScaleSpeed), switchModel.localScale.z);
            yield return null;
        }
    }
}
