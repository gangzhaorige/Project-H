using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;

    [Header("BGM Clips")]
    public AudioClip loginBGM;
    public AudioClip champSelectBGM;
    public AudioClip gameplayBGM;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayBGMForCurrentScene();
    }

    private void PlayBGMForCurrentScene()
    {
        if (bgmSource == null) return;

        string sceneName = SceneManager.GetActiveScene().name;
        AudioClip clipToPlay = null;

        // Automatically select clip based on scene name
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
            bgmSource.clip = clipToPlay;
            bgmSource.loop = true;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"[AudioManager] No BGM clip assigned for scene: {sceneName}");
        }
    }
}
