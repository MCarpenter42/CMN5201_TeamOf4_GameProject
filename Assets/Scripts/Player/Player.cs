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

    [Header("Properties")]
    [SerializeField] float jumpHeight = 0.5f;
    [SerializeField] float jumpTime = 0.4f;
    [SerializeField] float jumpWindup = 0.1f;
    [SerializeField] int stepsPerTile = 2;

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
        jumpTime += jumpWindup;
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
            GameManager.LevelController.NextLevel();
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

    public void PlayerMove(Vector3 gridMovement)
    {
        PlayerMove(gridMovement, 1.0f);
    }
    
    public void PlayerMove(Vector3 gridMovement, float speedMod)
    {
        Vector3 dir = gridMovement.normalized;
        float moveDuration = 0.0f;
        if (objectMoving == null)
        {
            moveDuration = moveTime / speedMod;
            bool obstructed = Physics.Raycast(transform.position + checkOffset, dir, GameManager.LevelController.gridCellScale * 1.4f);
            if (!obstructed)
            {
                Vector3 targetPos = transform.position + checkOffset + dir * GameManager.LevelController.gridCellScale;
                bool walkable = Physics.Raycast(targetPos, Vector3.down, GameManager.LevelController.gridCellScale * 0.20f);

                targetPos += dir * GameManager.LevelController.gridCellScale;
                bool jumpable = Physics.Raycast(targetPos, Vector3.down, GameManager.LevelController.gridCellScale * 0.20f);

                obstructed = Physics.Raycast(transform.position + checkOffset, dir, GameManager.LevelController.gridCellScale * 2.4f);

                if (walkable)
                {
                    Move(gridMovement, moveDuration);
                    StartCoroutine(DelayedCheckObjectMovable(gridMovement, moveDuration));
                    int steps = (int)((gridMovement.magnitude / GameManager.LevelController.gridCellScale) * (float)stepsPerTile);
                    GameManager.AudioController.PlayerWalk(moveDuration, steps);
                }
                else
                {
                    if (jumpable && !obstructed)
                    {
                        moveDuration = jumpTime;
                        Move(gridMovement * 2.0f, moveDuration, Vector3.up * jumpHeight, jumpWindup);
                        StartCoroutine(DelayedCheckObjectMovable(gridMovement, moveDuration));
                        GameManager.AudioController.PlayerJump(moveDuration);
                    }
                }
            }
        }
        else if (objectMoving.movable)
        {
            moveDuration = objectMoving.moveTime / speedMod;
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
                        Move(gridMovement, moveDuration);
                        objectMoving.Move(gridMovement, moveDuration);
                        StartCoroutine(DelayedCheckObjectMovable(gridMovement, moveDuration));
                        int steps = (int)((gridMovement.magnitude / GameManager.LevelController.gridCellScale) * (float)stepsPerTile);
                        GameManager.AudioController.PlayerWalk(moveDuration, steps);
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
                        Move(gridMovement, moveDuration);
                        objectMoving.Move(gridMovement, moveDuration);
                        StartCoroutine(DelayedCheckObjectMovable(gridMovement, moveDuration));
                        int steps = (int)((gridMovement.magnitude / GameManager.LevelController.gridCellScale) * (float)stepsPerTile);
                        GameManager.AudioController.PlayerWalk(moveDuration, steps);
                    }
                }
            }
        }
        ChangeFacing(gridMovement);
    }

    public void DelayedPlayerMove(float delay, Vector3 gridMovement)
    {
        StartCoroutine(IDelayedPlayerMove(delay, gridMovement, 1.0f));
    }
    
    public void DelayedPlayerMove(float delay, Vector3 gridMovement, float speedMod)
    {
        StartCoroutine(IDelayedPlayerMove(delay, gridMovement, speedMod));
    }

    private IEnumerator IDelayedPlayerMove(float delay, Vector3 gridMovement, float speedMod)
    {
        yield return new WaitForSecondsRealtime(delay);
        PlayerMove(gridMovement, speedMod);
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

        CheckObjectMovable(dir);
    }

    public void CheckObjectMovable(Vector3 dir)
    {
        RaycastHit hit;
        Physics.Raycast(transform.position + checkOffset, dir, out hit, GameManager.LevelController.gridCellScale * 1.10f);
        if (hit.collider != null)
        {
            LevelObject obj = hit.collider.gameObject.GetComponent<LevelObject>();
            if (obj != null && (obj.movable || obj.rotatable))
            {
                canGrabFacing = true;
                obj.GlowPulse(2, 0.9f);
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

        Prompt interactPrompt = GameManager.UIController.hud.keyInteract;

        if (interactPrompt != null)
        {
            if (canGrabFacing && objectMoving == null)
            {
                interactPrompt.Show(true);
                interactPrompt.SetText(1, "Grab Object", AdjustCondition.Always, AdjustCondition.Never);
            }
            else if (!canGrabFacing && objectMoving == null)
            {
                interactPrompt.Show(false);
            }
        }
    }

    public IEnumerator DelayedCheckObjectMovable(Vector3 dir, float time)
    {
        yield return new WaitForSeconds(time);
        CheckObjectMovable(dir);
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
            //CheckObjectMovable(grabDir);
        }

        Prompt interactPrompt = GameManager.UIController.hud.keyInteract;
        Prompt rotCWPrompt = GameManager.UIController.hud.keyRotCW;
        Prompt rotCCWPrompt = GameManager.UIController.hud.keyRotCCW;

        if (grabSuccess && interactPrompt.visible)
        {
            interactPrompt.SetText(1, "Release Object", AdjustCondition.Always, AdjustCondition.Never);
            if (objectMoving.rotatable)
            {
                rotCWPrompt.Show(true);
                rotCWPrompt.SetText(1, "Rotate Clockwise", AdjustCondition.Always, AdjustCondition.Never);
                rotCCWPrompt.Show(true);
                rotCCWPrompt.SetText(1, "Rotate Counter-Clockwise", AdjustCondition.Always, AdjustCondition.Never);
            }
        }

        return grabSuccess;
    }

    public void Release()
    {
        objectMoving = null;
        objectDir = Vector3.zero;
        CheckObjectMovable(GetGridDir(facingDir));

        Prompt rotCWPrompt = GameManager.UIController.hud.keyRotCW;
        Prompt rotCCWPrompt = GameManager.UIController.hud.keyRotCCW;
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

    public FloorTypes GetFloorType()
    {
        FloorTypes type = FloorTypes.Empty;
        Vector3 castPos = transform.position + checkOffset;
        RaycastHit hit;
        if (Physics.Raycast(castPos, Vector3.down, out hit, GameManager.LevelController.gridCellScale * 0.20f))
        {
            StaticSurface floor = hit.collider.gameObject.GetComponent<StaticSurface>();
            if (floor != null)
            {
                type = floor.material;
            }
        }
        return type;
    }

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private void Debugging()
    {
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
