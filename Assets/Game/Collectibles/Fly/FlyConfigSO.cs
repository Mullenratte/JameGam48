using UnityEngine;

[CreateAssetMenu(fileName = "FlyConfigSO", menuName = "Scriptable Objects/FlyConfigSO")]
public class FlyConfigSO : ScriptableObject
{
    [Range(0f, 0.99f)] public float spawnChance;
    public float size;
    public float flyRadius;
    public float baseSpeed;
    public float maxSpeed;
    public int points;

    public Material material;
    public float lightIntensity;

    public AudioClip[] OnHitClips;
    public AudioClip[] OnCollectedClips;
}
