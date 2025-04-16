using Unity.VisualScripting;
using UnityEngine;

public class Fly : MonoBehaviour, ILickable
{
    // Start is called once before the first execution of Update after the BaseItem is created
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 acceleration;
    bool isEaten = false;

    private Vector3 spawnPosition;
    Vector3 currentRandomDir = Vector3.zero;

    [SerializeField] FlyConfigSO[] configs;

    FlyConfigSO config;

    public void TriggerOnCollectedAction()
    {
        if(isEaten) return;
        isEaten = true;

        // Add points to the score
        HighScoreManager.Instance.AddScore(config.points);
    }

    void Awake()
    {
        // config laden
        config = configs[Random.Range(0, configs.Length)];
    }

    void Start()
    {
        // Set spawn position
        spawnPosition = transform.position;
        position = transform.position;
        position.y = 0.5f;
        transform.localScale = new Vector3(config.size, config.size, config.size);
    }

    // Update is called once per frame
    void Update()
    {
        if (isEaten) return;

        // Fly Brain
        FlyMovement();

        // Update position
        transform.position = position;
    }

    void FlyMovement()
    {
        Vector3 targetRandomDir = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-0.2f, 0.2f), 
            Random.Range(-1f, 1f)
        ).normalized;

        currentRandomDir = Vector3.Lerp(currentRandomDir, targetRandomDir, Time.deltaTime * 2f);

        float targetHeight = 1.2f;
        float heightDiff = targetHeight - position.y;
        Vector3 heightAdjustment = new Vector3(0, heightDiff * 1f, 0);

        Vector3 toCenter = spawnPosition - position;
        Vector3 centerForce = Vector3.zero;
        if (toCenter.magnitude > config.flyRadius)
        {
            float distanceRatio = Mathf.Clamp01(toCenter.magnitude / config.flyRadius);
            centerForce = toCenter.normalized * distanceRatio * 1.5f;
        }

        acceleration = currentRandomDir * 1f + centerForce + heightAdjustment;

        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, config.maxSpeed);

        position += velocity * Time.deltaTime;

        position.y = Mathf.Max(position.y, 1f);
    }
}
