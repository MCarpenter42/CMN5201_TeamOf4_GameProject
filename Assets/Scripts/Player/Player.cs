using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class Player : LevelObject
{
    #region [ PROPERTIES ]

    [SerializeField] GameObject dirInd;
    [HideInInspector] public bool canGrabFacing = false;
    [HideInInspector] public LevelObject objectMoving = null;
    [HideInInspector] public Vector3 objectDir = Vector3.zero;

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
    }
	
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    public Vector3 GetStartingMove()
    {
        Vector3 startingMove = Vector3.zero;
        if (gridPos.x < 0 || gridPos.x >= GameManager.LevelController.worldGrid.gridSize[0])
        {
            startingMove.x -= gridPos.x;
        }
        if (gridPos.z < 0 || gridPos.z >= GameManager.LevelController.worldGrid.gridSize[1])
        {
            startingMove.z -= gridPos.z;
        }
        return startingMove;
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void PlayerMove(Vector3 gridMovement, float animTime)
    {
        Vector3 dir = gridMovement.normalized;
        if (objectMoving == null)
        {
            bool obstructed = Physics.Raycast(transform.position, dir, GameManager.LevelController.gridCellScale * 1.45f);
            if (!obstructed)
            {
                Vector3 targetPos = transform.position + dir * GameManager.LevelController.gridCellScale;
                bool walkable = Physics.Raycast(targetPos, Vector3.down, GameManager.LevelController.gridCellScale * 1.10f);
                if (walkable)
                {
                    Move(gridMovement, animTime);
                    StartCoroutine(DelayedCheckObjectPushable(gridMovement, animTime));
                }
            }
        }
        else
        {
            if (gridMovement.normalized == -objectDir)
            {

            }
            else if (gridMovement.normalized == objectDir)
            {

            }
        }
        ChangeFacing(gridMovement);
    }

    public void ChangeFacing(Vector3 dir)
    {
        float angleFromUp = 0.0f;
        if (dir.x >= 0.0f)
        {
            angleFromUp = ToDeg(Mathf.Acos(dir.z / dir.magnitude));
        }
        else
        {
            angleFromUp = 360.0f - ToDeg(Mathf.Acos(dir.z / dir.magnitude));
        }

        if (dirInd == null)
        {
            Debug.Log(angleFromUp);
        }
        else
        {
            dirInd.transform.eulerAngles = new Vector3(0.0f, angleFromUp, 0.0f);
        }

        CheckObjectPushable(dir);
    }

    public void CheckObjectPushable(Vector3 dir)
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, dir, out hit, GameManager.LevelController.gridCellScale * 1.10f);
        if (hit.collider != null)
        {
            LevelObject obj = hit.collider.gameObject.GetComponent<LevelObject>();
            if (obj != null && obj.type == ObjectTypes.Movable)
            {
                canGrabFacing = true;
            }
            else
            {
                canGrabFacing = false;
            }
        }
        else
        {
            canGrabFacing = false;
        }

        Debug.Log(canGrabFacing);
    }

    public IEnumerator DelayedCheckObjectPushable(Vector3 dir, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        CheckObjectPushable(dir);
    }
}
