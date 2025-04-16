using UnityEngine;

public class GameOverZone : MonoBehaviour {
    [SerializeField] float _scrollSpeed;

    bool paused = false;
    float pauseTimer;

    private void Start() {
        Effect_TimeStopper.OnActionTriggered += Effect_TimeStopper_OnActionTriggered;
    }

    private void Effect_TimeStopper_OnActionTriggered(ItemConfigSO_TimeStopper obj) {
        paused = true;
        pauseTimer = obj.duration;
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
