using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // Singleton

    public AudioSource audioSource; // Referencia al AudioSource
    public AudioClip backgroundMusic; // Canción a reproducir

    void Awake()
    {
        // Singleton: Si ya existe un AudioManager, lo destruimos
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // No destruir entre escenas
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Configuración del AudioSource
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = backgroundMusic;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.Play();
    }
}