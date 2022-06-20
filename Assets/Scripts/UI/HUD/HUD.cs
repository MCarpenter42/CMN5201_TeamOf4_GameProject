using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class HUD : UI
{
    #region [ OBJECTS ]

    [HideInInspector] public List<Prompt> prompts;

    [Header("Prompts")]
    public Prompt levelHint;
    public Prompt keyInteract;
    public Prompt keyRotCW;
    public Prompt keyRotCCW;

	#endregion
    
	#region [ PROPERTIES ]
	
	
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        GetComponents();
    }

    void Start()
    {
        SetKeyPromptLabels();
        foreach (Prompt prompt in prompts)
        {
            prompt.SetShown(false);
        }
    }
	
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    private void GetComponents()
    {
        prompts = new List<Prompt>()
        {
            levelHint,
            keyInteract,
            keyRotCW,
            keyRotCCW
        };
    }

    private void SetKeyPromptLabels()
    {
        keyInteract.SetText(0, Controls.Interaction.Interact.Key.ToString(), AdjustCondition.GreaterThan, AdjustCondition.Never);
        keyRotCW.SetText(0, Controls.Interaction.RotateClockwise.Key.ToString(), AdjustCondition.GreaterThan, AdjustCondition.Never);
        keyRotCCW.SetText(0, Controls.Interaction.RotateCounterClockwise.Key.ToString(), AdjustCondition.GreaterThan, AdjustCondition.Never);
    }
}
