using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;

[ExecuteInEditMode]
public class ColourCycle : Core
{
	#region [ OBJECTS ]

	[SerializeField] Graphic[] targetGraphics;

    #endregion

    #region [ PROPERTIES ]

    [SerializeField] Color[] cycleColours;
    [Range(0.0f, 10.0f)]
    [SerializeField] float stepTime = 4.0f;

    private int cycleStage = 0;
    private float timePassed = 0.0f;

    #endregion

    #region [ COROUTINES ]

    private Coroutine cycle = null;

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    #region [ BUILT-IN UNITY FUNCTIONS ]

    void OnValidate()
    {
        if (cycleColours.Length > 0)
        {
            foreach (Graphic graphic in targetGraphics)
            {
                graphic.color = cycleColours[0];
            }
        }
    }

    void OnEnable()
    {
        if (Application.isPlaying && cycleColours.Length > 1)
        {
            cycle = StartCoroutine(ICycle());
        }
    }

    void OnDisable()
    {
        if (cycle != null)
        {
            StopCoroutine(cycle);
        }
    }

    #endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

    private IEnumerator ICycle()
    {
        while (true)
        {
            Color clr1 = cycleColours[cycleStage];
            int x = cycleStage + 1;
            if (x == cycleColours.Length)
            { x = 0; }
            Color clr2 = cycleColours[x];
            while (timePassed < stepTime)
            {
                yield return null;
                timePassed += Time.deltaTime;
                float delta = timePassed / stepTime;
                foreach (Graphic graphic in targetGraphics)
                {
                    graphic.color = Color.Lerp(clr1, clr2, delta);
                }
            }
            timePassed = 0;
            cycleStage = x;
        }
    }
}
