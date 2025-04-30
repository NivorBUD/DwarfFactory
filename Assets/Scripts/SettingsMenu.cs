using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public Toggle muteToggle;
    public Dropdown resolutionDropdown;
    public AudioMixer audioMixer;

    private Resolution[] resolutions;

    void Start()
    {
        // Настройка громкости
        float savedVolume = PlayerPrefs.GetFloat("volume", 1f);
        volumeSlider.value = savedVolume;
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(savedVolume) * 20);

        // Настройка выключения звука
        bool isMuted = PlayerPrefs.GetInt("muted", 0) == 1;
        muteToggle.isOn = isMuted;
        audioMixer.SetFloat("MasterVolume", isMuted ? -80f : Mathf.Log10(savedVolume) * 20);

        // Настройка разрешений
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        int currentResIndex = 0;
        var options = new System.Collections.Generic.List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetVolume(float volume)
    {
        PlayerPrefs.SetFloat("volume", volume);
        if (!muteToggle.isOn)
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void ToggleMute(bool isMuted)
    {
        PlayerPrefs.SetInt("muted", isMuted ? 1 : 0);
        if (isMuted)
            audioMixer.SetFloat("MasterVolume", -80f);
        else
            SetVolume(volumeSlider.value);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution res = resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void BackToMainMenu(GameObject settingsPanel, GameObject mainMenuPanel)
    {
        settingsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}