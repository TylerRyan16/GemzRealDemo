using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject staticUI;
    public GameObject shopUI;
    public GameObject closeShopUI;
    public GameObject videoSettings;
    public GameObject audioSettings;

    public bool isPaused;

    private void Start()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void Update()
    {
        // if paused
        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CloseAllMenus();
            }
        } 
        // if not paused
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OpenPauseMenu();
            }
        }
    }

    public void OpenPauseMenu()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
        staticUI.SetActive(false);
        shopUI.SetActive(false);
        closeShopUI.SetActive(false);

    }

    public void ClosePauseMenu()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        staticUI.SetActive(true);
        shopUI.SetActive(true);
        closeShopUI.SetActive(true);
    }

    public void OpenSettingsMenu()
    {
        isPaused = true;
        settingsMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    public void CloseSettingsMenu()
    {
        isPaused = true;
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void CloseAllMenus()
    {
        isPaused = false;
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        videoSettings.SetActive(false);
        audioSettings.SetActive(false);
        staticUI.SetActive(true);
        shopUI.SetActive(true);
        closeShopUI.SetActive(true);
    }

    public void OpenVideoSettings()
    {
        settingsMenu.SetActive(false);
        videoSettings.SetActive(true);
    }

    public void CloseVideoSettings()
    {
        videoSettings.SetActive(false);
        settingsMenu.SetActive(true);
    }

    public void OpenAudioSettings()
    {
        settingsMenu.SetActive(false);
        audioSettings.SetActive(true);

    }

    public void CloseAudioSettings()
    {
        audioSettings.SetActive(false);
        settingsMenu.SetActive(true);
    }


}
