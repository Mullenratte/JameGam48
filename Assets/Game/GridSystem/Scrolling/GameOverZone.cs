using UnityEngine;

public class GameOverZone : MonoBehaviour {
    [SerializeField] float _scrollSpeed;

    bool paused = false;
    float pauseTimer;

    private void Start() {
        Item_TimeStopper.OnActionTriggered += Item_TimeStopper_OnActionTriggered;
    }

    private void Item_TimeStopper_OnActionTriggered(object sender, float e) {
        paused = true;
        pauseTimer = e;
    }

    void Update() {
        if (!paused) {
            transform.position += Vector3.forward * Time.deltaTime * _scrollSpeed;
        } else {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0) {
                paused = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<PlayerMovement>(out _)) {
            GameManager.Instance.EndGame();

        }
    }


}
