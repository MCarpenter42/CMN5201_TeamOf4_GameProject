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

    private enum Dims { Default, XY, XZ, YZ };
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
    [SerializeField] bool addPositionToOffset = false;
    [SerializeField] Dims posDimensionsToAdd = Dims.Default;
    [SerializeField] bool invertPosOffset = true;
    [SerializeField] float posOffsetScale = 1.0f;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Start()
    {
        if (surfaceScaleDimensions == Dims.Default)
        {
            surfaceScaleDimensions = Dims.XZ;
        }
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
        Vector2 matOffset = materialOffset;
        if (scaleType == ScaleType.FromParent)
        {
            switch (surfaceScaleDimensions)
            {
                default:
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
            }
        }
        else
        {
            switch (surfaceScaleDimensions)
            {
                default:
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
            }
        }

        matScale = surfaceSize;
        matScale.x *= scaleAdjustment.x;
        matScale.y *= scaleAdjustment.y;
        if (addPositionToOffset)
        {
            matOffset += PositionOffset();
        }

        if (localMaterials.Length > 0)
        {
            foreach (Material mat in localMaterials)
            {
                if (mat != null)
                {
                    mat.mainTextureScale = matScale;
                    mat.mainTextureOffset = matOffset;
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

    private Vector2 PositionOffset()
    {
        Vector2 offset = materialOffset;
        Dims dims = posDimensionsToAdd;
        if (dims == Dims.Default)
        {
            dims = surfaceScaleDimensions;
        }
        switch (dims)
        {
            default:
            case Dims.XY:
                offset.x += transform.position.x;
                offset.y += transform.position.y;
                break;

            case Dims.XZ:
                offset.x += transform.position.x;
                offset.y += transform.position.z;
                break;

            case Dims.YZ:
                offset.x += transform.position.y;
                offset.y += transform.position.z;
                break;
        }
        if (invertPosOffset)
        {
            return -offset * posOffsetScale;
        }
        else
        {
            return offset * posOffsetScale;
        }
    }
}
