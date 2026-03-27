using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("BGM Clips")]
    public AudioClip loginBGM;
    public AudioClip champSelectBGM;
    public AudioClip gameplayBGM;

    [Header("SFX Clips")]
    public AudioClip cardHoverClip;
    public AudioClip cardDrawClip;

    private Dictionary<int, AudioClip> cardAudioMap = new Dictionary<int, AudioClip>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    private void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayBGMForScene(scene.name);
    }

    private void Start()
    {
        // Initial play for the first scene
        PlayBGMForScene(SceneManager.GetActiveScene().name);
    }

    private void PlayBGMForScene(string sceneName)
    {
        if (bgmSource == null) return;

        AudioClip clipToPlay = null;

        switch (sceneName)
        {
            case "Login":
            case "Start":
                clipToPlay = loginBGM;
                break;
            case "ChampSelect":
                clipToPlay = champSelectBGM;
                break;
            case "Game":
                clipToPlay = gameplayBGM;
                break;
        }

        if (clipToPlay != null)
        {
            ChangeBGM(clipToPlay);
        }
        else
        {
            // Optional: stop music if entering a scene with no music (like Lobby)
            // bgmSource.Stop();
            Debug.Log($"[AudioManager] No BGM clip assigned for scene: {sceneName}");
        }
    }

    public void ChangeBGM(AudioClip newClip)
    {
        if (bgmSource == null || newClip == null) return;

        // Only change if it's a different clip
        if (bgmSource.clip == newClip && bgmSource.isPlaying) return;

        bgmSource.Stop();
        bgmSource.clip = newClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlayCardHoverSFX()
    {
        if (sfxSource != null && cardHoverClip != null)
        {
            sfxSource.PlayOneShot(cardHoverClip);
        }
    }

    public void PlayCardDrawSFX()
    {
        if (sfxSource != null && cardDrawClip != null)
        {
            sfxSource.PlayOneShot(cardDrawClip);
        }
    }

    public void PlayCardMoveSFX()
    {
        if (sfxSource != null && cardDrawClip != null)
        {
            sfxSource.PlayOneShot(cardDrawClip);
        }
    }

    public void PlayCardAudio(int cardType)
    {
        if (cardAudioMap.TryGetValue(cardType, out AudioClip clip))
        {
            if (sfxSource != null && clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
            return;
        }

        // Try to load from the paths mentioned in prompt or found in project
        clip = Resources.Load<AudioClip>($"Audio/SFX/BasicCharacterAudio/{cardType}");
        if (clip == null) clip = Resources.Load<AudioClip>($"Audios/CardAudio/{cardType}");

        if (clip != null)
        {
            cardAudioMap[cardType] = clip;
            if (sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }
        else
        {
            Debug.LogWarning($"[AudioManager] No audio clip found for cardType: {cardType}");
        }
    }

    public void PlaySkillAudio(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}
