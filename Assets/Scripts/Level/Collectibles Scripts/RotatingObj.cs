using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObj : MonoBehaviour
{
    [SerializeField]
    private float degreesPerSec;

    void Update()
    {
        transform.Rotate(new Vector3(1.0f, 3 * (Time.deltaTime * degreesPerSec), 1.5f), Space.World);
    }
}
