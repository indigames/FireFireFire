using System;
using UnityEngine;


[CreateAssetMenu(menuName = "Events/Void Event Channel")]
public class VoidEventChannel : ScriptableObject
{
    public Action OnEventRaised;

    public void RaiseEvent()
    {
        if (OnEventRaised == null)
        {
            Debug.LogWarning($"No one listen to this event {name}");
            return;
        }

        OnEventRaised.Invoke();
    }
}