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
public class DropdownButton : AudioButton, IPointerExitHandler
{
    #region [ PROPERTIES ]

    [Header("Menu")]
    public DropdownMenu attachedMenu;

    [Header("")]
    [SerializeField] UnityEvent_int eventOnSelectionMade;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    protected override void Awake()
    {
        base.Awake();
        attachedMenu.mainButton = this;
    }

    protected override void Start()
    {
        base.Start();
        attachedMenu.mainButton = this;
        if (Application.isPlaying)
        {
            attachedMenu.SetShown(false);
            UpdateSelection(attachedMenu.selOption);
        }
    }

#if UNITY_EDITOR
    protected override void Update()
    {
        SetLabel(attachedMenu.menuOptions[attachedMenu.selOption]);
    }
#endif

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (Application.isPlaying)
        {
            StartCoroutine(IDelayedShowMenu(false));
        }
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    protected override void AddListeners()
    {
        base.AddListeners();
        button.onClick.AddListener(ToggleMenu);
    }

    public void ToggleMenu()
    {
        attachedMenu.SetShown(!attachedMenu.visible);
    }

    public void UpdateSelection(int selectedInt)
    {
        SetLabel(attachedMenu.menuOptions[selectedInt]);
        if (Application.isPlaying)
        {
            eventOnSelectionMade.Invoke(selectedInt);
            attachedMenu.SetShown(false);
        }
    }

    private IEnumerator IDelayedShowMenu(bool show)
    {
        yield return null;
        if (attachedMenu.visible && !attachedMenu.mouseOver)
        {
            attachedMenu.SetShown(show);
        }
    }

    private IEnumerator IDelayedShowMenu(bool show, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (attachedMenu.visible && !attachedMenu.mouseOver)
        {
            attachedMenu.SetShown(show);
        }
    }
}
