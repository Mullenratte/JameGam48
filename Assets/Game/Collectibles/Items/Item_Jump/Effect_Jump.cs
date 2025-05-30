using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Jump", menuName = "Scriptable Objects/ItemEffects/Jump")]
public class Effect_Jump : ItemEffect {
    public ItemConfigSO_Jump config;

    public static event Action<ItemConfigSO_Jump> OnActionTriggered;

    public override void Activate() {
        OnActionTriggered?.Invoke(config);
        SoundFXManager.instance.PlaySoundFXClip(ActivatedAudio, PlayerMovement.Instance.transform.position, .35f, 0.8f, 1.2f);
    }
}
