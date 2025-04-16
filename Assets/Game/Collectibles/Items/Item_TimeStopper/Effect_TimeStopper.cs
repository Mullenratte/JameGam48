using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TimeStopper", menuName = "Scriptable Objects/ItemEffects/Time Stopper")]
public class Effect_TimeStopper : ItemEffect {
    public ItemConfigSO_TimeStopper config;

    public static event Action<ItemConfigSO_TimeStopper> OnActionTriggered;

    public override void Activate() {
        OnActionTriggered?.Invoke(config);
    }
}
