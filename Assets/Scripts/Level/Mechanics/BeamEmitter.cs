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

    [Header("Beam")]
    [SerializeField] GameObject beamPrefab;
    [SerializeField] BeamColours beamColour = BeamColours.White;

    public bool isEmitting = true;

    private List<Vector3> beamPoints = new List<Vector3>();
    private List<Vector3> beamPath = new List<Vector3>();
    private GameObject beamObj;
    private LineRenderer lightBeam;

    private List<SFXSource> beamAudio = new List<SFXSource>();

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

                Gradient gradient = new Gradient();
                switch (beamColour)
                {
                    case BeamColours.White:
                        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(new Color(1.0f, 1.0f, 1.0f), 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f) });
                        break;
                        
                    case BeamColours.Red:
                        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(new Color(1.0f, 0.0f, 0.0f), 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f) });
                        break;
                        
                    case BeamColours.Green:
                        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(new Color(0.0f, 1.0f, 0.0f), 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f) });
                        break;
                        
                    case BeamColours.Blue:
                        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(new Color(0.0f, 0.0f, 1.0f), 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f) });
                        break;

                    default:
                        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(new Color(1.0f, 1.0f, 1.0f), 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f) });
                        break;
                }
                lightBeam.colorGradient = gradient;
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
        Vector3[] output = new Vector3[] { transform.position, transform.forward };
        while(true)
        {
            if (output[1] == Vector3.zero)
            {
                break;
            }
            else
            {
                output = BeamCast(output[0], output[1]);
            }
        }
        DrawBeam();
    }

    private Vector3[] BeamCast(Vector3 origin, Vector3 dir)
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
                    if (triggerHit.ColourCheck(beamColour))
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
