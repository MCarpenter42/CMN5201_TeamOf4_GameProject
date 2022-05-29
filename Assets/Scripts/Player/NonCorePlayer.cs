using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonCorePlayer : MonoBehaviour
{
    /*My attempt at the core mechanics, i dont think these mechanics are up to scratch so they will not be placed in the core player script.you are free to take this code
    and use it or improve on it or scrap it later on*/
    Rigidbody rb;
    [SerializeField] float speed;
    [SerializeField] float Rotation = 0f;
    [SerializeField] GameObject boulder;
    [SerializeField] GameObject boulder2;
    private Vector3 offset = new Vector3(0, 0, 1);
    private Vector3 offset2 = new Vector3(0, 0, -1);
    private Vector3 offset3 = new Vector3(-1, 0, 0);
    private Vector3 offset4 = new Vector3(1, 0, 0);


    //The player uses rigidbody instead of transform as i could not make progress and got bugs using no physics.
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerMove();
        PickingUp();
        Rotating();
    }
    void PlayerMove()
    {
        if (Input.GetKey(KeyCode.UpArrow) || (Input.GetKey(KeyCode.W)))
        {
            rb.velocity = new Vector3(0, 0, 1) * speed;
        }
        if (Input.GetKey(KeyCode.DownArrow) || (Input.GetKey(KeyCode.S)))
        {
            rb.velocity = new Vector3(0, 0, -1) * speed;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || (Input.GetKey(KeyCode.A)))
        {
            rb.velocity = new Vector3(-1, 0, 0) * speed;
        }
        if (Input.GetKey(KeyCode.RightArrow) || (Input.GetKey(KeyCode.D)))
        {
            rb.velocity = new Vector3(1, 0, 0) * speed;
        }
    }
    void PickingUp()
    {
        //intended for this to be pushing and dragging but turned into a picking up function, hold space while pressing a directional arrow to move the mirrors around
        float distance = Vector3.Distance(transform.position, boulder.transform.position);
        float distance2 = Vector3.Distance(transform.position, boulder2.transform.position);

        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKey(KeyCode.Space))
        {

            if (distance < 1.5f)
            {
                boulder.transform.position = transform.position + offset;


            }
            else if (distance2 < 1.5f)
            {
                boulder2.transform.position = transform.position + offset;
            }

        }




        if (Input.GetKey(KeyCode.DownArrow) && Input.GetKey(KeyCode.Space))
        {
            if (distance < 1.5f)
            {
                boulder.transform.position = transform.position + offset2;
            }
            else if (distance2 < 1.5f)
            {
                boulder2.transform.position = transform.position + offset2;
            }

        }
        if (Input.GetKey(KeyCode.LeftArrow) && Input.GetKey(KeyCode.Space))
        {
            if (distance < 1.5f)
            {

                boulder.transform.position = transform.position + offset3;
            }
            else if (distance2 < 1.5f)
            {
                boulder2.transform.position = transform.position + offset3;
            }
        }




        if (Input.GetKey(KeyCode.RightArrow) && Input.GetKey(KeyCode.Space))
        {
            if (distance < 1.5f)
            {
                boulder.transform.position = transform.position + offset4;

            }
            else if (distance2 < 1.5f)
            {
                boulder2.transform.position = transform.position + offset4;
            }



        }

    }
    void Rotating()
    {
        //My Attempt at rotating a object, Hold q while near a object to rotate it.
        float distance = Vector3.Distance(transform.position, boulder.transform.position);
        float distance2 = Vector3.Distance(transform.position, boulder2.transform.position);
        if (Input.GetKey(KeyCode.Q) && !Input.GetKey(KeyCode.Space))
        {
            if (distance < 1.7f)
            {
                boulder.transform.rotation = Quaternion.Euler(0, Rotation++, 0);
            }
            else if (distance2 < 1.7f)
            {
                boulder2.transform.rotation = Quaternion.Euler(0, Rotation++, 0);
            }
        }
    }
}


