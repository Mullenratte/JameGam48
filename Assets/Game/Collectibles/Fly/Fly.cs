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

    [SerializeField] FlyConfigSO[] configs;

    FlyConfigSO config;

    public void TriggerOnLickedAction()
    {
        Debug.Log("collected " + this.name + ": +" + config.points + " points");
        isEaten = true;
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
        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f), 
            Random.Range(-1f, 1f)
        ).normalized;

        // Calculate acceleration
        acceleration = randomDirection * Time.deltaTime;

        Vector3 toCenter = spawnPosition - position;
        if (toCenter.magnitude > config.flyRadius)
        {
            acceleration += toCenter.normalized;
        }

        // Calculate velocity
        velocity += acceleration;
        velocity = Vector3.ClampMagnitude(velocity, config.maxSpeed);

        // Update position
        position += velocity * Time.deltaTime;

        acceleration = Vector3.zero;
    }
}
