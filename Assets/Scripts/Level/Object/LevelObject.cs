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

    [HideInInspector] public GameObject pivot;

    public bool movable = false;
    public bool rotatable = false;

    protected Coroutine movement;

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
        GetComponents();
    }
    
    protected void OnStart()
    {
        GetFacing();
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    protected void GetComponents()
    {
        if (GetChildrenWithTag(gameObject, "Pivot").Count > 0)
        {
            pivot = GetChildrenWithTag(gameObject, "Pivot")[0];
        }
    }

    public Vector3 GetGridPos()
    {
        gridPos = transform.position / GameManager.LevelController.gridCellScale;
        return gridPos;
    }

    public float GetFacing()
    {
        if (pivot != null)
        {
            facingDir = WrapClamp(pivot.transform.eulerAngles.y, 0.0f, 360.0f);
        }
        return facingDir;
    }

    public float GetGridBearing(Vector3 dir)
    {
        if (dir.x >= 0.0f)
        {
            return ToDeg(Mathf.Acos(dir.z / dir.magnitude));
        }
        else
        {
            return 360.0f - ToDeg(Mathf.Acos(dir.z / dir.magnitude));
        }
    }
    
    public Vector3 GetGridDir(float bearing)
    {
        Vector3 dir = new Vector3();
        dir.x = Mathf.Sin(ToRad(bearing));
        dir.z = Mathf.Cos(ToRad(bearing));
        return dir;
    }
    
    public void Move(Vector3 gridMovement)
    {
        if (!isMoving)
        {
            movement = StartCoroutine(MoveAnim(gridMovement, 1.0f));
        }
    }
    
    public void Move(Vector3 gridMovement, bool interrupt)
    {
        if (!isMoving)
        {
            movement = StartCoroutine(MoveAnim(gridMovement, 1.0f));
        }
        else if (interrupt)
        {
            if (movement != null)
            {
                StopCoroutine(movement);
                movement = StartCoroutine(MoveAnim(gridMovement, 1.0f));
            }
        }
    }

    public void Move(Vector3 gridMovement, float animTime)
    {
        if (!isMoving)
        {
            movement = StartCoroutine(MoveAnim(gridMovement, animTime));
        }
    }
    
    public void Move(Vector3 gridMovement, float animTime, bool interrupt)
    {
        if (!isMoving)
        {
            movement = StartCoroutine(MoveAnim(gridMovement, animTime));
        }
        else if (interrupt)
        {
            if (movement != null)
            {
                StopCoroutine(movement);
                movement = StartCoroutine(MoveAnim(gridMovement, animTime));
            }
        }
    }
    
    protected IEnumerator MoveAnim(Vector3 gridMovement, float animTime)
    {
        isMoving = true;

        Vector3 posStart = transform.position;
        Vector3 posEnd = posStart + gridMovement * GameManager.LevelController.gridCellScale;

        float timeElapsed = 0.0f;
        while (timeElapsed <= animTime)
        {
            timeElapsed += Time.deltaTime;
            float delta = timeElapsed / animTime;

            yield return null;

            transform.position = Vector3.Lerp(posStart, posEnd, delta);
        }

        GetGridPos();

        isMoving = false;
    }
    
    public void Move(Vector3 gridMovement, Vector3 jumpOffset)
    {
        if (!isMoving)
        {
            movement = StartCoroutine(MoveAnim(gridMovement, jumpOffset, 1.0f));
        }
    }
    
    public void Move(Vector3 gridMovement, Vector3 jumpOffset, bool interrupt)
    {
        if (!isMoving)
        {
            movement = StartCoroutine(MoveAnim(gridMovement, jumpOffset, 1.0f));
        }
        else if (interrupt)
        {
            if (movement != null)
            {
                StopCoroutine(movement);
                movement = StartCoroutine(MoveAnim(gridMovement, jumpOffset, 1.0f));
            }
        }
    }

    public void Move(Vector3 gridMovement, Vector3 jumpOffset, float animTime)
    {
        if (!isMoving)
        {
            movement = StartCoroutine(MoveAnim(gridMovement, jumpOffset, animTime));
        }
    }
    
    public void Move(Vector3 gridMovement, Vector3 jumpOffset, float animTime, bool interrupt)
    {
        if (!isMoving)
        {
            movement = StartCoroutine(MoveAnim(gridMovement, jumpOffset, animTime));
        }
        else if (interrupt)
        {
            if (movement != null)
            {
                StopCoroutine(movement);
                movement = StartCoroutine(MoveAnim(gridMovement, jumpOffset, animTime));
            }
        }
    }

    protected IEnumerator MoveAnim(Vector3 gridMovement, Vector3 jumpOffset, float animTime)
    {
        isMoving = true;

        Vector3 posStart = transform.position;
        Vector3 posEnd = posStart + gridMovement * GameManager.LevelController.gridCellScale;

        float timeElapsed = 0.0f;
        while (timeElapsed <= animTime)
        {
            timeElapsed += Time.deltaTime;
            float delta = timeElapsed / animTime;

            yield return null;

            Vector3 pos = Vector3.Lerp(posStart, posEnd, delta);
            pos += jumpOffset * Mathf.Sin(Mathf.PI * delta);
            transform.position = pos;
        }

        transform.position = posEnd;
        GetGridPos();

        isMoving = false;
    }

    public void Rotate(float gridRot, float animTime)
    {
        if (!isMoving)
        {
            movement = StartCoroutine(RotateAnim(gridRot, animTime));
        }
    }
    
    public void Rotate(float gridRot, float animTime, bool interrupt)
    {
        if (!isMoving)
        {
            movement = StartCoroutine(RotateAnim(gridRot, animTime));
        }
        else if (interrupt)
        {
            if (movement != null)
            {
                StopCoroutine(movement);
                movement = StartCoroutine(RotateAnim(gridRot, animTime));
            }
        }
    }

    protected IEnumerator RotateAnim(float gridRot, float animTime)
    {
        isMoving = true;

        float rotAngle = gridRot * 90.0f;
        Vector3 rotStart = pivot.transform.eulerAngles;
        Vector3 rotEnd = rotStart;
        rotEnd.y += rotAngle;

        float facingStart = facingDir;
        float facingEnd = WrapClamp(facingStart + rotAngle, 0.0f, 360.0f);

        float timeElapsed = 0.0f;
        while (timeElapsed <= animTime)
        {
            timeElapsed += Time.deltaTime;
            float delta = Time.deltaTime / animTime;
            float rotDelta = rotAngle * delta;

            yield return null;

            if (pivot != null)
            {
                pivot.transform.Rotate(Vector3.up, rotDelta);
            }
            facingDir += rotDelta;
        }
        pivot.transform.eulerAngles = rotEnd;
        facingDir = facingEnd;

        isMoving = false;
    }
    
    public void RotateAround(Vector3 gridPivot, float gridRot, float animTime)
    {
        if (!isMoving)
        {
            movement = StartCoroutine(RotateAroundAnim(gridPivot, gridRot, animTime));
        }
    }
    
    public void RotateAround(Vector3 gridPivot, float gridRot, float animTime, bool interrupt)
    {
        if (!isMoving)
        {
            movement = StartCoroutine(RotateAroundAnim(gridPivot, gridRot, animTime));
        }
        else if (interrupt)
        {
            if (movement != null)
            {
                StopCoroutine(movement);
                movement = StartCoroutine(RotateAroundAnim(gridPivot, gridRot, animTime));
            }
        }
    }

    protected IEnumerator RotateAroundAnim(Vector3 gridPivot, float gridRot, float animTime)
    {
        isMoving = true;
        
        float rotAngle = gridRot * 90.0f;

        Vector3 pivotPoint = gridPivot * GameManager.LevelController.gridCellScale;
        float rotRadius = Vector3.Distance(transform.position, pivotPoint);

        float startAngle = GetGridBearing(transform.position - gridPivot);
        float endAngle = startAngle + rotAngle;

        Vector3 posStart = transform.position;
        Vector3 posEnd = new Vector3();
        posEnd.x = pivotPoint.x + rotRadius * Mathf.Sin(ToRad(endAngle));
        posEnd.y = posStart.y;
        posEnd.z = pivotPoint.z + rotRadius * Mathf.Cos(ToRad(endAngle));

        Vector3 rotStart = pivot.transform.eulerAngles;
        Vector3 rotEnd = rotStart;
        rotEnd.y += rotAngle;

        float facingStart = facingDir;
        float facingEnd = WrapClamp(facingStart + rotAngle, 0.0f, 360.0f);

        // pos x = pvt x + r * sin(angle)
        // pos z = pvt z + r * cos(angle)

        float timeElapsed = 0.0f;
        while (timeElapsed <= animTime)
        {
            timeElapsed += Time.deltaTime;
            float delta = timeElapsed / animTime;

            float angle = Mathf.Lerp(startAngle, endAngle, delta);
            float angleRad = ToRad(angle);
            float x = pivotPoint.x + rotRadius * Mathf.Sin(angleRad);
            float z = pivotPoint.z + rotRadius * Mathf.Cos(angleRad);

            float rotDelta = rotAngle * Time.deltaTime / animTime;

            yield return null;

            transform.position = new Vector3(x, transform.position.y, z);

            if (pivot != null)
            {
                pivot.transform.Rotate(Vector3.up, rotDelta);
            }
            facingDir += rotDelta;
        }
        transform.position = posEnd;
        pivot.transform.eulerAngles = rotEnd;
        facingDir = facingEnd;

        GetGridPos();
        isMoving = false;
    }

}
