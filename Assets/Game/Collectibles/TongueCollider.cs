using System;
using UnityEngine;

public class TongueCollider : MonoBehaviour {

    public event Action<Collider> OnTriggerEntered;

    private void OnTriggerEnter(Collider other) {
        OnTriggerEntered?.Invoke(other);
    }
}

