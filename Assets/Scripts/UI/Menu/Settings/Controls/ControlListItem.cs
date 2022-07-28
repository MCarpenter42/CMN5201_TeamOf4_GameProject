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

    [Header("Components")]
    [SerializeField] TMP_Text controlName;
    [SerializeField] TMP_Text controlKey;

    [HideInInspector] public string itemName;
    [HideInInspector] public string targetInput;
    [HideInInspector] public int index;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ SETUP ]

    public void ReceiveProperties(string targetInput, int index)
    {
        this.targetInput = targetInput;
        this.index = index;
    }

    public void SetName(string name)
    {
        itemName = name;
        controlName.text = name;
    }

    #endregion
}
