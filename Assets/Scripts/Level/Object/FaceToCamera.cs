using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class FaceToCamera : Core
{
    #region [ PROPERTIES ]

    private GameObject camPos;
    [SerializeField] bool syncX = true;
    [SerializeField] bool syncY = true;
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Start()
    {
        camPos = FindObjectOfType<CameraController>().cameraZoom;
    }
	
    void Update()
    {
        Vector3 rotPrev = transform.localEulerAngles;
        transform.LookAt(camPos.transform.position);
        Vector3 rot = transform.localEulerAngles;
        if (!syncX)
        {
            rot.x = rotPrev.x;
        }
        if (!syncY)
        {
            rot.y = rotPrev.y;
        }
        rot.z = rotPrev.z;
        transform.eulerAngles = rot;
    }

	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
}
