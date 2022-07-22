using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class VideoSettings : Core
{
    #region [ PROPERTIES ]

    public int targetFramerate = 60;
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        SetTargetFramerate(targetFramerate);
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
	
    public void SetTargetFramerate(int fRate)
    {
        if (fRate < 1)
        {
            targetFramerate = 0;
            Application.targetFrameRate = -1;
        }
        else
        {
            targetFramerate = fRate;
            Application.targetFrameRate = fRate;
        }
    }
}
