using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{

    public GameObject start;
    public GameObject end;

    // Start is called before the first frame update
    void Start()
    {
        // transform.position = start.transform;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        position.z = Mathf.Lerp(start.transform.position.z, end.transform.position.z, 0.5f+0.5f*Mathf.Sin(Time.realtimeSinceStartup));
        transform.position = position;
    }
}
