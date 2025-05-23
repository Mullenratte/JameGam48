using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineCameraShake : MonoBehaviour
{
    public static CinemachineCameraShake Instance;

    [NoSaveDuringPlay] CinemachineCamera cam;
    [NoSaveDuringPlay] [SerializeField] CinemachineBasicMultiChannelPerlin noise;
    [SerializeField] AnimationCurve defaultCurve;
    [SerializeField] float defaultDuration;
    [SerializeField] float defaultIntensity;
    [SerializeField] float intensityFalloffMultiplier; // 

    private void Awake() {
        if (Instance == null) { 
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    public void ScreenshakeDefault() {
        StartCoroutine(Shake(defaultCurve, defaultDuration, defaultIntensity));
    }

    public void ScreenshakeDefault(float duration, float intensity) {
        StartCoroutine(Shake(defaultCurve, duration, intensity));
    }

    public void Screenshake(AnimationCurve curve, float duration, float intensity) {
        StartCoroutine(Shake(curve, duration, intensity));
    }


    IEnumerator Shake(AnimationCurve curve, float duration, float intensity) {
        Vector3 startPos = transform.position;
        float elapsed = 0f;
        float intensityFalloff = intensityFalloffMultiplier * Mathf.Abs(PlayerMovement.Instance.transform.position.z - GameOverZone.Instance.transform.position.z);

        while (elapsed < duration) {
            elapsed += Time.deltaTime;
            float strength = curve.Evaluate(elapsed / duration);

            noise.AmplitudeGain = strength * Mathf.Max(0f, intensity - intensityFalloff);
            yield return null;
        }

        noise.AmplitudeGain = 0;
    }
}
