using UnityEngine;
using PrimeTween;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Clips")]
    public AudioClip backgroundMusic;
    public AudioClip shootClip;
    public AudioClip enemyHitClip;

    private Tween musicFadeTween;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (backgroundMusic != null)
        {
            PlayMusic(backgroundMusic);
        }
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1.0f)
    {
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        if (fadeDuration > 0f && musicSource.isPlaying)
        {
            musicFadeTween.Stop();
            musicFadeTween = Tween.AudioVolume(musicSource, 0f, fadeDuration * 0.5f)
                .OnComplete(() =>
                {
                    musicSource.clip = clip;
                    musicSource.Play();
                    musicFadeTween = Tween.AudioVolume(musicSource, 1f, fadeDuration * 0.5f);
                });
        }
        else
        {
            musicSource.clip = clip;
            musicSource.volume = 1f;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip, float volume = 1.0f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlaySFXWithPitchVariation(AudioClip clip, float minPitch = 0.9f, float maxPitch = 1.1f)
    {
        if (clip == null) return;
        sfxSource.pitch = Random.Range(minPitch, maxPitch);
        sfxSource.PlayOneShot(clip);
    }
}