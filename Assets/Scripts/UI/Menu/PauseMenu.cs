using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : Core
{
    // Having a cariable local to this script to log whether the game is paused isn't
    // necessary - use GameManager.isPaused instead!
    //public static bool gamePaused = false;
    public GameObject pauseMenuUI;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(Controls.General.Pause.Key))
        {
            if (GameManager.isPaused)
            {
                PauseMenuResume();
            }
            else
            {
                PauseMenuPause();
            }
        }
 
    }

    public void PauseMenuResume()
    {
        /*pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        gamePaused = false;*/
        pauseMenuUI.SetActive(false);
        Resume();
    }

    public void PauseMenuPause()
    {
        /*pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        gamePaused = true;*/
        Pause();
        pauseMenuUI.SetActive(true);
    }

    public void LoadMenu()
    {
        Debug.Log("Test");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
