using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

public class PauseMenu : Menu
{
    public GameObject menuFrame;

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    protected override void Awake()
    {
        base.Awake();
        onShow.AddListener(OnShow);
        onHide.AddListener(OnHide);
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private void OnShow()
    {
        Pause();
        menuFrame.SetActive(true);
    }
    
    private void OnHide()
    {
        Resume();
        menuFrame.SetActive(false);
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void LoadMenu()
    {
        SetShown(false);
        GameManager.Instance.GoToMainMenu();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Resume();
    }

    public void Quit()
    {
        GameManager.Instance.Exit();
    }
}
