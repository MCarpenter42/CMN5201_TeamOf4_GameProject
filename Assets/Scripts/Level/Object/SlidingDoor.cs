using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class SlidingDoor : LevelObject
{
    #region [ PROPERTIES ]

    [Header("Door Properties")]
    [SerializeField] LevelObject doorObject;
    [SerializeField] Vector3 gridOffsetWhenOpen;
    [SerializeField] bool startOpen;
    private Vector3 posClosed;
    private Vector3 posOpen;
    private bool isOpen = false;
    [SerializeField] float transitionTime = 4.0f;
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        OnAwake();
        if (doorObject == null)
        {
            Debug.Log("No door object designated");
            doorObject = this;
        }
    }

    void Start()
    {
        OnStart();
        posClosed = doorObject.transform.position;
        posOpen = doorObject.transform.position + gridOffsetWhenOpen * GameManager.LevelController.gridCellScale;
        if (startOpen)
        {
            doorObject.transform.position = posOpen;
            isOpen = true;
        }
    }
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    public void SetOpen(bool open)
    {
        if (isOpen != open)
        {
            Vector3 targetPos;
            if (open)
            {
                targetPos = posOpen;
            }
            else
            {
                targetPos = posClosed;
            }
            Vector3 gridMovement = (targetPos - doorObject.transform.position) / GameManager.LevelController.gridCellScale;
            doorObject.Move(gridMovement, transitionTime, true);
        }

        isOpen = open;
    }
}
