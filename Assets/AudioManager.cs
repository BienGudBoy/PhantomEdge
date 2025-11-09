using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Clips")]
    public AudioClip damageClip;
    public AudioClip scoreClip;
    public AudioClip clickClip;
    public AudioClip youDiedClip;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Get or add AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("AudioManager: AudioSource component was missing and has been added automatically.");
        }
    }

    public void PlayDamageSound()
    {
        if (audioSource != null && damageClip != null)
        {
            audioSource.PlayOneShot(damageClip);
        }
    }

    public void PlayScoreSound()
    {
        if (audioSource != null && scoreClip != null)
        {
            audioSource.PlayOneShot(scoreClip);
        }
    }

    public void PlayClickSound()
    {
        if (audioSource != null && clickClip != null)
        {
            audioSource.PlayOneShot(clickClip);
        }
    }

    public void PlayYouDiedSound()
    {
        if (audioSource != null && youDiedClip != null)
        {
            audioSource.PlayOneShot(youDiedClip);
        }
    }
}
