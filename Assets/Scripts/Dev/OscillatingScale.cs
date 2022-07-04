using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class OscillatingScale : Core
{
    #region [ PROPERTIES ]

    private Vector3 baseScale;
    [SerializeField] Vector3 scaleDeviation = Vector3.zero;
    [SerializeField] float oscillationRate = 1.0f;
    private float oscillationPeriod = 1.0f;
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Start()
    {
        baseScale = transform.localScale;
        oscillationPeriod = 1 / oscillationRate;
        StartCoroutine(Oscillate());
    }
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    private IEnumerator Oscillate()
    {
        float timePassed = Random.Range(0.0f, oscillationPeriod * 0.95f);
        while (true)
        {
            yield return null;
            timePassed += Time.deltaTime;
            if (timePassed > oscillationPeriod)
            {
                timePassed -= oscillationPeriod;
            }
            float delta = Mathf.Sin((timePassed / oscillationPeriod) * Mathf.PI);
            transform.localScale = baseScale + scaleDeviation * delta;
        }
    }
}
