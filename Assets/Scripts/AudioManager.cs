using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Tooltip("Blok grid'e yerleştirilince çalar")]
    public AudioClip placeSound;

    [Tooltip("Satır/sütun temizlenince çalar")]
    public AudioClip clearSound;

    private AudioSource audioSource;

    void Awake()
    {
        // Basit Singleton: her yerden AudioManager.Instance ile erişilebilsin
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPlaceSound()
    {
        if (placeSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(placeSound);
        }
    }

    public void PlayClearSound()
    {
        if (clearSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clearSound);
        }
    }
}
