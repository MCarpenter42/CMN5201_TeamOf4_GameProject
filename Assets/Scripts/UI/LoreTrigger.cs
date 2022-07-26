using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoreTrigger : MonoBehaviour
{
    public GameObject text;
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        text.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(player.position, this.transform.position);
        if (dist >= 1.5f)
        {
            text.SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            text.SetActive(true);
        }
    }
}
