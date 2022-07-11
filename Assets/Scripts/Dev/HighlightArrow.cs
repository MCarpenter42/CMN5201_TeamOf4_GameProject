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

    [Header("Components")]
    [SerializeField] GameObject arrowAbove;
    [SerializeField] GameObject arrowRing;

    [Header("Settings")]
    [SerializeField] ArrowType type = ArrowType.AboveObject;
    [Range(0.5f, 5.0f)]
    [SerializeField] float displacement = 0.5f;
    [SerializeField] Material[] materials;
    [Range(0.0f, 1.0f)]
    [SerializeField] float opacity = 1.0f;
    [Range(0.0f, 10.0f)]
    [SerializeField] float rotationSpeed = 1.0f;
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        SetOffset();
        if (materials.Length != 0)
        {
            SetMaterials();
        }
        SetRotSpeed();
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
        }
    }
	
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
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
            arrowRing.transform.GetChild(0).localPosition += new Vector3(0.0f, 0.0f, displacement);
            arrowRing.transform.GetChild(1).localPosition += new Vector3(displacement, 0.0f, 0.0f);
            arrowRing.transform.GetChild(2).localPosition += new Vector3(0.0f, 0.0f, -displacement);
            arrowRing.transform.GetChild(3).localPosition += new Vector3(-displacement, 0.0f, 0.0f);
        }
    }

    private void SetMaterials()
    {
        Material[] localMaterials = new Material[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i] != null)
            {
                Material newMat = new Material(materials[i]);
                newMat.name = "localMat_" + i;
                //newMat.shader = Shader.Find("Transparent/Standard");
                Color matClr = newMat.color;
                matClr.a = opacity;
                newMat.color = matClr;
                localMaterials[i] = newMat;
            }
        }

        if (type == ArrowType.AboveObject)
        {
            arrowAbove.gameObject.GetComponent<MeshRenderer>().materials = localMaterials;
        }
        else
        {
            arrowRing.transform.GetChild(0).GetComponent<MeshRenderer>().materials = localMaterials;
            arrowRing.transform.GetChild(1).GetComponent<MeshRenderer>().materials = localMaterials;
            arrowRing.transform.GetChild(2).GetComponent<MeshRenderer>().materials = localMaterials;
            arrowRing.transform.GetChild(3).GetComponent<MeshRenderer>().materials = localMaterials;
        }
    }

    private void SetRotSpeed()
    {
        arrowAbove.gameObject.GetComponent<ConstantRotation>().rotation = Vector3.up * 50.0f * rotationSpeed;
        arrowRing.gameObject.GetComponent<ConstantRotation>().rotation = Vector3.up * 50.0f * rotationSpeed;
    }

}
