
using UnityEngine;

public class dayandnight : MonoBehaviour
{
  Vector3 rot=Vector3.zero;
    float degpersec = 0.5f;
    // Update is called once per frame
    void Update()
    {
        rot.x = degpersec * Time.deltaTime;
        transform.Rotate(rot, Space.World);
    }
}
