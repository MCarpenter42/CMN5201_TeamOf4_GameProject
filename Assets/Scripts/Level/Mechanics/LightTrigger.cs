using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEditor;
using TMPro;

public class LightTrigger : Core
{
    #region [ PROPERTIES ]

    [SerializeField] BeamColours triggerColour = BeamColours.White;
    private bool triggerState;

    [SerializeField] UnityEvent falseEvent;
    [SerializeField] UnityEvent trueEvent;
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Start()
    {
        ChangeTriggerState(false);
    }
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    public bool ColourCheck(BeamColours colour)
    {
        return (colour == triggerColour);
    }

    public void ChangeTriggerState(bool state)
    {
        if (triggerState != state)
        {
            if (state)
            {
                if (trueEvent != null)
                {
                    trueEvent.Invoke();
                }
            }
            else
            {
                if (falseEvent != null)
                {
                    falseEvent.Invoke();
                }
            }
            triggerState = state;
        }
    }
}
