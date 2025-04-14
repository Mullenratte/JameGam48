using UnityEngine;

public class GameOverZone : MonoBehaviour {
    [SerializeField] float _scrollSpeed;


    void Update() {
        transform.position += Vector3.forward * Time.deltaTime * _scrollSpeed;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<PlayerMovement>(out _)) {
            GameManager.Instance.EndGame();

        }
    }


}
