using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BaseItem : MonoBehaviour, ILickable {

    Vector3 startPos;

    private void Start() {
        startPos = transform.position;
    }

    public virtual void TriggerOnLickedAction() {
        Debug.Log("collected " + this.name);

    }
}

