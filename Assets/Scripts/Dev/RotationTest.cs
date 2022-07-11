using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class RotationTest : LevelObject
{
    #region [ PROPERTIES ]

    [SerializeField] Vector3 pivotGridOffset = Vector3.zero;
    private Vector3 pivotPoint;
    [SerializeField] float rotTime = 1.0f;
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        OnAwake();
    }

    void Start()
    {
        OnStart();
        pivotPoint = transform.position + pivotGridOffset * GameManager.LevelController.gridCellScale;
        StartCoroutine(RotAroundTest());
    }
	
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    private IEnumerator RotAroundTest()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(rotTime + 2.0f);
            RotateAround(pivotPoint, 1.0f, rotTime);
        }
    }
}
