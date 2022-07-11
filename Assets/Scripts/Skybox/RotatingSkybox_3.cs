using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingSkybox_3 : MonoBehaviour
{
    [SerializeField]
    private float speedRot;
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * speedRot);
    }
}
