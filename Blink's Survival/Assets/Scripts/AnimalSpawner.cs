using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimalSpawner : MonoBehaviour
{
    public GameObject bearSpawn;
    public int currentBearCount;
    public GameObject wolfSpawn;
    public int currentWolfCount;
    public GameObject rabbitSpawn;
    public int currentRabbitCount;
    public GameObject tigerSpawn;
    public int currentTigerCount;
    public GameObject apple;
    public int currentAppleCount;
    // Start is called before the first frame update
    void Start()
    {
       

    }

    // Update is called once per frame
    void Update()
    {

        if (currentBearCount <= 5)
        {
            for (int i = 0; i < 10; i++)
            {
                Instantiate(bearSpawn, new Vector3(Random.Range(800f, 2000f), 202f, Random.Range(2300f, 2900f)), Quaternion.identity);
                currentBearCount++;


            }
        }
        if (currentWolfCount <= 5)
        {
            for (int i = 0; i < 10; i++)
            {
                Instantiate(wolfSpawn, new Vector3(Random.Range(100f, 800f), 210f, Random.Range(850f, 2000f)), Quaternion.identity);
                currentWolfCount++;

            }
        }
        if (currentTigerCount <= 5)
        {
            for (int i = 0; i < 10; i++)
            {
                Instantiate(tigerSpawn, new Vector3(Random.Range(2200f, 3000f), 210f, Random.Range(400f, 2100f)), Quaternion.identity);
                currentTigerCount++;

            }
        }
        if (currentRabbitCount <= 10)
        {
            for (int i = 0; i < 20; i++)
            {
                Instantiate(rabbitSpawn, new Vector3(Random.Range(100f, 2900f), 210f, Random.Range(400f, 2100f)), Quaternion.identity);
                currentRabbitCount++;

            }
        }
        if (currentAppleCount <= 100)
        {
            for (int i = 0; i < 101; i++)
            {
                 Instantiate(apple, new Vector3(Random.Range(200f, 2900f), 210f, Random.Range(400f, 2900f)), Quaternion.identity);
                currentAppleCount++;
               
            }
        }



    }
   


}

