using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

[ExecuteInEditMode]
public class AudioButton : UIElement
{
    public enum ButtonType { Standard, Heavy, Slider };

    protected Button button;
    [Header("Button Properties")]
    [SerializeField] Graphic targetGraphic;
    [SerializeField] ColorBlock colours = new ColorBlock
    {
        colorMultiplier = 1.0f,
        fadeDuration = 0.1f
    };

    [Header("Audio Properties")]
    [SerializeField] ButtonType type;

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    protected override void Awake()
    {
        base.Awake();
        ButtonComponent();
    }

    protected override void Start()
    {
        base.Start();
        AddListeners();
    }

#if UNITY_EDITOR
    protected override void Update()
    {
        ButtonComponent();
        ApplyButtonProperties();
    }
#endif

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    protected void ButtonComponent()
    {
        button = gameObject.GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }
        ApplyButtonProperties();
    }

    protected void ApplyButtonProperties()
    {
        if (targetGraphic == null)
        {
            targetGraphic = gameObject.GetComponent<Graphic>();
        }
        button.targetGraphic = targetGraphic;
        button.colors = colours;
    }

    protected virtual void AddListeners()
    {
        button.onClick.AddListener(ClickSound);
    }

    public void ClickSound()
    {
        GameManager.AudioController.ButtonClick(type);
    }
}
