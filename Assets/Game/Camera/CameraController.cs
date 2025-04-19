using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    CinemachineCamera cam;
    [SerializeField] float originalCameraDistance;
    [SerializeField] float maxCameraDistance;
    CinemachinePositionComposer positionComposer;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
        cam = GetComponent<CinemachineCamera>();
        positionComposer = GetComponent<CinemachinePositionComposer>();
    }

    private void Start() {
        transform.position = new Vector3(PlayerMovement.Instance.transform.position.x, cam.transform.position.y, cam.transform.position.z);
        positionComposer.CameraDistance = originalCameraDistance;
    }

    private void Update() {
        positionComposer.CameraDistance = Mathf.Min(originalCameraDistance + PlayerMovement.Instance.MoveSpeed, maxCameraDistance);
    }
}
