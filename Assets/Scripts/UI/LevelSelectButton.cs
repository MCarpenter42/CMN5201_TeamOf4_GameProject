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
public class LevelSelectButton : UIElement
{
    #region [ PROPERTIES ]

    private enum ColourPreset { Green, Yellow, Orange, Red };
    private Color presetGreen = new Color(0.3529412f, 0.772549f, 0.0f, 1.0f);
    private Color presetYellow = new Color(0.9056604f, 0.8990637f, 0.1999288f, 1.0f);
    private Color presetOrange = new Color(1.0f, 0.6383666f, 0.0f, 1.0f);
    private Color presetRed = new Color(1.0f, 0.248857f, 0.0f, 1.0f);

    [Header("Button")]
    [SerializeField] ColourPreset colour;
    [SerializeField] int level;
    [SerializeField] TMP_Text label;

    [Header("Text Colours")]
    [SerializeField] ColorBlock textColours;

    private Image img;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        OnAwake();
        UpdateVisuals();
        UpdateButtonProperties();
    }

    void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(ButtonLevelLoad);
        OnStart();
    }

#if UNITY_EDITOR
    void Update()
    {
        UpdateVisuals();
        UpdateButtonProperties();
    }
#endif

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public override void EventOnPointerEnter()
    {

    }

    public override void EventOnPointerExit()
    {

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

    private void UpdateButtonProperties()
    {
        Button btn = gameObject.GetComponent<Button>();
        if (gameObject.GetComponent<Button>() == null)
        {
            btn = gameObject.AddComponent<Button>();
        }

        btn.targetGraphic = label;

        btn.colors = textColours;
    }

    private void ButtonLevelLoad()
    {
        GameManager.Instance.LoadLevelAdjusted(level);
    }

}
