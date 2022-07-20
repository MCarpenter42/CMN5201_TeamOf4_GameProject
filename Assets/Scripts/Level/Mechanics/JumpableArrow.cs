using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class JumpableArrow : Core
{
    #region [ PROPERTIES ]

    [SerializeField] GameObject arrow;
    [SerializeField] Axis followAxis;
    [SerializeField] float minPosition;
    [SerializeField] float maxPosition;

    private Vector3[] bounds;
    private bool arrowActive = true;
    private Vector3 arrowPos;
    private Vector3 playerPos;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        SetBounds();
        arrowPos = arrow.transform.position;
    }

    void Start()
    {
        PlayerPosition();
        ArrowPosition();
    }
	
    void Update()
    {
        if (arrowActive)
        {
            PlayerPosition();
            ArrowPosition();
        }
        else if (arrow.activeSelf)
        {
            arrow.SetActive(false);
        }
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private void SetBounds()
    {
        Vector3 boundsEdgeOffset = (transform.localScale - Vector3.one) / 2.0f;
        bounds = new Vector3[2];
        bounds[0] = transform.position - boundsEdgeOffset;
        bounds[1] = transform.position + boundsEdgeOffset;
    }

    private void PlayerPosition()
    {
        playerPos = GameManager.Player.transform.position;
        bool inBoundsX = (playerPos.x >= bounds[0].x && playerPos.x <= bounds[1].x);
        bool inBoundsY = (playerPos.y >= bounds[0].y && playerPos.y <= bounds[1].y);
        bool inBoundsZ = (playerPos.z >= bounds[0].z && playerPos.z <= bounds[1].z);
        if (inBoundsX && inBoundsY && inBoundsZ)
        {
            arrowActive = false;
        }
    }

    private void ArrowPosition()
    {
        float trackedPos;
        switch (followAxis)
        {
            default:
            case Axis.X:
                trackedPos = playerPos.x;
                break;

            case Axis.Y:
                trackedPos = playerPos.y;
                break;

            case Axis.Z:
                trackedPos = playerPos.z;
                break;
        }

        if (trackedPos >= minPosition && trackedPos <= maxPosition)
        {
            arrow.SetActive(true);
            switch (followAxis)
            {
                default:
                case Axis.X:
                    arrowPos.x = trackedPos;
                    break;

                case Axis.Y:
                    arrowPos.y = trackedPos;
                    break;

                case Axis.Z:
                    arrowPos.z = trackedPos;
                    break;
            }
            arrow.transform.position = arrowPos;
        }
        else
        {
            arrow.SetActive(false);
        }
    }
}
