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

    private enum PerfDebugDisplay { None, FPS, MoveTime, MoveFrames, MoveAnimInfo, CoroutineFrameTime };

    [Header("UI Components")]
    public HUD hud;
    public PauseMenu pauseMenu;

    [Header("Performance & Debug")]
    [SerializeField] PerfDebugDisplay debugPanelInfo = PerfDebugDisplay.None;

    public GameObject debugDisplay;
    [HideInInspector] public float fps;
    [HideInInspector] public float moveTime;
    [HideInInspector] public int moveFrames;
    [HideInInspector] public string moveAnimInfo;
    [HideInInspector] public float coroutineFrameTime;

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
        if (debugPanelInfo != PerfDebugDisplay.None)
        {
            debugDisplay.SetActive(true);
            TextMeshProUGUI label = GetChildrenWithComponent<TextMeshProUGUI>(debugDisplay)[1].GetComponent<TextMeshProUGUI>();
            if (debugPanelInfo == PerfDebugDisplay.FPS)
            {
                label.text = "FPS:";
            }
            else if (debugPanelInfo == PerfDebugDisplay.MoveTime)
            {
                label.text = "Movement Time:";
            }
            else if (debugPanelInfo == PerfDebugDisplay.MoveFrames)
            {
                label.text = "Movement Frames:";
            }
            else if (debugPanelInfo == PerfDebugDisplay.MoveAnimInfo)
            {
                label.text = "Move Anim Info:";
            }
            else if (debugPanelInfo == PerfDebugDisplay.CoroutineFrameTime)
            {
                label.text = "Coroutine Frame Time:";
            }
        }
        else
        {
            debugDisplay.SetActive(false);
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
            if (hud == null)
            {
                hud = GetChildrenWithComponent<HUD>(gameObject)[0].GetComponent<HUD>();
            }
            if (pauseMenu == null)
            {
                pauseMenu = GetChildrenWithComponent<PauseMenu>(gameObject)[0].GetComponent<PauseMenu>();
            }
            pauseMenu.menuFrame.SetActive(false);
        }
    }

    public void UpdateDebugDisplay()
    {
        TextMeshProUGUI textObj = GetChildrenWithComponent<TextMeshProUGUI>(debugDisplay)[0].GetComponent<TextMeshProUGUI>();
        if (debugPanelInfo == PerfDebugDisplay.FPS)
        {
            textObj.text = "FPS: " + fps.ToString();
        }
        else if (debugPanelInfo == PerfDebugDisplay.MoveTime)
        {
            textObj.text = moveTime.ToString();
        }
        else if (debugPanelInfo == PerfDebugDisplay.MoveFrames)
        {
            textObj.text = moveFrames.ToString();
        }
        else if (debugPanelInfo == PerfDebugDisplay.MoveAnimInfo)
        {
            textObj.text = moveAnimInfo;
        }
        else if (debugPanelInfo == PerfDebugDisplay.CoroutineFrameTime)
        {
            textObj.text = coroutineFrameTime.ToString();
        }
    }
}
