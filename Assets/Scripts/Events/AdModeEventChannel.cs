
    using System;
    using UnityEngine;

    [CreateAssetMenu(menuName = "Events/AdMode Event Channel")]
    public class AdModeEventChannel: ScriptableObject
    {
        public Action<KantanManager.AdMode> OnEventRaised;
        
        public void RaiseEvent(KantanManager.AdMode adMode )
        {
            if (OnEventRaised == null)
            {
                Debug.LogWarning($"No one listen to this event {name}");
                return;
            }

            OnEventRaised.Invoke(adMode);
        }

    }