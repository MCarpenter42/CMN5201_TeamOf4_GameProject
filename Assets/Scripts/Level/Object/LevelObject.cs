using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class LevelObject : Core
{
    #region [ PROPERTIES ]

    [HideInInspector] public Vector3 gridPos = new Vector3();
    [HideInInspector] public bool isMoving = false;
    [HideInInspector] public float facingDir = 0.0f;

    public ObjectTypes type;

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

    protected void OnAwake()
    {
        
    }
    
    protected void OnStart()
    {

    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public Vector3 GetGridPos()
    {
        Vector3 pos = transform.position;
        Vector3 gridPos = Vector3.zero;
        gridPos.x = pos.x - GameManager.LevelController.worldGrid.gridOffset.x;
        gridPos.z = pos.z - GameManager.LevelController.worldGrid.gridOffset.z;
        gridPos.x /= GameManager.LevelController.gridCellScale;
        gridPos.z /= GameManager.LevelController.gridCellScale;
        this.gridPos = gridPos;
        //Debug.Log(transform.position + ", " + gridPos);
        return gridPos;
    }

    public void Move(Vector3 gridMovement)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveAnim(gridMovement, 1.0f));
        }
    }

    public void Move(Vector3 gridMovement, float animTime)
    {
        if (!isMoving)
        {
            StartCoroutine(MoveAnim(gridMovement, animTime));
        }
    }

    private IEnumerator MoveAnim(Vector3 gridMovement, float animTime)
    {
        isMoving = true;

        int animFrames = (int)(animTime * 200.0f);
        float animFrameTime = animTime / (float)animFrames;
        Vector3 posStart = transform.position;
        Vector3 posEnd = posStart + gridMovement * GameManager.LevelController.gridCellScale;

        for (int i = 1; i <= animFrames; i++)
        {
            float delta = (float)i / (float)animFrames;

            yield return new WaitForSecondsRealtime(animFrameTime);

            transform.position = Vector3.Lerp(posStart, posEnd, delta);
        }

        GetGridPos();

        isMoving = false;
    }

}
