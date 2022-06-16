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
    public Prompt promptInteract;
    public Prompt promptRotCW;
    public Prompt promptRotCCW;

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
            prompt.Show(false);
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
            promptInteract,
            promptRotCW,
            promptRotCCW,
        };
    }

    private void SetKeyPromptLabels()
    {
        promptInteract.SetText(0, Controls.Interaction.Interact.Key.ToString(), AdjustCondition.GreaterThan);
        promptRotCW.SetText(0, Controls.Interaction.RotateClockwise.Key.ToString(), AdjustCondition.GreaterThan);
        promptRotCCW.SetText(0, Controls.Interaction.RotateCounterClockwise.Key.ToString(), AdjustCondition.GreaterThan);
    }
}
