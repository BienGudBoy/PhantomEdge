using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Clips")]
    public AudioClip damageClip;
    public AudioClip scoreClip;
    public AudioClip clickClip;
    public AudioClip youDiedClip;
    public AudioClip victoryClip;
    public AudioClip enemyAttackClip;
    [Tooltip("Punch attack sound for Mushroom boss (no weapon)")]
    public AudioClip punchAttackClip;

    [Header("Background Music")]
    [Tooltip("At least 2 music tracks for Scene1. One will be randomly selected.")]
    public AudioClip[] scene1MusicTracks = new AudioClip[2];
    
    [Tooltip("Music track for Mushroom boss in Scene2")]
    public AudioClip scene2MushroomBossMusic;
    
    [Tooltip("Music track for Sword (final) boss in Scene2")]
    public AudioClip scene2SwordBossMusic;

    private AudioSource audioSource;
    private AudioSource musicSource;
    private AudioClip currentMusicClip;

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
            return;
        }

        // Get or add AudioSource component for sound effects
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.LogWarning("AudioManager: AudioSource component was missing and has been added automatically.");
        }

        // Create separate AudioSource for background music
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = true;
        musicSource.playOnAwake = false;
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

    public void PlayVictorySound()
    {
        if (audioSource != null && victoryClip != null)
        {
            audioSource.PlayOneShot(victoryClip);
        }
    }

    public void PlayEnemyAttackSound()
    {
        if (audioSource != null && enemyAttackClip != null)
        {
            audioSource.PlayOneShot(enemyAttackClip);
        }
    }

    public void PlayPunchAttackSound()
    {
        if (audioSource != null && punchAttackClip != null)
        {
            audioSource.PlayOneShot(punchAttackClip);
        }
    }

    /// <summary>
    /// Plays background music for Scene1. Randomly selects from available tracks.
    /// </summary>
    public void PlayScene1Music()
    {
        if (scene1MusicTracks == null || scene1MusicTracks.Length == 0)
        {
            Debug.LogWarning("AudioManager: No Scene1 music tracks assigned!");
            return;
        }

        // Filter out null tracks
        System.Collections.Generic.List<AudioClip> validTracks = new System.Collections.Generic.List<AudioClip>();
        foreach (var track in scene1MusicTracks)
        {
            if (track != null)
            {
                validTracks.Add(track);
            }
        }

        if (validTracks.Count == 0)
        {
            Debug.LogWarning("AudioManager: No valid Scene1 music tracks found!");
            return;
        }

        // Randomly select a track
        int randomIndex = Random.Range(0, validTracks.Count);
        AudioClip selectedTrack = validTracks[randomIndex];

        PlayMusic(selectedTrack);
        Debug.Log($"AudioManager: Playing Scene1 music track {randomIndex + 1}/{validTracks.Count}: {selectedTrack.name}");
    }

    /// <summary>
    /// Plays background music for Scene2 based on boss type.
    /// </summary>
    /// <param name="bossType">The type of boss (Mushroom or Sword)</param>
    public void PlayScene2BossMusic(BossManager.BossType bossType)
    {
        AudioClip bossMusic = null;

        switch (bossType)
        {
            case BossManager.BossType.Mushroom:
                bossMusic = scene2MushroomBossMusic;
                break;
            case BossManager.BossType.Sword:
                bossMusic = scene2SwordBossMusic;
                break;
        }

        if (bossMusic == null)
        {
            Debug.LogWarning($"AudioManager: No music track assigned for {bossType} boss!");
            return;
        }

        PlayMusic(bossMusic);
        Debug.Log($"AudioManager: Playing Scene2 {bossType} boss music: {bossMusic.name}");
    }

    /// <summary>
    /// Stops the currently playing background music.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
            currentMusicClip = null;
        }
    }

    /// <summary>
    /// Plays the specified music clip, stopping any currently playing music first.
    /// </summary>
    private void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null)
        {
            return;
        }

        // Stop current music if playing
        if (musicSource.isPlaying && currentMusicClip == clip)
        {
            // Already playing this track, no need to restart
            return;
        }

        StopMusic();
        currentMusicClip = clip;
        musicSource.clip = clip;
        musicSource.Play();
    }
}
