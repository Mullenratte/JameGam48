using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public abstract class BaseItem : MonoBehaviour, ILickable {

    [SerializeField] ItemData data;
    public static event Action<ItemData> OnCollected;


    public virtual void TriggerOnCollectedAction() {
        Debug.Log("collected " + this.name);
        OnCollected?.Invoke(this.data);
        Destroy(gameObject);
    }

    public virtual void TriggerOnHitAction() {
        Debug.Log("hit item " + this.name);
    }
}

