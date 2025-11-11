using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject PauseMenuUI;

    private void OnEnable()
    {
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnPauseToggle += HandlePauseToggle;
        }
    }

    private void OnDisable()
    {
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.OnPauseToggle -= HandlePauseToggle;
        }
    }

    private void HandlePauseToggle()
    {
        if (GameIsPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }
    public void Resume()
    {
        if (PauseMenuUI != null)
            PauseMenuUI.SetActive(false);

        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        if (PauseMenuUI != null)
            PauseMenuUI.SetActive(true);

        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;

        if (Application.CanStreamedLevelBeLoaded("StartMenu"))
        {
            SceneManager.LoadScene("StartMenu");
        }
        else
        {
            Debug.LogError("StartMenu scene not found! Check if it's added to Build Settings.");
        }
    }
}
