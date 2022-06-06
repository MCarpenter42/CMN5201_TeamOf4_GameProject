using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class TrueFalseIndicator : LevelObject
{
    #region [ PROPERTIES ]

    [SerializeField] GameObject indicatorPart;
    private MeshRenderer rndr;
    [SerializeField] Color falseColour = Color.red;
    [SerializeField] Color trueColour = Color.green;
    [SerializeField] bool startState = false;
    private bool state;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        rndr = indicatorPart.GetComponent<MeshRenderer>();
    }
    
    void Start()
    {
        //SetState(startState);
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void SetState(bool state)
    {
        this.state = state;
        if (state)
        {
            rndr.material.color = trueColour;
        }
        else
        {
            rndr.material.color = falseColour;
        }
    }
}
