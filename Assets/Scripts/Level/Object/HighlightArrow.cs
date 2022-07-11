using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class HighlightArrow : Core
{
    #region [ PROPERTIES ]

    private enum ArrowType { AboveObject, Ring };
    private enum StateTrigger { None, Movement, Rotation, Either };

    [Header("Components")]
    [SerializeField] GameObject arrowAbove;
    [SerializeField] GameObject arrowRing;
    private Transform[] ringArrows = new Transform[4];
    private Vector3[] ringArrowAnchors = new Vector3[4];

    [Header("Animation")]
    [SerializeField] ArrowType type = ArrowType.AboveObject;
    [Range(0.5f, 5.0f)]
    [SerializeField] float displacement = 0.5f;
    [SerializeField] Material material_1;
    [SerializeField] Material material_2;
    [Range(0.0f, 1.0f)]
    [SerializeField] float opacity = 1.0f;
    [Range(0.0f, 1.0f)]
    [SerializeField] float rotationRate = 0.5f;
    private Vector3 rotVector = Vector3.up;
    [Range(0.0f, 4.0f)]
    [SerializeField] float oscillationPeriod = 1.0f;
    [Range(0.0f, 4.0f)]
    [SerializeField] float posDeviation = 1.0f;

    [Header("Target Settings")]
    [SerializeField] LevelObject targetObject;
    [SerializeField] StateTrigger triggerOnEvent;
    private Vector3 objPosStart;
    private Vector3 objRotStart;

    // SHOW/HIDE ANIM
    [SerializeField] bool startHidden;
    private float animTime = 0.25f;
    private Vector3 visiblePos;
    private Vector3 visibleScale;
    private bool visible = true;
    [SerializeField] bool canReTrigger = false;
    private bool triggered = false;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        ringArrows[0] = arrowRing.transform.GetChild(0);
        ringArrows[1] = arrowRing.transform.GetChild(1);
        ringArrows[2] = arrowRing.transform.GetChild(2);
        ringArrows[3] = arrowRing.transform.GetChild(3);

        SetOffset();
        SetMaterials();

        rotVector *= 360.0f * rotationRate;
    }

    void Start()
    {
        if (type == ArrowType.AboveObject)
        {
            arrowAbove.gameObject.SetActive(true);
            arrowRing.gameObject.SetActive(false);
        }
        else
        {
            arrowAbove.gameObject.SetActive(false);
            arrowRing.gameObject.SetActive(true);
            StartCoroutine(Displacement());
        }

        visibleScale = transform.localScale;
        if (startHidden)
        {
            arrowAbove.gameObject.SetActive(false);
            arrowRing.gameObject.SetActive(false);
            visible = false;
        }

        if (targetObject != null)
        {
            objPosStart = targetObject.transform.position;
            if (targetObject.pivot != null)
            {
                objRotStart = targetObject.pivot.transform.eulerAngles;
            }
        }
    }

    void Update()
    {
        if (!canReTrigger)
        {
            if (!triggered && TriggerVisChange())
            {
                triggered = true;
                StartCoroutine(ScaleShow(!visible));
            }
        }
        else
        {
            if (TriggerVisChange())
            {
                triggered = true;
                StartCoroutine(ScaleShow(!visible));
            }
        }
    }

    void FixedUpdate()
    {
        if (type == ArrowType.AboveObject)
        {
            arrowAbove.transform.Rotate(Time.fixedDeltaTime * rotVector);
        }
        else
        {
            arrowRing.transform.Rotate(Time.fixedDeltaTime * rotVector);
        }
    }

	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    private void SetOffset()
    {
        if (type == ArrowType.AboveObject)
        {
            arrowAbove.transform.localPosition += new Vector3(0.0f, displacement, 0.0f);
        }
        else
        {
            ringArrows[0].localPosition += new Vector3(0.0f, 0.0f, displacement);
            ringArrowAnchors[0] = ringArrows[0].localPosition;
            ringArrows[1].localPosition += new Vector3(displacement, 0.0f, 0.0f);
            ringArrowAnchors[1] = ringArrows[1].localPosition;
            ringArrows[2].localPosition -= new Vector3(0.0f, 0.0f, displacement);
            ringArrowAnchors[2] = ringArrows[2].localPosition;
            ringArrows[3].localPosition -= new Vector3(displacement, 0.0f, 0.0f);
            ringArrowAnchors[3] = ringArrows[3].localPosition;
        }
    }

    private void SetMaterials()
    {
        if (material_1 != null)
        {
            Material[] localMaterials_1 = new Material[1];
            Material[] localMaterials_2 = new Material[1];

            Material newMat_1 = new Material(material_1);
            newMat_1.name = "localMat_1";
            Color matClr_1 = newMat_1.color;
            matClr_1.a = opacity;
            newMat_1.color = matClr_1;
            localMaterials_1[0] = newMat_1;

            Material newMat_2;
            if (material_2 == null)
            {
                localMaterials_2[0] = newMat_1;
            }
            else
            {
                newMat_2 = new Material(material_2);
                newMat_2.name = "localMat_2";
                Color matClr_2 = newMat_2.color;
                matClr_2.a = opacity;
                newMat_2.color = matClr_2;
                localMaterials_2[0] = newMat_2;
            }

            if (type == ArrowType.AboveObject)
            {
                arrowAbove.gameObject.GetComponent<MeshRenderer>().materials = localMaterials_1;
            }
            else
            {
                ringArrows[0].gameObject.GetComponent<MeshRenderer>().materials = localMaterials_1;
                ringArrows[1].gameObject.GetComponent<MeshRenderer>().materials = localMaterials_2;
                ringArrows[2].gameObject.GetComponent<MeshRenderer>().materials = localMaterials_1;
                ringArrows[3].gameObject.GetComponent<MeshRenderer>().materials = localMaterials_2;
            }
        }
    }

    private IEnumerator Displacement()
    {
        float timePassed = 0.0f;
        while (true)
        {
            yield return null;
            timePassed += Time.deltaTime;
            if (timePassed > oscillationPeriod)
            {
                timePassed -= oscillationPeriod;
            }
            float delta = Mathf.Sin((timePassed / oscillationPeriod) * Mathf.PI);
            float disp = (delta + 0.5f) * posDeviation;
            ringArrows[0].localPosition = ringArrowAnchors[0] + new Vector3(0.0f, 0.0f, disp);
            ringArrows[1].localPosition = ringArrowAnchors[1] + new Vector3(disp, 0.0f, 0.0f);
            ringArrows[2].localPosition = ringArrowAnchors[2] - new Vector3(0.0f, 0.0f, disp);
            ringArrows[3].localPosition = ringArrowAnchors[3] - new Vector3(disp, 0.0f, 0.0f);
        }
    }

    private bool TriggerVisChange()
    {
        bool trigger = false;
        if (triggerOnEvent == StateTrigger.Movement)
        {
            if (targetObject.transform.position != objPosStart)
            {
                trigger = true;
            }
        }
        else if (triggerOnEvent == StateTrigger.Rotation)
        {
            if (targetObject.pivot.transform.eulerAngles != objRotStart)
            {
                trigger = true;
            }
        }
        else if (triggerOnEvent == StateTrigger.Either)
        {
            if (targetObject.transform.position != objPosStart || targetObject.pivot.transform.eulerAngles != objRotStart)
            {
                trigger = true;
            }
        }
        return trigger;
    }

    private IEnumerator ScaleShow(bool show)
    {
        if (show)
        {
            if (type == ArrowType.AboveObject)
            {
                arrowAbove.gameObject.SetActive(true);
            }
            else
            {
                arrowRing.gameObject.SetActive(true);
            }
        }

        Vector3 scaleStart = transform.localScale;
        Vector3 scaleTarget = Vector3.one;
        if (show)
        {
            scaleTarget = visibleScale;
        }
        else
        {
            scaleTarget = visibleScale * 0.01f;
        }

        float timePassed = 0.0f;
        while (timePassed <= animTime)
        {
            yield return null;
            timePassed += Time.deltaTime;
            float delta = timePassed / animTime;
            transform.localScale = Vector3.Lerp(scaleStart, scaleTarget, delta);
        }
        transform.localScale = scaleTarget;

        if (!show)
        {
            arrowAbove.gameObject.SetActive(false);
            arrowRing.gameObject.SetActive(false);
        }

        visible = show;
    }

}
