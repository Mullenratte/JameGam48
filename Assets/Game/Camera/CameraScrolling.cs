using Unity.Cinemachine;
using UnityEngine;

public class CameraScrolling : MonoBehaviour
{
    CinemachineCamera cam;
    [SerializeField] float _scrollSpeed;

    private void Awake() {
        cam = GetComponent<CinemachineCamera>();
    }

    private void Start() {
        cam.transform.position = new Vector3(LevelGrid.Instance.GridSystem.GetWidth() / 2, cam.transform.position.y, cam.transform.position.z);
    }

    //private void Update() {
    //    cam.transform.position += Vector3.forward * Time.deltaTime * _scrollSpeed;
    //}
}
