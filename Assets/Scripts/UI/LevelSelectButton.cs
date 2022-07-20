using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

[ExecuteInEditMode]
public class LevelSelectButton : AudioButton
{
    #region [ PROPERTIES ]

    private enum ColourPreset { Green, Yellow, Orange, Red };
    private Color presetGreen = new Color(0.3529412f, 0.772549f, 0.0f, 1.0f);
    private Color presetYellow = new Color(0.9056604f, 0.8990637f, 0.1999288f, 1.0f);
    private Color presetOrange = new Color(1.0f, 0.6383666f, 0.0f, 1.0f);
    private Color presetRed = new Color(1.0f, 0.248857f, 0.0f, 1.0f);

    [Header("Level Button Properties")]
    [SerializeField] ColourPreset colour;
    [SerializeField] int level;
    [SerializeField] TMP_Text label;

    private Image img;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    protected override void Awake()
    {
        base.Awake();
        UpdateVisuals();
    }

    protected override void Start()
    {
        base.Start();
    }

#if UNITY_EDITOR
    protected override void Update()
    {
        base.Update();
        UpdateVisuals();
    }
#endif

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    protected override void AddListeners()
    {
        base.AddListeners();
        button.onClick.AddListener(ButtonLevelLoad);
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private void UpdateVisuals()
    {
        if (level == 0)
        {
            label.text = "00";
        }
        else
        {
            label.text = level.ToString();
        }

        if (img == null)
        {
            img = GetComponent<Image>();
        }

        switch (colour)
        {
            default:
            case ColourPreset.Green:
                img.color = presetGreen;
                break;

            case ColourPreset.Yellow:
                img.color = presetYellow;
                break;

            case ColourPreset.Orange:
                img.color = presetOrange;
                break;

            case ColourPreset.Red:
                img.color = presetRed;
                break;
        }
    }

    private void ButtonLevelLoad()
    {
        GameManager.Instance.LoadLevelAdjusted(level);
    }

}
