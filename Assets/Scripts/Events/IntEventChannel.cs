using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Int Event Channel")]
public class IntEventChannel : ScriptableObject
{
    public Action<int> OnEventRaised;

    public void RaiseEvent(int value)
    {
        OnEventRaised?.Invoke(value);
    }
}