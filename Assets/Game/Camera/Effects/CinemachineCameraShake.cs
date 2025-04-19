using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraShake : MonoBehaviour
{
    public static CinemachineCameraShake Instance;

    CinemachineCamera cam;
    [SerializeField] CinemachineBasicMultiChannelPerlin noise;
    [SerializeField] AnimationCurve defaultCurve;
    [SerializeField] float defaultDuration = 0.7f;
    [SerializeField] float defaultIntensity = 1.0f;
    [SerializeField] float intensityFalloffMultiplier; // 

    private void Awake() {
        if (Instance == null) { 
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void ScreenshakeDefault() {
        Debug.Log("her");
        StartCoroutine(Shake(defaultCurve, defaultDuration, defaultIntensity));
    }

    public void Screenshake(AnimationCurve curve, float duration, float intensity) {
        StartCoroutine(Shake(curve, duration, intensity));
    }


    IEnumerator Shake(AnimationCurve curve, float duration, float intensity) {
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        float intensityFalloff = intensityFalloffMultiplier * Mathf.Abs(PlayerMovement.Instance.transform.position.z - GameOverZone.Instance.transform.position.z);

        Debug.Log("falloff " + intensityFalloff);
        Debug.Log("overall " + (intensity - intensityFalloff));
        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float strength = curve.Evaluate(elapsed / duration);

            noise.AmplitudeGain = strength * Mathf.Max(0f, intensity - intensityFalloff);
            yield return null;
        }

        noise.AmplitudeGain = 0;
    }
}
