using Unity.Cinemachine;
using UnityEngine;

public class CameraSetup : MonoBehaviour
{
    CinemachineCamera cam;

    private void Awake() {
        cam = GetComponent<CinemachineCamera>();
    }

    private void Start() {
        transform.position = new Vector3(LevelGrid.Instance.GridSystem.GetWidth() / 2, cam.transform.position.y, cam.transform.position.z);
    }
}
