using Unity.VisualScripting;
using UnityEngine;

public class Fly : MonoBehaviour, ILickable, IScoreObject
{
    // Start is called once before the first execution of Update after the BaseItem is created
    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;
    bool isHit = false;

    private Vector3 spawnPosition;
    Vector3 currentRandomDir = Vector3.zero;

    [SerializeField] FlyConfigSO[] configs;

    FlyConfigSO config;
    Light lightObj;

    public void TriggerOnHitAction() {
        isHit = true;
        SoundFXManager.instance.PlayRandomSoundFXClipPitchVariation(config.OnHitClips, transform.position, 0.3f, 0.8f, 1.3f);
    }

    public void TriggerOnCollectedAction()
    {
        // Add points to the score
        HighScoreManager.Instance.AddScore(config.points);

       // SoundFXManager.instance.PlayRandomSoundFXClipPitchVariation(config.OnCollectedClips, transform.position, 1f, 0.8f, 1.3f);

        Destroy(gameObject);
    }

    void Awake()
    {
        // config laden
        float rnd = Random.Range(0.01f, .99f);
        int configToLoad = 0;
        for (int i = 0; i < configs.Length; i++) { 
            var cfg = configs[i];
            if (cfg.spawnChance > rnd) {
                configToLoad = i;
            }
        }

        config = configs[configToLoad];
        lightObj = GetComponentInChildren<Light>();
    }

    void Start()
    {
        // Set spawn position
        spawnPosition = transform.position;
        position = transform.position;
        position.y = 0.5f;
        transform.localScale = new Vector3(config.size, config.size, config.size);
        GetComponent<MeshRenderer>().material = config.material;
        lightObj.color = Color.Lerp(config.material.color, config.material.GetColor("_EmissionColor"), 0.3f);
        lightObj.intensity = config.lightIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (isHit) return;

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

        acceleration = currentRandomDir * config.baseSpeed + centerForce + heightAdjustment;

        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, config.maxSpeed);

        position += velocity * Time.deltaTime;

        position.y = Mathf.Max(position.y, 1f);
    }

    public int GetScoreAmount() {
        return this.config.points;
    }
}
