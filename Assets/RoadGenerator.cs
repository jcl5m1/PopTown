using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public GameObject straightPrefab;
    public GameObject turnPrefab;
    public GameObject nodePrefab;

    public GameObject nodeA;
    public GameObject nodeB;

    public int  x1;
    public int  z1;
    public int  x2;
    public int  z2;

    private float roadHeight = 0.01f;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Generate() 
    {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
        int dx = x1-x2;
        int dz = z1-z2;

        {
            GameObject obj = Instantiate(nodePrefab, new Vector3(x1, roadHeight, z1), Quaternion.Euler(90,0,0));
            obj.transform.parent = transform;
        }
        {
            GameObject obj = Instantiate(nodePrefab, new Vector3(x2, roadHeight, z2), Quaternion.Euler(90,0,0));
            obj.transform.parent = transform;
        }
        if (Mathf.Abs(dx) < Mathf.Abs(dz))
        {
            // dx is smaller, do dx first
            if(x1 < x2) {

                for(int x = x1+1; x < x2; x++) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x, roadHeight, z1), Quaternion.Euler(90,90,0));
                    obj.transform.parent = transform;
                }

                // add right or left turn
                if(z1 < z2){
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x2, roadHeight, z1), Quaternion.Euler(90,0,0));
                    obj2.transform.localScale = new Vector3(1,-1,1);
                    obj2.transform.parent = transform;
                } else {
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x2, roadHeight, z1), Quaternion.Euler(90,0,0));
                    obj2.transform.parent = transform;
                }
            }

            if(x1 > x2) {
                for(int x = x1-1; x > x2; x--) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x, roadHeight, z1), Quaternion.Euler(90,90,0));
                    obj.transform.parent = transform;
                }
                // add right or left turn
                if(z1 > z2){
                    // Debug.Log("a");
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x2, roadHeight, z1), Quaternion.Euler(90,0,0));
                    obj2.transform.localScale = new Vector3(-1,1,1);
                    obj2.transform.parent = transform;
                } else {
                    // Debug.Log("b");
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x2, roadHeight, z1), Quaternion.Euler(90,0,0));
                    obj2.transform.localScale = new Vector3(-1,-1,1);
                    obj2.transform.parent = transform;
                }
            }

            // no turn, add straight section
            // if(x1 == x2) {
            //     GameObject obj = Instantiate(straightPrefab, new Vector3(x2, roadHeight, z1), Quaternion.Euler(90,0,0));
            //     obj.transform.parent = transform;
            // }

            // do dx
            if(z1 < z2) {
                for(int z = z1+1; z < z2; z++) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x2, roadHeight, z), Quaternion.Euler(90,0,0));
                    obj.transform.parent = transform;
                }
            }
            if(z1 > z2) {
                for(int z = z1-1; z > z2; z--) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x2, roadHeight, z), Quaternion.Euler(90,0,0));
                    obj.transform.parent = transform;
                }
            }
        }else{

            // dz is smaller, do dz first
            if(z1 < z2) {
                for(int z = z1+1; z < z2; z++) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x1, roadHeight, z), Quaternion.Euler(90,0,0));
                    obj.transform.parent = transform;
                }

                // add right or left turn
                if(x1 < x2){
                    // Debug.Log("e");
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x1, roadHeight, z2), Quaternion.Euler(90,0,0));
                    obj2.transform.localScale = new Vector3(-1,1,1);
                    obj2.transform.parent = transform;
                } else {
                    // Debug.Log("f");
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x1, roadHeight, z2), Quaternion.Euler(90,0,0));
                    obj2.transform.parent = transform;
                }
            }

            if(z1 > z2) {
                for(int z = z1-1; z > z2; z--) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x1, roadHeight, z), Quaternion.Euler(90,0,0));
                    obj.transform.parent = transform;
                }
                // add right or left turn
                if(x1 > x2){
                    // Debug.Log("g");
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x1, roadHeight, z2), Quaternion.Euler(90,0,0));
                    obj2.transform.localScale = new Vector3(1,-1,1);
                    obj2.transform.parent = transform;
                } else {
                    // Debug.Log("h");
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x1, roadHeight, z2), Quaternion.Euler(90,0,0));
                    obj2.transform.localScale = new Vector3(-1,-1,1);
                    obj2.transform.parent = transform;
                }
            }

            // no turn, add straight section
            // if(z1 == z2) {
            //     GameObject obj = Instantiate(straightPrefab, new Vector3(x1, roadHeight, z2), Quaternion.Euler(90,90,0));
            //     obj.transform.parent = transform;
            // }

            // do dx
            if(x1 < x2) {
                for(int x = x1+1; x < x2; x++) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x, roadHeight, z2), Quaternion.Euler(90,90,0));
                    obj.transform.parent = transform;
                }
            }
            if(x1 > x2) {
                for(int x = x1-1; x > x2; x--) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x, roadHeight, z2), Quaternion.Euler(90,90,0));
                    obj.transform.parent = transform;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
