using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBoost", menuName = "Scriptable Objects/ItemEffects/Speed Boost")]
public class Effect_SpeedBoost : ItemEffect {
    public ItemConfigSO_SpeedBoost config;

    public static event Action<ItemConfigSO_SpeedBoost> OnActionTriggered;

    

    public override void Activate() {
        OnActionTriggered?.Invoke(config);
        SoundFXManager.instance.PlaySoundFXClip(ActivatedAudio, PlayerMovement.Instance.transform.position, .2f, 0.8f, 1.2f);
    }
}
