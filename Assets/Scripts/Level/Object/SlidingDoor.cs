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
    }

    void Start()
    {
        OnStart();
        posClosed = transform.position;
        posOpen = transform.position + gridOffsetWhenOpen * GameManager.LevelController.gridCellScale;
        if (startOpen)
        {
            transform.position = posOpen;
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
            Vector3 gridMovement = (targetPos - transform.position) / GameManager.LevelController.gridCellScale;
            Move(gridMovement, transitionTime, true);
        }

        isOpen = open;
    }
}
