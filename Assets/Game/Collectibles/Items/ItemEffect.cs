using UnityEngine;

public abstract class ItemEffect : ScriptableObject {
    public abstract void Activate();
    [SerializeField] protected AudioClip ActivatedAudio;
}
