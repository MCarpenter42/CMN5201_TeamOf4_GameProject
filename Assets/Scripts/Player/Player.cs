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
    private Vector3 checkOffset = new Vector3(0.0f, -0.9f, 0.0f);

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
            bool obstructed = Physics.Raycast(transform.position + checkOffset, dir, GameManager.LevelController.gridCellScale * 1.45f);
            if (!obstructed)
            {
                Vector3 targetPos = transform.position + checkOffset + dir * GameManager.LevelController.gridCellScale;
                bool walkable = Physics.Raycast(targetPos, Vector3.down, GameManager.LevelController.gridCellScale * 0.20f);
                if (walkable)
                {
                    Move(gridMovement, animTime);
                    StartCoroutine(DelayedCheckObjectPushable(gridMovement, animTime));
                }
            }
        }
        else
        {
            GetObjectMovingDir();
            if (gridMovement.normalized == -objectDir.normalized)
            {
                bool obstructed = Physics.Raycast(transform.position + checkOffset, dir, GameManager.LevelController.gridCellScale * 1.45f);
                if (!obstructed)
                {
                    Vector3 targetPos = transform.position + checkOffset + dir * GameManager.LevelController.gridCellScale;
                    bool walkable = Physics.Raycast(targetPos, Vector3.down, GameManager.LevelController.gridCellScale * 0.20f);
                    if (walkable)
                    {
                        Move(gridMovement, animTime);
                        objectMoving.Move(gridMovement, animTime);
                        StartCoroutine(DelayedCheckObjectPushable(gridMovement, animTime));
                    }
                }
            }
            else if (gridMovement.normalized == objectDir.normalized)
            {
                bool obstructed = Physics.Raycast(objectMoving.transform.position + checkOffset, dir, GameManager.LevelController.gridCellScale * 1.45f);
                if (!obstructed)
                {
                    Vector3 targetPos = objectMoving.transform.position + checkOffset + dir * GameManager.LevelController.gridCellScale;
                    bool walkable = Physics.Raycast(targetPos, Vector3.down, GameManager.LevelController.gridCellScale * 0.20f);
                    if (walkable)
                    {
                        Move(gridMovement, animTime);
                        objectMoving.Move(gridMovement, animTime);
                        StartCoroutine(DelayedCheckObjectPushable(gridMovement, animTime));
                    }
                }
            }
        }
        ChangeFacing(gridMovement);
    }

    public void ChangeFacing(Vector3 dir)
    {
        facingDir = GetGridBearing(dir);

        if (dirInd == null)
        {
            Debug.Log(facingDir);
        }
        else
        {
            pivot.transform.eulerAngles = new Vector3(0.0f, facingDir, 0.0f);
        }

        CheckObjectPushable(dir);
    }

    public void CheckObjectPushable(Vector3 dir)
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + checkOffset, dir, out hit, GameManager.LevelController.gridCellScale * 1.10f);
        if (hit.collider != null)
        {
            LevelObject obj = hit.collider.gameObject.GetComponent<LevelObject>();
            if (obj != null && obj.movable)
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

        Prompt interactPrompt;
        GameManager.UIController.hud.prompts.TryGetValue("Interact", out interactPrompt);

        if (canGrabFacing && objectMoving == null)
        {
            interactPrompt.gameObject.SetActive(true);
        }
        else
        {
            interactPrompt.gameObject.SetActive(false);
        }
    }

    public IEnumerator DelayedCheckObjectPushable(Vector3 dir, float time)
    {
        yield return new WaitForSecondsRealtime(time);
        CheckObjectPushable(dir);
    }

    public bool Grab()
    {
        bool grabSuccess = false;
        Vector3 grabDir = new Vector3(Mathf.Sin(ToRad(facingDir)), 0.0f, Mathf.Cos(ToRad(facingDir)));
        RaycastHit hit;
        Physics.Raycast(transform.position + checkOffset, grabDir, out hit, GameManager.LevelController.gridCellScale * 1.0f);
        GameObject hitObject = hit.collider.gameObject;
        if (hitObject.GetComponent<LevelObject>() != null)
        {
            grabSuccess = true;
            objectMoving = hitObject.GetComponent<LevelObject>();
            objectDir = grabDir;
            CheckObjectPushable(grabDir);
        }
        return grabSuccess;
    }

    public void Release()
    {
        objectMoving = null;
        objectDir = Vector3.zero;
    }

    public Vector3 GetObjectMovingDir()
    {
        float x = objectMoving.transform.position.x - transform.position.x;
        float z = objectMoving.transform.position.z - transform.position.z;
        //Debug.Log(x + ", " + z);
        Vector3 dirObjectMoving = new Vector3(x, 0.0f, z);
        float angleObjectMoving = GetGridBearing(dirObjectMoving);
        objectDir = new Vector3(Mathf.Sin(ToRad(angleObjectMoving)), 0.0f, Mathf.Cos(ToRad(angleObjectMoving)));
        return objectDir;
    }
}
