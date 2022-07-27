using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

public class MouseoverTrigger : UIElement, IPointerEnterHandler, IPointerExitHandler
{
    public bool listenForMouseover = true;

    [Header("Mouse Pointer Enter/Exit Events")]
    public UnityEvent pointerEnterEvent;
    public UnityEvent pointerExitEvent;

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        if (listenForMouseover)
        {
            pointerEnterEvent.Invoke();
        }
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        if (listenForMouseover)
        {
            pointerExitEvent.Invoke();
        }
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void ListenForMouseover(bool listen)
    {
        this.listenForMouseover = listen;
    }
}
