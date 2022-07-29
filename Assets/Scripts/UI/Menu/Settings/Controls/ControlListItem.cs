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

    [HideInInspector] public ControlListCategory category;

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

    /*public void ReceiveProperties(string targetInput, int index)
    {
        this.targetInput = targetInput;
        this.index = index;
    }*/

    public void SetName(string name)
    {
        itemName = name;
        controlName.text = name;
    }

    public void SetLabel(string label)
    {
        controlKey.text = label;
    }

    public void UpdateLabel()
    {
        KeyCode keyCode = Controls.GetControlByName(targetInput).Key;
        Controls.KeyNames.TryGetValue(keyCode, out string keyText);
        //Debug.Log(targetInput + ", " + keyCode + ", " + keyText);
        SetLabel(keyText);
    }

    public void TriggerSetControl()
    {
        category.parent.SetControl(this);
    }
}
