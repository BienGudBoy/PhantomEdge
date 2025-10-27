using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Clips")]
    public AudioClip damageClip;
    public AudioClip scoreClip;
    public AudioClip clickClip;

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

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDamageSound()
    {
        audioSource.PlayOneShot(damageClip);
    }

    public void PlayScoreSound()
    {
        audioSource.PlayOneShot(scoreClip);
    }

    public void PlayClickSound()
    {
        audioSource.PlayOneShot(clickClip);
    }
}
