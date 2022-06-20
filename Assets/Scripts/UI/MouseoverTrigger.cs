using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

public class MouseoverTrigger : UI
{
    [Header("Mouse Pointer Enter/Exit Events")]
    public UnityEvent pointerEnterEvent;
    public UnityEvent pointerExitEvent;

    public override void EventOnPointerEnter()
    {
        pointerEnterEvent.Invoke();
    }

    public override void EventOnPointerExit()
    {
        pointerExitEvent.Invoke();
    }
}
