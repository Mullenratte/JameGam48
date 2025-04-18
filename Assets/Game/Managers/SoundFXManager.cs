using UnityEngine;

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
