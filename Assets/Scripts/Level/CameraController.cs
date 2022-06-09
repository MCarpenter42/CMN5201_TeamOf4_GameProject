using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class CameraController : Core
{
    #region [ PROPERTIES ]

    private GameObject cameraMount;
    [SerializeField] bool useLevelGridScale = false;
    [SerializeField] List<Vector3> cameraPoints = new List<Vector3>() { Vector3.zero };
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        cameraMount = transform.GetChild(0).gameObject;
    }

    void Start()
    {
        if (useLevelGridScale)
        {
            cameraMount.transform.position = cameraPoints[0] * GameManager.LevelController.gridCellScale;
        }
        else
        {
            cameraMount.transform.position = cameraPoints[0];
        }
    }
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
}
