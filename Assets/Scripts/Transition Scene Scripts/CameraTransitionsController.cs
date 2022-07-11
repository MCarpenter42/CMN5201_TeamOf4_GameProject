using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTransitionsController : MonoBehaviour
{
    private Vector3 firstPosition;
    private Vector3 secondPosition = new Vector3(0.6f, 31.3f, -13.4f);
    private Vector3 thirdPosition = new Vector3(35.05f, 31.3f, -13.4f);

    private float transitionDuration = 8.0f;
    private float elapsedTime;

    void Start()
    {
        firstPosition = transform.position;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        float percentageComplete = elapsedTime / transitionDuration;
        
        transform.position = Vector3.Lerp(firstPosition, secondPosition, percentageComplete);
        transform.position = Vector3.Lerp(transform.position, thirdPosition, Mathf.SmoothStep(0.0f, 1.0f, percentageComplete));
    }
}
