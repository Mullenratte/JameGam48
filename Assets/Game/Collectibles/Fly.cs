using Unity.VisualScripting;
using UnityEngine;

public class Fly : MonoBehaviour, ILickable
{
    // Start is called once before the first execution of Update after the BaseItem is created
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 acceleration;
    bool isEaten = false;   

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
        position = transform.position;
        position.y = 0.5f;
        transform.position = position;
        transform.localScale = new Vector3(config.size, config.size, config.size);
    }

    // Update is called once per frame
    void Update()
    {
        if (isEaten) return;

        // Update position

    }

    void FlyMovement()
    {

    }
}
