using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class UIController : UI
{
    #region [ OBJECTS ]

    private enum PerfDebugDisplay { None, FPS };

    [Header("UI Components")]
    public HUD hud;
    public PauseMenu pauseMenu;

    [Header("Performance & Debug")]
    [SerializeField] PerfDebugDisplay debugPanelInfo = PerfDebugDisplay.None;
    public GameObject debugDisplay;
    [HideInInspector] public float fps;

    #endregion

    #region [ PROPERTIES ]



    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        GetComponents();
    }

    void Start()
    {
        if (debugDisplay != null)
        {
            if (debugPanelInfo != PerfDebugDisplay.None)
            {
                debugDisplay.SetActive(true);
                TextMeshProUGUI label = GetChildrenWithComponent<TextMeshProUGUI>(debugDisplay)[1].GetComponent<TextMeshProUGUI>();
                if (debugPanelInfo == PerfDebugDisplay.FPS)
                {
                    label.text = "FPS:";
                }
            }
            else
            {
                debugDisplay.SetActive(false);
            }
        }
    }
	
    void Update()
    {
        if (debugPanelInfo != PerfDebugDisplay.None)
        {
            UpdateDebugDisplay();
        }
    }

    void FixedUpdate()
    {
        
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private void GetComponents()
    {
        if (GameManager.LevelController.isGameplayLevel)
        {
            if (hud == null && GetChildrenWithComponent<HUD>(gameObject).Count > 0)
            {
                hud = GetChildrenWithComponent<HUD>(gameObject)[0].GetComponent<HUD>();
            }
            if (pauseMenu == null)
            {
                if (GetChildrenWithComponent<PauseMenu>(gameObject).Count > 0)
                {
                    pauseMenu = GetChildrenWithComponent<PauseMenu>(gameObject)[0].GetComponent<PauseMenu>();
                    pauseMenu.menuFrame.SetActive(false);
                }
            }
            else
            {
                pauseMenu.menuFrame.SetActive(false);
            }
        }
    }

    public void UpdateDebugDisplay()
    {
        TextMeshProUGUI textObj = GetChildrenWithComponent<TextMeshProUGUI>(debugDisplay)[0].GetComponent<TextMeshProUGUI>();
        if (debugPanelInfo == PerfDebugDisplay.FPS)
        {
            textObj.text = "FPS: " + fps.ToString();
        }
    }
}
