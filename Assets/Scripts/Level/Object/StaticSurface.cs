using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class StaticSurface : LevelObject
{
    #region [ PROPERTIES ]

    [Header("Surface Settings")]
    [SerializeField] bool isFloor = false;
    [SerializeField] FloorTypes material = FloorTypes.Stone;
	
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
	
}
