using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Clips")]
    public AudioClip coinClip;
    public AudioClip crashClip;
    public AudioClip musicClip;

    [Header("Volume")]
    [Range(0f, 1f)] public float sfxVolume = 0.8f;
    [Range(0f, 1f)] public float musicVolume = 0.4f;

    private AudioSource sfxSource;
    private AudioSource musicSource;

    void Awake()
    {
        Instance = this;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
    }

    void Start()
    {
        if (musicClip != null)
        {
            musicSource.clip = musicClip;
            musicSource.Play();
        }
    }

    public void PlayCoin()
    {
        if (coinClip != null)
        {
            sfxSource.PlayOneShot(coinClip, sfxVolume);
        }
    }

    public void PlayCrash()
    {
        if (crashClip != null)
        {
            sfxSource.PlayOneShot(crashClip, sfxVolume);
        }
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
}