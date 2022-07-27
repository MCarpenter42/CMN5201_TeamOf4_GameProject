using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TriggerUI : MonoBehaviour
{
    public GameObject text;
    public GameObject Jewel;
    public GameObject Scrolls;
    public GameObject Coins;
    public Transform Table;
    public Transform player;
    // Start is called before the first frame update
    void Start()
    {
        text.SetActive(false);
        
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            text.SetActive(true);
        }
    }




    private void Update()
    {
        float dist = Vector3.Distance(player.position, this.transform.position);
        if (dist >= 1.5f)
        {
            text.SetActive(false);
        }

        if (Input.GetKey(KeyCode.G) && dist <= 1.5f)
        {
            Destroy(this.gameObject);
            Destroy(text);
            Destroy(Coins);
            Destroy(Jewel);
            Destroy(Scrolls);
            Destroy(GameObject.FindWithTag("gamemanager"));
            SceneManager.LoadScene(19);
            
            
        }
        
    }
  
   
}
