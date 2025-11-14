using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [Header("Ambience")]
    public AudioClip ambientClip;

    [Header("Audio Clips:")]
    [Header("-------------")]

    [Space(5)]

    [Header("Portal Gun Sounds")]
    public AudioClip shootBluePortalClip;
    public AudioClip shootOrangePortalClip;
    public AudioClip portalBlueOpen;
    public AudioClip portalOrangeOpen;
    public AudioClip portalInvalidSurface;
    public AudioClip portalCross;

    [Header("Player Sounds")]
    public AudioClip playerHurtClip;
    public AudioClip playerDieClip;
    public AudioClip playerDrownClip;

    [Header("Cube Sounds")]
    public AudioClip pickupCubeClip;
    public AudioClip throwCubeClip;
    public AudioClip cubeSpawn;
    public AudioClip cubeHoldLoop;

    [Header("Door Sounds")]
    public AudioClip doorOpenClip;
    public AudioClip doorCloseClip;

    [Header("Map Sounds")]
    public AudioClip buttonPressClip;
    public AudioClip mechanismActivateClip;
    public AudioClip mechanismDeactivateClip;
    public AudioClip checkpoint;

    [Header("Turret Sounds")]
    public AudioClip turretLaserStart;
    public AudioClip turretLaserLoop;
    public AudioClip turretDie;

    [Header("Winning Sounds")]
    public AudioClip confeti;
    public AudioClip oiia;

    private Dictionary<string, AudioClip> soundDictionary;
    private string currentExclusiveSound = null;

    private void Awake()
    {
        // Lo hacemos singleton (solo un sound manager en escena)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        soundDictionary = new Dictionary<string, AudioClip>()
        {
            // Portal Gun Sounds
            {"shoot_blue_portal", shootBluePortalClip},
            {"shoot_orange_portal", shootOrangePortalClip},
            {"portal_blue_open", portalBlueOpen},
            {"portal_orange_open", portalOrangeOpen},
            {"portal_invalid_surface", portalInvalidSurface},
            {"portal_cross", portalCross},

            // Player Sounds
            {"player_hurt", playerHurtClip},
            {"player_die", playerDieClip},
            {"player_drown", playerDrownClip},

            // Cube Sounds
            {"cube_pickup", pickupCubeClip},
            {"cube_throw", throwCubeClip},
            {"cube_spawn", cubeSpawn},
            {"cube_holding_loop", cubeHoldLoop},

            // Door Sounds
            {"door_open", doorOpenClip},
            {"door_close", doorCloseClip},

            // Map Sounds
            {"button_press", buttonPressClip},
            {"mechanism_activate", mechanismActivateClip},
            {"mechanism_deactivate", mechanismDeactivateClip},
            {"checkpoint", checkpoint},

            // Turret Sounds
            {"turret_laser_start", turretLaserStart},
            {"turret_laser_loop", turretLaserLoop},
            {"turret_die", turretDie},

            // Win Sounds
            {"confeti", confeti},
            {"oiia", oiia},

            // Ambience
            {"radio_loop", ambientClip }
        };
    }

    public void PlaySFX(string soundName)
    {
        if (currentExclusiveSound != null && currentExclusiveSound != soundName)
            return;

        if (soundDictionary.ContainsKey(soundName) && soundDictionary[soundName] != null)
            sfxSource.PlayOneShot(soundDictionary[soundName]);
        else
            Debug.LogWarning($"SoundManager: No se ha encontrado el sonido '{soundName}'");
    }

    public void PlayExclusiveSFX(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName) && soundDictionary[soundName] != null)
        {
            sfxSource.Stop();
            currentExclusiveSound = soundName;

            sfxSource.PlayOneShot(soundDictionary[soundName]);
            StartCoroutine(ClearExclusiveAfterClip(soundDictionary[soundName].length));
        }
        else
        {
            Debug.LogWarning($"SoundManager: No se ha encontrado el sonido '{soundName}'");
        }
    }

    public void PauseLoop()
    {
        musicSource.Pause();
        sfxSource.Pause();
    }

    public void ResumeLoop()
    {
        musicSource.UnPause();
        sfxSource.UnPause();
    }

    public void StopLoop()
    {
        sfxSource.Stop();
        sfxSource.loop = false;
        sfxSource.clip = null;
    }


    private System.Collections.IEnumerator ClearExclusiveAfterClip(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentExclusiveSound = null;

    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayAmbience()
    {
        if (ambientClip != null)
        {
            PlayMusic(ambientClip, true);
        }
        else
        {
            Debug.LogWarning("No ambient clip assigned in SoundManager!");
        }
    }

}
