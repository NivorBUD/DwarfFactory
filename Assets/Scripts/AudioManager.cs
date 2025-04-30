using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioMixer audioMixer;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            ApplySettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ApplySettings()
    {
        float volume = PlayerPrefs.GetFloat("volume", 1f);
        bool isMuted = PlayerPrefs.GetInt("muted", 0) == 1;

        audioMixer.SetFloat("MasterVolume", isMuted ? -80f : Mathf.Log10(volume) * 20);
    }

    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("volume", volume);
        if (!IsMuted())
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void ToggleMute(bool mute)
    {
        PlayerPrefs.SetInt("muted", mute ? 1 : 0);
        audioMixer.SetFloat("MasterVolume", mute ? -80f : Mathf.Log10(GetVolume()) * 20);
    }

    public float GetVolume() => PlayerPrefs.GetFloat("volume", 1f);
    public bool IsMuted() => PlayerPrefs.GetInt("muted", 0) == 1;
}