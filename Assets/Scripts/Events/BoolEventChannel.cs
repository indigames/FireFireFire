using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Bool Event Channel")]
public class BoolEventChannel : ScriptableObject
{
    public Action<bool> OnEventRaised;
    public bool isActive;

    public void RaiseEvent(bool isBool)
    {
        OnEventRaised?.Invoke(isBool);
        isActive = isBool;
    }
}