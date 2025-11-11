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

    [Header("Audio Clips")]
   
    public AudioClip shootPortalClip;
    public AudioClip playerHurtClip;
    public AudioClip playerDieClip;
    public AudioClip playerDieLavaClip;
    public AudioClip pickupCubeClip;
    public AudioClip throwCubeClip;
    public AudioClip releaseCubeClip;
    public AudioClip doorOpenClip;
    public AudioClip doorCloseClip;
    public AudioClip buttonPressClip;
    public AudioClip mechanismActivateClip;
    public AudioClip mechanismDeactivateClip;

    private Dictionary<string, AudioClip> soundDictionary;
    private string currentExclusiveSound = null; //para poder reproducir solo un sonido a la vez si queremos

    //TODO: Falta añadir SFX del laser de torreta y reemplazar todos los sonidos (ahora mismo todos son placeholders)
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

        // Inicializamos diccionario para acceso rápido
        soundDictionary = new Dictionary<string, AudioClip>()
        {
            {"player_hurt", playerHurtClip},
            {"player_die", playerDieClip},
            {"player_die_lava", playerDieLavaClip},
            {"cube_pickup", pickupCubeClip},
            {"cube_throw", throwCubeClip},
            {"cube_release", releaseCubeClip},
            {"door_open", doorOpenClip},
            {"door_close", doorCloseClip},
            {"button_press", buttonPressClip},
            {"mechanism_activate", mechanismActivateClip},
            {"mechanism_deactivate", mechanismDeactivateClip}
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
