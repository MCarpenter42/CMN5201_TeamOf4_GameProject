using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class OscillatingPosition : Core
{
    #region [ PROPERTIES ]

    private Vector3 anchorPos;
    [SerializeField] Vector3 posDeviation = Vector3.zero;
    [SerializeField] float oscillationRate = 1.0f;
    private float oscillationPeriod = 1.0f;
    [SerializeField] bool randomStartingOffset = false;
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */

	#region [ BUILT-IN UNITY FUNCTIONS ]

    void Start()
    {
        anchorPos = transform.localPosition;
        oscillationPeriod = 1 / oscillationRate;
        StartCoroutine(Oscillate());
    }
	
	#endregion

    /* - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - */
	
    private IEnumerator Oscillate()
    {
        float timePassed = 0.0f;
        if (randomStartingOffset)
        {
            timePassed = Random.Range(0.0f, oscillationPeriod * 0.95f);
        };
        while (true)
        {
            yield return null;
            timePassed += Time.deltaTime;
            if (timePassed > oscillationPeriod)
            {
                timePassed -= oscillationPeriod;
            }
            float delta = Mathf.Sin((timePassed / oscillationPeriod) * Mathf.PI);
            transform.localPosition = anchorPos + posDeviation * delta;
        }
    }
}
