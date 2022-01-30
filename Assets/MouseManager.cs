using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public GameObject cursor;
    public GridRoads gridRoads;
    public RoadPreview roadPreview;
    private int[] currPos = new int[]{0,0};

    private int[] prevPos = new int[]{0,0};
    private bool wasDown = false;
    private bool addingRoad = false;
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
                if(type == 0) {
                    addingRoad = true;
                    roadPreview.x1 = currPos[0];
                    roadPreview.z1 = currPos[1];
                    roadPreview.UpdateDestination(currPos[0], currPos[1]);
                }
                else
                {
                    addingRoad = false;
                }
            }
            if(addingRoad) {
                if(prevPos[0] != currPos[0] || prevPos[1] != currPos[1]) {
                    roadPreview.UpdateDestination(currPos[0], currPos[1]);
                }
            }else {
                gridRoads.Set(currPos[0], currPos[1],0);
            }
            wasDown = true;
        }else {
            if(wasDown) {
                gridRoads.SetPath(roadPreview.path);
                roadPreview.Clear();
            }
            prevPos[0]=10000;
            prevPos[1]=10000;
            wasDown = false;
        }

        if (Input.GetKeyDown("space"))
        {
            gridRoads.FindPath(0,5,0,0);
        }
        if (Input.GetKeyDown("r"))
            gridRoads.ResetGrid();
    }
}

