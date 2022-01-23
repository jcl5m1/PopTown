using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{

    public ArrayList path;
    public float speed;
    public float startTime;
    public int pathIndex;
    public float wheelBase;

    public bool forwardDirection = false;
    private bool previousDirection = false;
    public Vector3 frontAxle = new Vector3();
    public Vector3 rearAxle = new Vector3();

    private float laneOffset = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        pathIndex = 0;
    }

    public void SetPath(ArrayList path) {
        this.path = path;
        startTime = Time.realtimeSinceStartup;
    }
    // Update is called once per frame
    void Update()
    {
        if(path != null) 
        {
            previousDirection = forwardDirection;
            forwardDirection = (int)((speed*Time.realtimeSinceStartup-startTime)/(path.Count-1))%2==0;
            if(previousDirection != forwardDirection)
            {
                rearAxle = frontAxle + new Vector3(1,0,0);
            }

            if(forwardDirection)
            {
                float dt = (speed*Time.realtimeSinceStartup-startTime)%(path.Count-1);
                int idx1 = (int)Mathf.Floor(dt);
                int idx2 = (int)Mathf.Ceil(dt);
                float y = transform.position.y;
                int[] c1 = (int[])path[idx1];
                int[] c2 = (int[])path[idx2];
                Vector3 a = new Vector3(c1[0],y,c1[1]);
                Vector3 b = new Vector3(c2[0],y,c2[1]);
                Vector3 roadDir = b-a;
                Vector3 offset = laneOffset*(Quaternion.Euler(0, 90, 0) * roadDir);
                
                frontAxle = Vector3.Lerp(a+offset,b+offset,dt-idx1);
                Vector3 dir = rearAxle - frontAxle;
                rearAxle = frontAxle + wheelBase*dir/dir.magnitude;

                transform.position = (frontAxle+rearAxle)/2;
                transform.rotation = Quaternion.Euler(0,Mathf.Atan2(dir.x,dir.z)*Mathf.Rad2Deg,0);
            }
            else
            {
                float dt = (speed*Time.realtimeSinceStartup-startTime)%(path.Count-1);
                int idx1 = (int)Mathf.Floor(dt);
                int idx2 = (int)Mathf.Ceil(dt);
                float y = transform.position.y;
                int[] c1 = (int[])path[path.Count - idx1-1];
                int[] c2 = (int[])path[path.Count - idx2-1];
                Vector3 a = new Vector3(c1[0],y,c1[1]);
                Vector3 b = new Vector3(c2[0],y,c2[1]);
                Vector3 roadDir = b-a;
                Vector3 offset = laneOffset*(Quaternion.Euler(0, 90, 0) * roadDir);
                frontAxle = Vector3.Lerp(a+offset,b+offset,dt-idx1);
                Vector3 vehicleDir = rearAxle - frontAxle;
                rearAxle = frontAxle + wheelBase*vehicleDir/vehicleDir.magnitude;

                transform.position = (frontAxle+rearAxle)/2;
                transform.rotation = Quaternion.Euler(0,  Mathf.Atan2(vehicleDir.x,vehicleDir.z)*Mathf.Rad2Deg,0);
            }
        }
    }
}
