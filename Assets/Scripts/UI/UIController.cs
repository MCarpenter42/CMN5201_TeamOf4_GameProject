using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class UIController : Core
{
    #region [ OBJECTS ]

    [Header("UI Components")]
    public MainMenu mainMenu;
    public PauseMenu pauseMenu;
    public HUD hud;
    public Image blackScreen;
    public DeveloperConsole devConsole;
    public SFXSource sfx;

    [Header("Performance & Debug")]
    [SerializeField] DebugDisplay debugPanelInfo = DebugDisplay.None;
    public GameObject debugPanel;
    private TMP_Text debugHeader;
    private TMP_Text debugInfo;
    [HideInInspector] public float fps;

    #endregion

    #region [ PROPERTIES ]

    public Color blackoutColour = Color.black;
    [SerializeField] bool blackoutOnLoad = true;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        GetComponents();
        if (blackScreen != null)
        {
            blackScreen.gameObject.SetActive(true);
            if (blackoutOnLoad)
            {
                blackScreen.color = blackoutColour;
            }
        }
    }

    void Start()
    {
        if (debugPanel != null)
        {
            if (debugPanelInfo != DebugDisplay.None)
            {
                debugPanel.SetActive(true);
                TextMeshProUGUI label = GetChildrenWithComponent<TextMeshProUGUI>(debugPanel)[1].GetComponent<TextMeshProUGUI>();
                if (debugPanelInfo == DebugDisplay.FPS)
                {
                    label.text = "FPS:";
                }
            }
            else
            {
                debugPanel.SetActive(false);
            }
        }
    }
	
    void Update()
    {
        if (debugPanelInfo != DebugDisplay.None)
        {
            UpdateDebugDisplay();
        }
    }

    void FixedUpdate()
    {
        
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
    
    public void UIInputs()
    {
        if (Input.GetKeyDown(Controls.General.Pause.Key) && pauseMenu != null)
        {
            pauseMenu.SetShown(!GameManager.isPaused);
        }
    }

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

        if (debugPanel != null && GetChildrenWithComponent<TMP_Text>(debugPanel).Count > 1)
        {
            debugHeader = GetChildrenWithComponent<TMP_Text>(debugPanel)[0].GetComponent<TMP_Text>();
            debugInfo = GetChildrenWithComponent<TMP_Text>(debugPanel)[1].GetComponent<TMP_Text>();
        }
    }

    public void SetDebugType(DebugDisplay dbType)
    {
        debugPanelInfo = dbType;
        if (debugPanel != null)
        {
            switch (dbType)
            {
                case DebugDisplay.None:
                default:
                    debugPanel.SetActive(false);
                    break;

                case DebugDisplay.FPS:
                    debugPanel.SetActive(true);
                    debugHeader.text = "FPS:";
                    break;
            }
        }
    }

    public void UpdateDebugDisplay()
    {
        switch (debugPanelInfo)
        {
            case DebugDisplay.None:
            default:
                break;

            case DebugDisplay.FPS:
                debugInfo.text = fps.ToString();
                break;
        }
    }

    public void BlackScreenFade(bool show, float duration)
    {
        StartCoroutine(IBlackScreenFade(show, duration));
    }

    private IEnumerator IBlackScreenFade(bool show, float duration)
    {
        blackScreen.gameObject.SetActive(true);

        Color clrStart = new Color(blackoutColour.r, blackoutColour.g, blackoutColour.b, 0.000f);
        Color clrEnd = new Color(blackoutColour.r, blackoutColour.g, blackoutColour.b, 0.000f);

        if (show)
        {
            clrEnd.a = 1.000f;
        }
        else
        {
            clrStart.a = 1.000f;
        }

        float timePassed = 0.0f;
        while (timePassed < duration)
        {
            yield return null;
            timePassed += Time.deltaTime;
            float delta = timePassed / duration;
            blackScreen.color = Color.Lerp(clrStart, clrEnd, delta);
        }
        blackScreen.color = clrEnd;
    }
}
