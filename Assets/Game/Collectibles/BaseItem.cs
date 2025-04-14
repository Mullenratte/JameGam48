using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BaseItem : MonoBehaviour, ILickable {

    Vector3 startPos;

    private void Start() {
        startPos = transform.position;
    }

    public void TriggerOnLickedAction() {
        Debug.Log("collected " + this.name);
        //Destroy(gameObject);
    }
}

