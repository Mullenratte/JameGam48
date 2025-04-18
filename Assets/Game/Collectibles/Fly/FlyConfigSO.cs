using UnityEngine;

[CreateAssetMenu(fileName = "FlyConfigSO", menuName = "Scriptable Objects/FlyConfigSO")]
public class FlyConfigSO : ScriptableObject
{
    public float size;
    public float flyRadius;
    public float maxSpeed;
    public int points;

    public AudioClip[] OnHitClips;
    public AudioClip[] OnCollectedClips;
}
