using System;
using Unity.VisualScripting;
using UnityEngine;

public class GameOverZone : MonoBehaviour {
    public static GameOverZone Instance;
    [SerializeField] float _scrollSpeed;
    [SerializeField] int _scrollSpeedIncreaseInterval = 5;
    [SerializeField] float _defaultScrollSpeedIncrease;
    public int rowToDelete = 0;

    [SerializeField] int baseScorePerRow;
    int scorePerRow;
    [SerializeField, Range(1f, 5f)] float scorePerRowMultiplier;
    [SerializeField] int scorePerRowIncreaseRowCount;

    bool paused = false;
    float pauseTimer;

    [SerializeField] AudioClip[] _rowDeleteClips;
    [SerializeField] float volumeFalloffMultiplier;
    public event Action OnHitPlayer;

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

        scorePerRow = baseScorePerRow;
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
        if (transform.position.z > rowToDelete)
        {
            //LevelGrid.Instance.RemoveFirstRow();
            if(rowToDelete % _scrollSpeedIncreaseInterval == 0)
            {
                _scrollSpeed += _defaultScrollSpeedIncrease;
            }
            rowToDelete++;

            if (rowToDelete % scorePerRowIncreaseRowCount == 0) {
                scorePerRow = (int)(baseScorePerRow * scorePerRowMultiplier);
            }

            HighScoreManager.Instance.AddScore(scorePerRow);


            float volumeFalloff = volumeFalloffMultiplier * Mathf.Abs(PlayerMovement.Instance.transform.position.z - GameOverZone.Instance.transform.position.z);
            SoundFXManager.instance.PlayRandomSoundFXClipPitchVariation(
                _rowDeleteClips, 
                transform.position, 
                Mathf.Max(0f, 1f - volumeFalloff),
                .6f,
                1f);
            CinemachineCameraShake.Instance.ScreenshakeDefault();
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
            OnHitPlayer?.Invoke();
        }

        if (other.TryGetComponent<ILickable>(out _))
        {
            Destroy(other.gameObject);
        }
    }


}
