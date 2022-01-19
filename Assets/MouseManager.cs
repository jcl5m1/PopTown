using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public GameObject cursor;
    public GridRoads gridRoads;
    private int[] currPos = new int[]{0,0};

    private int[] prevPos = new int[]{0,0};
    private bool addingRoad = false;
    private bool wasDown = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit)) {

            Vector3 position = new Vector3();
            position.x = (int)Mathf.Round(hit.point.x);
            position.z = (int)Mathf.Round(hit.point.z);

            cursor.transform.position = position;  
            
            prevPos[0] = currPos[0];
            prevPos[1] = currPos[1];

            currPos[0] = (int)(cursor.transform.position.x);
            currPos[1] = (int)(cursor.transform.position.z);


            // Do something with the object that was hit by the raycast.
        }

        if (Input.GetMouseButton(0)) {

            if(!wasDown)
            {
                int type = gridRoads.Get(currPos[0], currPos[1]);
                addingRoad=(type == 0);
            }
            if(addingRoad) {
                gridRoads.Set(currPos[0], currPos[1],1);
            }else {
                gridRoads.Set(currPos[0], currPos[1],0);
            }
            wasDown = true;
        }else {
            wasDown = false;
        }

        if (Input.GetKeyDown("space"))
            gridRoads.FindPath(0,5,0,-2);
        if (Input.GetKeyDown("r"))
            gridRoads.ResetGrid();
    }
}

