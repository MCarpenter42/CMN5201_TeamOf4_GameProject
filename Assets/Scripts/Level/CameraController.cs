using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class CameraController : Core
{
    #region [ PROPERTIES ]

    [HideInInspector] public GameObject cameraMount;
    [HideInInspector] public bool isMoving = false;
    private Coroutine movement = null;
    [HideInInspector] public GameObject cameraPivot;
    [HideInInspector] public bool isRotating = false;
    private Coroutine rotation = null;
    [HideInInspector] public GameObject cameraZoom;
    [HideInInspector] public bool isZooming = false;
    private Coroutine zoom = null;

    [HideInInspector] public CompassBearing facing = CompassBearing.North;

    [SerializeField] bool useLevelGridScale = false;
    [SerializeField] List<Vector3> cameraPoints = new List<Vector3>() { Vector3.zero };
    [SerializeField] float minPitch = 45.0f;
    [SerializeField] float maxPitch = 75.0f;
    [Range(-24, -8)]
    [SerializeField] int minZoom = -24;
    [Range(-20, -4)]
    [SerializeField] int maxZoom = -8;
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        cameraMount = transform.GetChild(0).gameObject;
        cameraPivot = cameraMount.transform.GetChild(0).gameObject;
        cameraZoom = cameraPivot.transform.GetChild(0).gameObject;

        minPitch = Mathf.Clamp(minPitch, 0.0f, 80.0f);
        maxPitch = Mathf.Clamp(maxPitch, minPitch, 90.0f);
        minZoom = Mathf.Clamp(minZoom, -50, -1);
        maxZoom = Mathf.Clamp(maxZoom, minZoom + 1, 0);
    }

    void Start()
    {
        if (useLevelGridScale)
        {
            cameraMount.transform.position = cameraPoints[0] * GameManager.LevelController.gridCellScale;
        }
        else
        {
            cameraMount.transform.position = cameraPoints[0];
        }
    }
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    public bool DoRotation(Vector2Int rotIntervals, float duration)
    {
        if (!isRotating)
        {
            this.rotation = StartCoroutine(RotatePivot(rotIntervals, duration));
        }
        return !isRotating;
    }

    private IEnumerator RotatePivot(Vector2Int rotIntervals, float duration)
    {
        isRotating = true;

        Vector3 rotation = Vector3.zero;
        rotation.x = (float)rotIntervals.x * 5.0f;
        rotation.y = (float)rotIntervals.y * 90.0f;

        Vector3 rotStart = cameraPivot.transform.localEulerAngles;
        rotStart.y = WrapClamp(rotStart.y, -180.0f, 180.0f);
        Vector3 rotTarget = rotStart + rotation;
        rotTarget.x = Mathf.Clamp(rotTarget.x, minPitch, maxPitch);

        float timePassed = 0.0f;
        while (timePassed <= duration)
        {
            yield return null;
            timePassed += Time.deltaTime;
            float delta = InterpDelta.CosCurve(timePassed / duration);
            cameraPivot.transform.localEulerAngles = Vector3.Lerp(rotStart, rotTarget, delta);
        }
        cameraPivot.transform.localEulerAngles = rotTarget;

        float endFacingRaw = (int)facing + rotIntervals.y;
        int endFacing = (int)WrapClamp(endFacingRaw, -0.5f, 3.5f);
        facing = (CompassBearing)endFacing;

        isRotating = false;
    }

    public bool DoZoom(int distance, float duration)
    {
        if (!isZooming)
        {
            this.zoom = StartCoroutine(Zoom(distance, duration));
        }
        return !isZooming;
    }

    private IEnumerator Zoom(int distance, float duration)
    {
        isZooming = true;

        Vector3 posStart = cameraZoom.transform.localPosition;
        Vector3 posTarget = posStart;
        posTarget.z = Mathf.Clamp(posTarget.z + distance, minZoom, maxZoom);

        float timePassed = 0.0f;
        while (timePassed <= duration)
        {
            yield return null;
            timePassed += Time.deltaTime;
            float delta = timePassed / duration;
            cameraZoom.transform.localPosition = Vector3.Lerp(posStart, posTarget, delta);
        }
        cameraZoom.transform.localPosition = posTarget;

        isZooming = false;
    }
}
