using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class SoundFXManager : Singleton<SoundFXManager>
{
    [SerializeField] private AudioSource soundFXObject;


    public void PlaySoundFXClip(AudioClip clip, Vector3 spawnPos, float volume, float minPitch, float maxPitch) {
        if (clip == null) return;

        AudioSource audioSource = Instantiate(soundFXObject, spawnPos, Quaternion.identity);

        audioSource.clip = clip;
        audioSource.volume = volume;

        audioSource.pitch = Random.Range(minPitch, maxPitch);

        audioSource.Play();

        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);
    }

    public AudioSource PlaySoundFXClipContinuously(AudioClip clip, Transform parent, float volume, float minPitch, float maxPitch) {
        if (clip == null) return null;

        AudioSource audioSource = Instantiate(soundFXObject, parent.position, Quaternion.identity);
        audioSource.transform.SetParent(parent);

        audioSource.loop = true;
        audioSource.clip = clip;
        audioSource.volume = volume;

        audioSource.Play();

        return audioSource;
        //float clipLength = audioSource.clip.length;
        //StartCoroutine(ChangePitchOfPlayingClipRecursively(audioSource, minPitch, maxPitch, clipLength));
        //Destroy(audioSource.gameObject, clipLength);
    }

    IEnumerator ChangePitchOfPlayingClipRecursively(AudioSource source, float minPitch, float maxPitch, float seconds) {
        yield return new WaitForSeconds(seconds);
        source.pitch = Random.Range(minPitch, maxPitch);
        yield return ChangePitchOfPlayingClipRecursively(source, minPitch, maxPitch, seconds);
    }

    IEnumerator ChangePitchOfPlayingClipAfterTime(AudioSource source, float minPitch, float maxPitch, float seconds) {
        yield return new WaitForSeconds(seconds);
        source.pitch = Random.Range(minPitch, maxPitch);
        yield return null;
    }

    public void PlayRandomSoundFXClipPitchVariation(AudioClip[] clips, Vector3 spawnPos, float volume, float minPitch, float maxPitch) {
        if (clips.Length == 0) return;
        int rnd = Random.Range(0, clips.Length);

        if(clips.Length > 0) PlaySoundFXClip(clips[rnd], spawnPos, volume, minPitch, maxPitch);
    }

    public void PlayRandomSoundFXClip(AudioClip[] clips, Vector3 spawnPos, float volume) {
        if (clips.Length == 0) return;

        int rnd = Random.Range(0, clips.Length);

        if (clips.Length > 0) PlaySoundFXClip(clips[rnd], spawnPos, volume, 1f, 1f);
    }


}
