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
        button.enabled = CheckUnlockState();
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

    private bool CheckUnlockState()
    {
        int index = level - 1;
        return GameManager.levelsUnlocked[index];
    }

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

        Color clr;
        switch (colour)
        {
            default:
            case ColourPreset.Green:
                clr = presetGreen;
                break;

            case ColourPreset.Yellow:
                clr = presetYellow;
                break;

            case ColourPreset.Orange:
                clr = presetOrange;
                break;

            case ColourPreset.Red:
                clr = presetRed;
                break;
        }

        if (button.enabled)
        {
            img.color = clr;
            button.targetGraphic.color = Color.white;
        }
        else
        {
            img.color = GetDisableColour(clr);
            button.targetGraphic.color = GetDisableColour(Color.white);
        }
    }

    private void ButtonLevelLoad()
    {
        //GameManager.Instance.LoadUnlockedLevel(level - 1);
        GameManager.Instance.LoadLevel(level - 1);
        Debug.Log(level);
    }

    private Color GetDisableColour(Color clr)
    {
        Color disClr = Color.white;
        disClr.r = clr.r * 0.5f;
        disClr.g = clr.g * 0.5f;
        disClr.b = clr.b * 0.5f;
        return disClr;
    }

}
