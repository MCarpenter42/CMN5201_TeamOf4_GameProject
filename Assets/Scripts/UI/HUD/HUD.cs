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

    [HideInInspector] public Dictionary<string, Prompt> prompts = new Dictionary<string, Prompt>();

    public Prompt interactPrompt;
	
	#endregion
    
	#region [ PROPERTIES ]
	
	
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        GetComponents();
        foreach (KeyValuePair<string, Prompt> prompt in prompts)
        {
            prompt.Value.gameObject.SetActive(false);
        }
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
	
    private void GetComponents()
    {
        prompts.Add("Interact", interactPrompt);
    }
}
