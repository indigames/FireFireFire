using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/Game State Event Channel")]
public class GameStateEventChannel: ScriptableObject
{
    public Action<KantanManager.GameState> OnEventRaised;
    
    public void RaiseEvent(KantanManager.GameState gameState )
    {
        if (OnEventRaised == null)
        {
            Debug.LogWarning($"No one listen to this event {name}");
            return;
        }

        OnEventRaised.Invoke(gameState);
    }
}