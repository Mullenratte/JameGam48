using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour {
    public static MusicManager Instance;

    AudioSource audioSource;
    [SerializeField] AudioClip[] mainMenuPlaylist;
    [SerializeField] AudioClip[] gamePlaylist;

    [Range(0f, 1f)] public float musicVolume = 1f;
    public float fadeDuration = 1f;

    private AudioClip[] currentPlaylist;
    private int currentTrackIndex = 0;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
            return;
        }
        audioSource = GetComponent<AudioSource>();  
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy() {
        SceneManager.sceneLoaded -= OnSceneLoaded;

    }

    private void Start() {
        //SwitchPlaylist(mainMenuPlaylist); // Start with menu music
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        if (scene.name == "MainMenu") {
            Time.timeScale = 1f;
            SwitchPlaylist(mainMenuPlaylist);
        } else if (scene.name == "Game") {
            SwitchPlaylist(gamePlaylist);
        }
    }

    public void SwitchPlaylist(AudioClip[] newPlaylist) {
        if (currentPlaylist != null && currentPlaylist[0].name == newPlaylist[0].name) return;

        StopAllCoroutines(); // Stop previous fade if any
        StartCoroutine(TransitionMusic(newPlaylist));
    }

    private IEnumerator TransitionMusic(AudioClip[] newPlaylist) {
        // Fade out
        float time = 0f;
        while (time < fadeDuration) {
            audioSource.volume = Mathf.Lerp(musicVolume, 0, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = 0;

        // Switch playlist
        currentPlaylist = newPlaylist;
        currentTrackIndex = 0;
        // Play first track
        if (currentPlaylist.Length > 0) {
            audioSource.clip = currentPlaylist[currentTrackIndex];
            audioSource.Play();
        }

        // Fade in
        time = 0f;
        while (time < fadeDuration) {
            audioSource.volume = Mathf.Lerp(0, musicVolume, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        audioSource.volume = musicVolume;
    }

    // Optional: Automatically loop through playlist
    private void Update() {
        if (!audioSource.isPlaying && currentPlaylist != null && currentPlaylist.Length > 0) {
            currentTrackIndex = (currentTrackIndex + 1) % currentPlaylist.Length;
            audioSource.clip = currentPlaylist[currentTrackIndex];
            audioSource.Play();
        }
    }
}
