using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Stage Event Channel")]
public class StageEventChannel : ScriptableObject
{
    public Action<Stage> OnEventRaised;

    public void RaiseEvent(Stage value)
    {
        if (OnEventRaised == null)
        {
            Debug.LogWarning($"No one listen to this event {name}");
            return;
        }

        OnEventRaised.Invoke(value);
    }
}