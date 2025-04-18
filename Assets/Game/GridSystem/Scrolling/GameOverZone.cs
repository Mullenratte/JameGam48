using Unity.VisualScripting;
using UnityEngine;

public class GameOverZone : MonoBehaviour {
    public static GameOverZone Instance;
    [SerializeField] float _scrollSpeed;
    [SerializeField] int _scrollSpeedIncreaseInterval = 5;
    [SerializeField] float _defaultScrollSpeedIncrease;
    public int rowToDelete = 0;

    bool paused = false;
    float pauseTimer;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() {
        Effect_TimeStopper.OnActionTriggered += Effect_TimeStopper_OnActionTriggered;
    }

    private void Effect_TimeStopper_OnActionTriggered(ItemConfigSO_TimeStopper obj) {
        paused = true;
        pauseTimer = obj.duration;
    }

    void Update() {
        transform.position = new Vector3(PlayerMovement.Instance.transform.position.x, transform.position.y, transform.position.z);
        if (!paused) {
            transform.position += Vector3.forward * Time.deltaTime * _scrollSpeed;
        } else {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0) {
                paused = false;
            }
        }
        if (transform.position.z - 1 > rowToDelete)
        {
            //LevelGrid.Instance.RemoveFirstRow();
            if(rowToDelete % _scrollSpeedIncreaseInterval == 0)
            {
                _scrollSpeed += _defaultScrollSpeedIncrease;
            }
            rowToDelete++;
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SpeedUp()
    {
        _scrollSpeed += _defaultScrollSpeedIncrease;
    }

    public void SpeedUp(float amount)
    {
        _scrollSpeed += amount;
    }

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent<PlayerMovement>(out _)) {
            //GameManager.Instance.EndGame();
        }

        if (other.TryGetComponent<ILickable>(out _))
        {
            Destroy(other.gameObject);
        }
    }


}
