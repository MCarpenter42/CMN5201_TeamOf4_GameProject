using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using TMPro;

public class IntButton : AudioButton
{
    #region [ PROPERTIES ]

    [Header("")]
    public int invokeWith;
    public UnityEvent_int intEvent = new UnityEvent_int();

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    protected override void AddListeners()
    {
        base.AddListeners();
        button.onClick.AddListener(InvokeEvent);
    }

    public void InvokeEvent()
    {
        intEvent.Invoke(invokeWith);
    }
}
