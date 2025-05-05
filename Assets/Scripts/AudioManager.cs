using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    private AudioSource audioSource;
    private float defaultVolume = 0.5f;

    private void Awake()
    {
        Debug.Log("AudioManager.Awake called");

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            Debug.Log($"Duplicate AudioManager destroyed: {gameObject.name}");
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log($"AudioManager initialized: GameObject = {gameObject.name}");

        // Use existing AudioSource if present, otherwise add one
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("Added new AudioSource to AudioManager");
        }
        else
        {
            Debug.Log("Using existing AudioSource on AudioManager");
        }

        // Stop other AudioSources playing BackgroundMusic
        AudioSource[] allAudioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in allAudioSources)
        {
            if (source != null && source.clip != null && source.clip.name.Contains("BackgroundMusic") && source.gameObject != gameObject)
            {
                source.Stop();
                Debug.Log($"Stopped AudioSource on {source.gameObject.name} playing {source.clip.name}");
            }
        }

        audioSource.loop = true;
        audioSource.volume = defaultVolume;

        // Load music clip if not already set
        if (audioSource.clip == null || !audioSource.clip.name.Contains("BackgroundMusic"))
        {
            AudioClip clip = Resources.Load<AudioClip>("Audio/BackgroundMusic");
            if (clip != null)
            {
                audioSource.clip = clip;
                Debug.Log($"Loaded BackgroundMusic clip: {clip.name}");
            }
            else
            {
                Debug.LogWarning("Background music clip not found in Resources/Audio/BackgroundMusic");
            }
        }

        // Start playing if not already
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }

        Debug.Log($"AudioSource initialized: clip = {audioSource.clip?.name}, playing = {audioSource.isPlaying}, volume = {audioSource.volume}");

        // Load saved mute state
        bool isMuted = PlayerPrefs.GetInt("SoundMuted", 0) == 1;
        audioSource.volume = isMuted ? 0f : defaultVolume;
        Debug.Log($"AudioManager mute state loaded: volume = {audioSource.volume}, SoundMuted = {PlayerPrefs.GetInt("SoundMuted", 0)}");
    }

    public void SetSoundMuted(bool isMuted)
    {
        if (audioSource != null)
        {
            audioSource.volume = isMuted ? 0f : defaultVolume;
            PlayerPrefs.SetInt("SoundMuted", isMuted ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log($"SetSoundMuted: volume = {audioSource.volume}, SoundMuted = {PlayerPrefs.GetInt("SoundMuted", 0)}, playing = {audioSource.isPlaying}");
        }
        else
        {
            Debug.LogError("AudioSource is null in SetSoundMuted, cannot mute");
            PlayerPrefs.SetInt("SoundMuted", isMuted ? 1 : 0);
            PlayerPrefs.Save();
            Debug.Log($"SetSoundMuted fallback: SoundMuted = {PlayerPrefs.GetInt("SoundMuted", 0)}");
        }
    }
}