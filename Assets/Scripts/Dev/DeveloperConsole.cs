using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using TMPro;

public class DeveloperConsole : Core
{
    #region [ PROPERTIES ]

    [Header("Components")]
    [SerializeField] RectTransform overlayRect;
    [HideInInspector] public UIElement console;
    [SerializeField] TMP_Text logReadout;
    [SerializeField] TMP_InputField inputBox;
    [SerializeField] GameObject showHideButton;

    [Header("Console Properties")]
    [SerializeField] float fixedShowHideDelay = 0.0f;
    [SerializeField] UIElement.ShowHide showHideType = UIElement.ShowHide.Slide;
    [SerializeField] float transitionTime = 0.1f;
    [SerializeField] InterpDelta.InterpTypes slideMovementStyle = InterpDelta.InterpTypes.Linear;
    [SerializeField] Vector2 visiblePos = Vector2.zero;
    [SerializeField] Vector2 hiddenOffset = Vector2.zero;

    [SerializeField] UnityEvent onShow = new UnityEvent();
    [SerializeField] UnityEvent onHide = new UnityEvent();

    private string[] logLines = new string[24];
    private bool logConsoleState = false;

    #endregion

    #region [ COMMANDS ]



    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        GetComponents();
    }

    void Start()
    {
        console.SetShown(false, UIElement.ShowHide.Instant);

        showHideButton.SetActive(false);
#if UNITY_EDITOR
        showHideButton.SetActive(true);
#endif
    }
	
    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private void GetComponents()
    {
        console = GetOrAddComponent<UIElement>(overlayRect.gameObject);
        console.SetShowHideTransition(fixedShowHideDelay, showHideType, transitionTime, slideMovementStyle);
        if (visiblePos != Vector2.zero)
        {
            console.SetShowHidePositions(hiddenOffset, visiblePos);
        }
        else
        {
            console.SetHiddenOffset(hiddenOffset);
        }
        if (onShow != null)
        {
            console.SetOnShowEvent(onShow);
        }
        if (onHide != null)
        {
            console.SetOnHideEvent(onHide);
        }
        console.GetGenericComponents();
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    public void ToggleConsole()
    {
        if (!console.gameObject.activeSelf)
        {
            console.gameObject.SetActive(true);
        }
        console.SetShown(!console.visible, showHideType);
    }

    public void UpdateLog()
    {
        ClearArray(logLines);

        int c = DebugLogging.debugLog.Count;
        int l = logLines.Length;
        int n;
        if (c < l)
        {
            n = c;
        }
        else
        {
            n = l;
        }

        for (int i = 1; i <= n; i++)
        {
            logLines[l - i] = DebugLogging.debugLog[c - i];
        }

        string readout = "";
        for (int i = 0; i < l; i++)
        {
            if (i > 0)
            {
                readout += "\n";
            }
            readout += logLines[i];
        }
        logReadout.text = readout;
    }

    public void LogShowState(bool show)
    {
        if (logConsoleState)
        {
            if (show)
            {
                Debug.Log("Showing dev console...");
            }
            else
            {
                Debug.Log("Hiding dev console...");
            }
        }
        else
        {
            logConsoleState = true;
        }
    }

    public bool ParseCommandInput()
    {
        bool parseSuccessful = false;
        string input = inputBox.text;

        if (char.Parse(input.Substring(0, 1)) == '/')
        {
            string[] args = input.Substring(1).Split(' ');
            
            switch (args[0])
            {
                default:
                    break;
            }
        }

        inputBox.text = "";
        return parseSuccessful;
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

}
