using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] float rotateSpeed;
    public enum RotationAxis {
        X,
        Y,
        Z
    }

    [SerializeField] RotationAxis axis;

    private void Update() {
        switch (axis) {
            case RotationAxis.X:
                RotateAroundX();
                break;
            case RotationAxis.Y:
                RotateAroundY();

                break;
            case RotationAxis.Z:
                RotateAroundZ();

                break;
            default:
                break;
        }
    }

    public void RotateAroundZ() {
        transform.Rotate(Vector3.forward, Time.deltaTime * rotateSpeed);
    }

    public void RotateAroundY() {
        transform.Rotate(Vector3.up, Time.deltaTime * rotateSpeed);
    }

    public void RotateAroundX() {
        transform.Rotate(Vector3.right, Time.deltaTime * rotateSpeed);
    }
}
