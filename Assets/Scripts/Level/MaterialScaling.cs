using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

[ExecuteInEditMode]
public class MaterialScaling : Core
{
    #region [ PROPERTIES ]

    private enum Dims { XY, XZ, YZ };
    private enum ScaleType { Standard, FromParent, ToOther };

    [Header("Materials")]
    [SerializeField] Material[] materials = new Material[1];
    private Material[] localMaterials;
    [SerializeField] Vector2 scaleAdjustment = Vector2.one;
    [SerializeField] Dims surfaceScaleDimensions = Dims.XZ;
    [SerializeField] Vector2 materialOffset = Vector2.zero;

    [Header("Options")]
    [SerializeField] ScaleType scaleType = ScaleType.Standard;
    [SerializeField] MeshRenderer[] affectedMeshes;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Start()
    {
        Rescale();
    }

#if UNITY_EDITOR
    void Update()
    {
        Transform transform = gameObject.GetComponent<Transform>();
        if (scaleType == ScaleType.FromParent)
        {
            if (transform.parent.hasChanged)
            {
                Rescale();
                transform.parent.hasChanged = false;
            }
        }
        else
        {
            if (transform.hasChanged)
            {
                Rescale();
                transform.hasChanged = false;
            }
        }
    }

    void OnValidate()
    {
        Rescale();
    }
#endif

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private void GetMats()
    {
        localMaterials = new Material[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            if (materials[i] != null)
            {
                Material newMat = new Material(materials[i]);
                newMat.name = "localMat_" + i;
                localMaterials[i] = newMat;
            }
        }
    }

    private void Rescale()
    {
        if (materials != null && materials.Length > 0)
        {
            GetMats();
        }

        Vector2 surfaceSize = Vector2.one;
        Vector2 matScale = Vector2.one;
        if (scaleType == ScaleType.FromParent)
        {
            switch (surfaceScaleDimensions)
            {
                case Dims.XY:
                    surfaceSize.x = transform.parent.localScale.x;
                    surfaceSize.y = transform.parent.localScale.y;
                    break;

                case Dims.XZ:
                    surfaceSize.x = transform.parent.localScale.x;
                    surfaceSize.y = transform.parent.localScale.z;
                    break;

                case Dims.YZ:
                    surfaceSize.x = transform.parent.localScale.y;
                    surfaceSize.y = transform.parent.localScale.z;
                    break;

                default:
                    break;
            }
        }
        else
        {
            switch (surfaceScaleDimensions)
            {
                case Dims.XY:
                    surfaceSize.x = transform.localScale.x;
                    surfaceSize.y = transform.localScale.y;
                    break;

                case Dims.XZ:
                    surfaceSize.x = transform.localScale.x;
                    surfaceSize.y = transform.localScale.z;
                    break;

                case Dims.YZ:
                    surfaceSize.x = transform.localScale.y;
                    surfaceSize.y = transform.localScale.z;
                    break;

                default:
                    break;
            }
        }

        matScale = surfaceSize;
        matScale.x *= scaleAdjustment.x;
        matScale.y *= scaleAdjustment.y;

        if (localMaterials.Length > 0)
        {
            foreach (Material mat in localMaterials)
            {
                if (mat != null)
                {
                    mat.mainTextureScale = matScale;
                    mat.mainTextureOffset = materialOffset;
                }
            }

            if (scaleType == ScaleType.ToOther)
            {
                foreach (MeshRenderer rndr in affectedMeshes)
                {
                    if (rndr != null)
                    {
                        rndr.materials = localMaterials;
                    }
                }
            }
            else
            {
                MeshRenderer rndr = gameObject.GetComponent<MeshRenderer>();
                if (rndr != null)
                {
                    rndr.materials = localMaterials;
                }
            }
        }
    }
}
