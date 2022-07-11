using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionSceneCamera : MonoBehaviour
{
    //Delta is the amount of seconds since the last frame
    //Serialize field so we can see and change these values in the inspector
    //float speed for how fast our platform moves between point A and B

    TransitionSceneCamera cam;

    float Delta;
    float Alpha;
    float rotAroundZAxis;

    Vector3 direction;

    [SerializeField] Transform StartingPoint;
    [SerializeField] Transform ScenePoint;
    [SerializeField] float speed;

    // Start is called before the first frame update
    void Start()
    {
        Delta = 0.0f;//our delta will be going from 0f to 1f
        Alpha = 0.0f;
        /*direction = StartingPoint.position - ScenePoint.position;*/
        rotAroundZAxis = -direction.z * 180;
    }
    
    // Update is called once per frame
    void Update()
    {
        //code to make our platform move back and forth from point A to B
        /*transform.position = (1+Beta) * StartingPoint.position + Beta * ScenePoint.position;*/
        Delta = Mathf.Cos(Time.time * speed) * 0.75f + 0.75f;
        transform.position = Vector3.Lerp(transform.position, ScenePoint.transform.position, Time.deltaTime * Delta);
        
         /* transform.rotation = Quaternion.Lerp(StartingPoint.rotation, ScenePoint.rotation, Alpha * Time.deltaTime);
         Alpha = Alpha * Time.deltaTime;*/


        /*cam.transform.Rotate(new Vector3(0, 0, 25), rotAroundZAxis);*/
    }
}


