using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ArrowEngine : MonoBehaviour
{
    Transform arrowPos;
    public GameObject arrowPrefab;
    Transform playerPos;
   Vector3 arrowGravity = new Vector3 (0, -6f, 0);
    // Start is called before the first frame update
    void Start()
    {
        playerPos = GameObject.Find("Player").transform;
        arrowPos = GameObject.Find("ArrowPos").transform;
        transform.localRotation = Quaternion.Euler(90, 0,0);
        Physics.gravity = arrowGravity;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, 0, 100 * Time.deltaTime, playerPos);  
    }
   
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Bear")
        {
            BearMovement bearMovement = collision.gameObject.GetComponent<BearMovement>();
            bearMovement.currentHealth -= 10;
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Wolf")
        {
            WolfMovement wolfMovement = collision.gameObject.GetComponent<WolfMovement>();
            wolfMovement.currentHealth -= 20;
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Tiger")
        {
            TigerMovement tigerMovement = collision.gameObject.GetComponent<TigerMovement>();
            tigerMovement.currentHealth -= 20;
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Rabbit")
        {
            RabbitMovement rabbitMovement = collision.gameObject.GetComponent<RabbitMovement>();
            rabbitMovement.currentHealth -= 50;
            Destroy(gameObject);
        }
    }
        
    
}
