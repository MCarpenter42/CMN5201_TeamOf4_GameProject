using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class UIController : UI
{
    #region [ OBJECTS ]

    public HUD hud;
    public PauseMenu pauseMenu;
	
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
        if (GameManager.LevelController.isGameplayLevel)
        {
            if (hud == null)
            {
                hud = GetChildrenWithComponent<HUD>(gameObject)[0].GetComponent<HUD>();
            }
            if (pauseMenu == null)
            {
                pauseMenu = GetChildrenWithComponent<PauseMenu>(gameObject)[0].GetComponent<PauseMenu>();
            }
            pauseMenu.menuFrame.SetActive(false);
        }
    }
}
