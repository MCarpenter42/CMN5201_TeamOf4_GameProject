using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using TMPro;

public class ControlListItem : UIElement
{
    #region [ PROPERTIES ]

    [HideInInspector] public string targetInput;
    [HideInInspector] public int index;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {

    }

    void Start()
    {

    }

    void Update()
    {

    }

    void FixedUpdate()
    {

    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ SETUP ]

    public void ReceiveProperties(string targetInput, int index)
    {
        this.targetInput = targetInput;
        this.index = index;
    }

    public void SetPosition()
    {

    }

    #endregion

    #region [ INTERACTION ]

    public override void EventOnPointerEnter()
    {

    }

    public override void EventOnPointerExit()
    {

    }

    #endregion
}
