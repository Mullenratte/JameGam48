using UnityEngine;

public class Fly : BaseItem
{
    // Start is called once before the first execution of Update after the BaseItem is created
    public Vector3 position;
    public Vector3 velocity;
    public Vector3 acceleration;

    FlyConfigSO config;

    void Start()
    {
        position = transform.position;
        position.y = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
