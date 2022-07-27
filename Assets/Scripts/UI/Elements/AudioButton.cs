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

    protected Button button { get { return GetOrAddComponent<Button>(gameObject); } }
    [Header("Button Properties")]
    [SerializeField] Graphic targetGraphic;
    [SerializeField] ColorBlock colours = new ColorBlock
    {
        normalColor = Color.white,
        highlightedColor = new Color(0.9607843f, 0.9607843f, 0.9607843f, 1.0000000f),
        pressedColor = new Color(0.7843137f, 0.7843137f, 0.7843137f, 1.0000000f),
        selectedColor = new Color(0.9607843f, 0.9607843f, 0.9607843f, 1.0000000f),
        disabledColor = new Color(0.7843137f, 0.7843137f, 0.7843137f, 0.5019608f),
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
        ApplyButtonProperties();
    }

    protected override void Start()
    {
        base.Start();
        if (Application.isPlaying)
        {
            AddListeners();
        }
    }

#if UNITY_EDITOR
    protected override void Update()
    {
        ApplyButtonProperties();
    }
#endif

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public bool SetLabel(string text)
    {
        List<TMP_Text> textChildren = GetComponentsInChildren<TMP_Text>(button.gameObject);
        if (textChildren.Count > 0)
        {
            textChildren[0].text = text;
        }

        return (textChildren.Count > 0);
    }

    protected void ButtonComponent()
    {
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
