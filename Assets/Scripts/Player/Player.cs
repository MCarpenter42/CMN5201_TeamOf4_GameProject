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

    [Header("Components")]
    [SerializeField] GameObject dirInd;
    [HideInInspector] public bool canGrabFacing = false;
    [HideInInspector] public LevelObject objectMoving = null;
    [HideInInspector] public Vector3 objectDir = Vector3.zero;
    private Vector3 checkOffset = new Vector3(0.0f, -0.9f, 0.0f);

    [Header("Attributes")]
    [SerializeField] float jumpHeight = 0.5f;
    [SerializeField] float jumpTimescale = 1.2f;
    public float moveTime = 0.25f;

    [Header("Behaviour")]
    [SerializeField] public bool useStartPath = false;
    [SerializeField] public List<MovePathPoint> startPathPoints = new List<MovePathPoint>();

    private float moveTimeElapsed = 0.0f;

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
        Debugging();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<EndPoint>() != null)
        {
            GoToScene('+');
        }
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void FollowStartPath()
    {
        if (startPathPoints.Count > 0)
        {
            transform.localPosition = startPathPoints[0].transform.localPosition;
            GetGridPos();
            if (startPathPoints.Count > 1)
            {
                StartCoroutine(MovePathAnim(startPathPoints));
            }    
        }
    }

    private IEnumerator MovePathAnim(List<MovePathPoint> path)
    {
        for (int i = 1; i < path.Count; i++)
        {
            Vector3 gridMovement = path[i].transform.position - path[i - 1].transform.position;
            //PlayerMove();
            yield return new WaitForSecondsRealtime(0.0f);
        }
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
                targetPos += dir * GameManager.LevelController.gridCellScale;
                bool jumpable = Physics.Raycast(targetPos, Vector3.down, GameManager.LevelController.gridCellScale * 0.20f);
                if (walkable)
                {
                    Move(gridMovement, animTime);
                    StartCoroutine(DelayedCheckObjectPushable(gridMovement, animTime));
                }
                else
                {
                    if (jumpable)
                    {
                        Move(gridMovement * 2.0f, Vector3.up * jumpHeight, animTime * jumpTimescale);
                        StartCoroutine(DelayedCheckObjectPushable(gridMovement, animTime * jumpTimescale));
                    }
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

        Prompt interactPrompt = GameManager.UIController.hud.promptInteract;

        if (interactPrompt != null)
        {
            if (canGrabFacing && objectMoving == null)
            {
                interactPrompt.Show(true);
                interactPrompt.SetText(1, "Grab Object", AdjustCondition.Always);
            }
            else if (!canGrabFacing && objectMoving == null)
            {
                interactPrompt.Show(false);
            }
        }
    }

    public IEnumerator DelayedCheckObjectPushable(Vector3 dir, float time)
    {
        yield return new WaitForSeconds(time);
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
            //CheckObjectPushable(grabDir);
        }

        Prompt interactPrompt = GameManager.UIController.hud.promptInteract;
        Prompt rotCWPrompt = GameManager.UIController.hud.promptRotCW;
        Prompt rotCCWPrompt = GameManager.UIController.hud.promptRotCCW;

        if (grabSuccess && interactPrompt.visible)
        {
            interactPrompt.SetText(1, "Release Object", AdjustCondition.Always);
            if (objectMoving.rotatable)
            {
                rotCWPrompt.Show(true);
                rotCWPrompt.SetText(1, "Rotate Clockwise", AdjustCondition.Always);
                rotCCWPrompt.Show(true);
                rotCCWPrompt.SetText(1, "Rotate Counter-Clockwise", AdjustCondition.Always);
            }
        }

        return grabSuccess;
    }

    public void Release()
    {
        objectMoving = null;
        objectDir = Vector3.zero;
        CheckObjectPushable(GetGridDir(facingDir));

        Prompt rotCWPrompt = GameManager.UIController.hud.promptRotCW;
        Prompt rotCCWPrompt = GameManager.UIController.hud.promptRotCCW;
        rotCWPrompt.Show(false);
        rotCCWPrompt.Show(false);
    }

    public Vector3 GetObjectMovingDir()
    {
        float x = objectMoving.transform.position.x - transform.position.x;
        float z = objectMoving.transform.position.z - transform.position.z;
        Vector3 dirObjectMoving = new Vector3(x, 0.0f, z);
        float angleObjectMoving = GetGridBearing(dirObjectMoving);
        objectDir = new Vector3(Mathf.Sin(ToRad(angleObjectMoving)), 0.0f, Mathf.Cos(ToRad(angleObjectMoving)));
        return objectDir;
    }

    public bool CanRotateAround(Vector3 gridPivot, float gridRot)
    {
        bool rotObstructed = false;

        Vector3 startPos = transform.position + checkOffset;
        Vector3 pivot = gridPivot;
        pivot.y = startPos.y;
        float dist = Vector3.Distance(startPos, pivot);
        Vector3 startFacing = (startPos - pivot).normalized;
        float startAngle = GetGridBearing(startFacing);
        float rotAngle = 90.0f * gridRot;

        int stepCount = (int)(dist / GameManager.LevelController.gridCellScale) * 8;
        if (stepCount < 4)
        {
            stepCount = 4;
        }
        stepCount = (int)((Mathf.Abs(rotAngle)/360.0f) * (float)stepCount);

        startAngle = ToRad(startAngle);
        rotAngle = ToRad(rotAngle);
        float rotSegment = rotAngle / (float)stepCount;

        Vector3[] points = new Vector3[stepCount + 1];
        points[0] = startPos;
        float y = startPos.y;

        for (int i = 1; i < points.Length; i++)
        {
            float angleDelta = startAngle + rotSegment * (float)i;
            float x = pivot.x + dist * Mathf.Sin(angleDelta);
            float z = pivot.z + dist * Mathf.Cos(angleDelta);
            points[i] = new Vector3(x, y, z);
        }

        for (int i = 1; i < points.Length; i++)
        {
            Vector3 step = points[i] - points[i - 1];
            Vector3 stepDir = step.normalized;
            float stepDist = step.magnitude;
            if (Physics.Raycast(points[i - 1], stepDir, stepDist) || !Physics.Raycast(points[i], Vector3.down, GameManager.LevelController.gridCellScale * 0.20f))
            {
                rotObstructed = true;
                break;
            }
        }

        return !rotObstructed;
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private void Debugging()
    {
        GameManager.UIController.moveFrames = animFramesPassed;
        if (isMoving)
        {
            moveTimeElapsed += Time.deltaTime;
        }
        else if (moveTimeElapsed > 0.0f)
        {
            moveTimeElapsed = 0.0f;
        }
    }
}
