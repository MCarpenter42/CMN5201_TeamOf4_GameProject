using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class HighlightGlow : Core
{
    #region [ PROPERTIES ]

    [SerializeField] List<MeshRenderer> meshes = new List<MeshRenderer>();

    private List<Material[]> baseMaterials = new List<Material[]>();

    [SerializeField] Color emissiveColour = Color.white;
    [SerializeField] float emissivePulseRate = 1.2f;

    private Coroutine glowAnim = null;

    private bool glowActive = false;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void Awake()
    {
        
    }

    void Start()
    {
        
    }
	
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    public void PulseSequence(int pulses)
    {
        if (baseMaterials.Count == 0)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                baseMaterials.Add(meshes[i].materials);
            }
        }

        if (glowAnim != null)
        {
            StopCoroutine(glowAnim);
        }
        glowAnim = StartCoroutine(IGlowPulse(pulses, emissivePulseRate));
    }

    public void PulseSequence(int pulses, float rate)
    {
        if (baseMaterials.Count == 0)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                baseMaterials.Add(meshes[i].materials);
            }
        }

        if (glowAnim != null)
        {
            StopCoroutine(glowAnim);
        }
        glowAnim = StartCoroutine(IGlowPulse(pulses, rate));
    }

    public IEnumerator IGlowPulse(int pulses, float rate)
    {
        List<Material[]> localMaterials = baseMaterials;
        for (int i = 0; i < meshes.Count; i++)
        {
            Material[] mats = localMaterials[i];
            foreach (Material mat in mats)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.black);
            }
            meshes[i].materials = mats;
        }

        float pulseTime = 1 / rate;
        float timePassed;
        for (int i = 0; i < pulses; i++)
        {
            timePassed = 0.0f;
            while (timePassed < pulseTime)
            {
                yield return null;
                timePassed += Time.deltaTime;
                float delta = InterpDelta.CosHill(timePassed / pulseTime);
                Color clr = Color.Lerp(Color.black, emissiveColour, delta);
                for (int j = 0; j < meshes.Count; j++)
                {
                    Material[] mats = localMaterials[j];
                    foreach (Material mat in mats)
                    {
                        mat.SetColor("_EmissionColor", clr);
                    }
                    meshes[j].materials = mats;
                }
            }
        }
        for (int i = 0; i < meshes.Count; i++)
        {
            meshes[i].materials = baseMaterials[i];
        }
    }

    public void Toggle(bool active)
    {
        if (active != glowActive)
        {
            if (glowAnim != null)
            {
                StopCoroutine(glowAnim);
            }
            glowAnim = StartCoroutine(ToggleTransition(active, 0.5f));
        }
    }
    
    public void Toggle(bool active, float transitionTime)
    {
        if (active != glowActive)
        {
            if (glowAnim != null)
            {
                StopCoroutine(glowAnim);
            }
            glowAnim = StartCoroutine(ToggleTransition(active, transitionTime));
        }
    }

    public IEnumerator ToggleTransition(bool active, float transitionTime)
    {
        List<Material[]> localMaterials = baseMaterials;
        for (int i = 0; i < meshes.Count; i++)
        {
            Material[] mats = localMaterials[i];
            foreach (Material mat in mats)
            {
                mat.EnableKeyword("_EMISSION");
                mat.SetColor("_EmissionColor", Color.black);
            }
            meshes[i].materials = mats;
        }

        Color clrStart = Color.black;
        Color clrTarget = Color.black;
        if (active)
        {
            clrTarget = emissiveColour;
        }
        else
        {
            clrStart = emissiveColour;
        }

        float timePassed = 0.0f;
        while (timePassed <= transitionTime)
        {
            yield return null;
            timePassed += Time.deltaTime;
            float delta = timePassed / transitionTime;
            Color clr = Color.Lerp(clrStart, clrTarget, delta);

            for (int j = 0; j < meshes.Count; j++)
            {
                Material[] mats = localMaterials[j];
                foreach (Material mat in mats)
                {
                    mat.SetColor("_EmissionColor", clr);
                }
                meshes[j].materials = mats;
            }
        }

        if (active)
        {
            for (int j = 0; j < meshes.Count; j++)
            {
                Material[] mats = localMaterials[j];
                foreach (Material mat in mats)
                {
                    mat.SetColor("_EmissionColor", clrTarget);
                }
                meshes[j].materials = mats;
            }
        }
        else
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                meshes[i].materials = baseMaterials[i];
            }
        }
    }

}
