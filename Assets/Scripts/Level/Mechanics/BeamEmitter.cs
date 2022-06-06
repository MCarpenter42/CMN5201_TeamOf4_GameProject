using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class BeamEmitter : Core
{
    #region [ PROPERTIES ]

    [SerializeField] GameObject beamPrefab;
    [SerializeField] int beamID = 0;

    public bool isEmitting = true;

    private List<Vector3> beamPoints = new List<Vector3>();
    private List<Vector3> beamPath = new List<Vector3>();
    private GameObject beamObj;
    private LineRenderer lightBeam;

    private bool validTriggerHit = false;
    private LightTrigger triggerCurrent = null;
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        
    }

    void Start()
    {
        beamObj = Instantiate(beamPrefab);

        if (beamPrefab != null)
        {
            LineRenderer lnRndr = beamObj.GetComponent<LineRenderer>();
            if (lnRndr != null)
            {
                lightBeam = lnRndr;
            }
            else
            {
                Debug.LogError("Error: prefab for light beam renderer is missing a LineRenderer component. Please add one, or use a different prefab.");
            }
        }
        else
        {
            Debug.LogError("Error: prefab for light beam renderer not available. Please add one in the inspector.");
        }
    }

    void Update()
    {
        if (isEmitting)
        {
            EmitBeam();
        }
    }

    void FixedUpdate()
    {
        
    }

	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    private void EmitBeam()
    {
        beamPoints.Clear();
        beamPoints.Add(transform.position);
        int n = 0;
        Vector3[] output = new Vector3[] { transform.position, transform.forward };
        while(true)
        {
            if (output[1] == Vector3.zero)
            {
                break;
            }
            else
            {
                output = BeamCast(output[0], output[1], n);
                n++;
            }
        }
        DrawBeam();
    }

    private Vector3[] BeamCast(Vector3 origin, Vector3 dir, int step)
    {
        RaycastHit hit;
        bool didHit = Physics.Raycast(origin, dir, out hit, 300.0f);
        Vector3 endPoint = hit.point;
        if (didHit)
        {
            beamPoints.Add(endPoint);
            GameObject hitObj = hit.collider.gameObject;
            if (hitObj.GetComponent<Mirror>() != null)
            {
                Vector3 newDir = hitObj.GetComponent<Mirror>().ReflectedVector(dir);
                return new Vector3[] { hit.point, newDir };
            }
            else
            {
                if (triggerCurrent == null && hitObj.GetComponent<LightTrigger>() != null)
                {
                    LightTrigger triggerHit = hitObj.GetComponent<LightTrigger>();
                    if (triggerHit.IDCheck(beamID))
                    {
                        triggerCurrent = triggerHit;
                        triggerCurrent.ChangeTriggerState(true);
                    }
                }
                else if (triggerCurrent != null && hitObj.GetComponent<LightTrigger>() == null)
                {
                    triggerCurrent.ChangeTriggerState(false);
                    triggerCurrent = null;
                }
                return new Vector3[] { hit.point, Vector3.zero };
            }
        }
        else
        {
            return new Vector3[] { hit.point, Vector3.zero };
        }
    }

    private bool CheckNew()
    {
        bool pathChanged = false;
        if (beamPoints.Count == beamPath.Count)
        {
            for (int i = 0; i < beamPoints.Count; i++)
            {
                if (beamPoints[i].x != beamPath[i].x || beamPoints[i].z != beamPath[i].z)
                {
                    pathChanged = true;
                    break;
                }
            }
        }
        else
        {
            pathChanged = true;
        }
        return pathChanged;
    }

    private void DrawBeam()
    {
        beamPath.Clear();
        CopyListData(beamPoints, beamPath);

        float beamLength = 0.0f;
        Vector3 endPos = Vector3.zero;
        lightBeam.positionCount = beamPath.Count;
        Vector3[] positions = new Vector3[beamPath.Count];
        for (int i = 0; i < beamPath.Count; i++)
        {
            positions[i] = beamPath[i];
            if (i > 0)
            {
                beamLength += Vector3.Distance(beamPath[i - 1], beamPath[i]);
            }
            endPos = beamPath[i];
        }
        lightBeam.SetPositions(positions);
    }
}
