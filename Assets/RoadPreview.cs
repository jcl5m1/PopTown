using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPreview : MonoBehaviour
{
    public GameObject straightPrefab;
    public GameObject turnPrefab;

    public int  x1;
    public int  z1;
    public int  x2;
    public int  z2;

    private float previewHeight = 0.01f;
    public ArrayList path = new ArrayList();
    public GridRoads gridRoad;
    private int[] initialDirection = null;
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Clear()
    {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }
        path.Clear();
        initialDirection = null;
    }

    public void UpdateDestination(int x, int z) 
    {
        this.x2 = x;
        this.z2 = z;
        if(x1==x2 && z1==z2) 
        {
            initialDirection = null;
        }
        else
        {
            //capture the initial drag direction to bias routing preference
            if(initialDirection == null) {
                initialDirection = new int[]{x1-x2,z1-z2};
            }
        }
        path.Clear();
        Generate();
    }

    public void Generate() 
    {
        foreach (Transform child in transform) {
            GameObject.Destroy(child.gameObject);
        }

        bool xFirst = false;
        if(initialDirection != null)
            xFirst = Mathf.Abs(initialDirection[0])>Mathf.Abs(initialDirection[1]);

        if (xFirst)
            {
            // dx is smaller, do dx first
            if(x1 < x2) {
                for(int x = x1; x < x2; x++) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x, previewHeight, z1), Quaternion.Euler(90,90,0));
                    obj.transform.parent = transform;
                    path.Add(new int[]{x,z1});
                }

                // add right or left turn
                if(z1 < z2){
                    //Debug.Log("a");
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x2, previewHeight, z1), Quaternion.Euler(90,0,0));
                    obj2.transform.localScale = new Vector3(1,-1,1);
                    obj2.transform.parent = transform;
                    path.Add(new int[]{x2,z1});
                } else {
                    //Debug.Log("b");
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x2, previewHeight, z1), Quaternion.Euler(90,0,0));
                    obj2.transform.parent = transform;
                    path.Add(new int[]{x2,z1});
                }
            }

            if(x1 > x2) {
                for(int x = x1; x > x2; x--) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x, previewHeight, z1), Quaternion.Euler(90,90,0));
                    obj.transform.parent = transform;
                    path.Add(new int[]{x,z1});
                }
                // add right or left turn
                if(z1 > z2){
                    //Debug.Log("c");
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x2, previewHeight, z1), Quaternion.Euler(90,0,0));
                    obj2.transform.localScale = new Vector3(-1,1,1);
                    obj2.transform.parent = transform;
                    path.Add(new int[]{x2,z1});
                } else {
                    //Debug.Log("d");
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x2, previewHeight, z1), Quaternion.Euler(90,0,0));
                    obj2.transform.localScale = new Vector3(-1,-1,1);
                    obj2.transform.parent = transform;
                    path.Add(new int[]{x2,z1});
                }
            }

            // no turn, add straight section
            if(x1 == x2) {
                GameObject obj = Instantiate(straightPrefab, new Vector3(x2, previewHeight, z1), Quaternion.Euler(90,0,0));
                obj.transform.parent = transform;
                path.Add(new int[]{x2,z1});
            }

            // do dx
            if(z1 < z2) {
                for(int z = z1+1; z <= z2; z++) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x2, previewHeight, z), Quaternion.Euler(90,0,0));
                    obj.transform.parent = transform;
                    path.Add(new int[]{x2,z});
                }
            }
            if(z1 > z2) {
                for(int z = z1-1; z >= z2; z--) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x2, previewHeight, z), Quaternion.Euler(90,0,0));
                    obj.transform.parent = transform;
                    path.Add(new int[]{x2,z});
                }
            }
        }else{
            //if no movement add first point
            //path.Add(new int[]{x1,z1});
            // dz is smaller, do dz first
            if(z1 < z2) {
                for(int z = z1; z < z2; z++) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x1, previewHeight, z), Quaternion.Euler(90,0,0));
                    obj.transform.parent = transform;
                    path.Add(new int[]{x1,z});
                }

                // add right or left turn
                if(x1 < x2){
                    //Debug.Log("e");
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x1, previewHeight, z2), Quaternion.Euler(90,0,0));
                    obj2.transform.localScale = new Vector3(-1,1,1);
                    obj2.transform.parent = transform;
                    path.Add(new int[]{x1,z2});
                } else {
                    //Debug.Log("f");
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x1, previewHeight, z2), Quaternion.Euler(90,0,0));
                    obj2.transform.parent = transform;
                    path.Add(new int[]{x1,z2});
                }
            }

            if(z1 > z2) {
                for(int z = z1; z > z2; z--) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x1, previewHeight, z), Quaternion.Euler(90,0,0));
                    obj.transform.parent = transform;
                    path.Add(new int[]{x1,z});
                }
                // add right or left turn
                if(x1 > x2){
                    //Debug.Log("g");
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x1, previewHeight, z2), Quaternion.Euler(90,0,0));
                    obj2.transform.localScale = new Vector3(1,-1,1);
                    obj2.transform.parent = transform;
                    path.Add(new int[]{x1,z2});
                } else {
                    //Debug.Log("h");
                    GameObject obj2 = Instantiate(turnPrefab, new Vector3(x1, previewHeight, z2), Quaternion.Euler(90,0,0));
                    obj2.transform.localScale = new Vector3(-1,-1,1);
                    obj2.transform.parent = transform;
                    path.Add(new int[]{x1,z2});
                }
            }

            // no turn, add straight section
            if(z1 == z2) {
                GameObject obj = Instantiate(straightPrefab, new Vector3(x1, previewHeight, z2), Quaternion.Euler(90,90,0));
                obj.transform.parent = transform;
                path.Add(new int[]{x1,z2});

            }

            // do dx
            if(x1 < x2) {
                for(int x = x1+1; x <= x2; x++) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x, previewHeight, z2), Quaternion.Euler(90,90,0));
                    obj.transform.parent = transform;
                    path.Add(new int[]{x,z2});
                }
            }
            if(x1 > x2) {
                for(int x = x1-1; x >= x2; x--) {
                    GameObject obj = Instantiate(straightPrefab, new Vector3(x, previewHeight, z2), Quaternion.Euler(90,90,0));
                    obj.transform.parent = transform;
                    path.Add(new int[]{x,z2});
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
