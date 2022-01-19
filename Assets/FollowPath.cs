using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{

    public ArrayList path;
    public float speed;
    public float startTime;
    public int pathIndex;

    public bool forward = true;

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
            forward = (int)((speed*Time.realtimeSinceStartup-startTime)/(path.Count-1))%2==0;
            if(forward)
            {
                float dt = (speed*Time.realtimeSinceStartup-startTime)%(path.Count-1);
                int idx1 = (int)Mathf.Floor(dt);
                int idx2 = (int)Mathf.Ceil(dt);
                float y = transform.position.y;
                int[] c1 = (int[])path[idx1];
                int[] c2 = (int[])path[idx2];
                transform.position = Vector3.Lerp(new Vector3(c1[0],y,c1[1]),new Vector3(c2[0],y,c2[1]),dt-idx1);
            }
            else
            {
                float dt = (path.Count-1) - (speed*Time.realtimeSinceStartup-startTime)%(path.Count-1);
                int idx1 = (int)Mathf.Floor(dt);
                int idx2 = (int)Mathf.Ceil(dt);
                float y = transform.position.y;
                int[] c1 = (int[])path[idx1];
                int[] c2 = (int[])path[idx2];
                transform.position = Vector3.Lerp(new Vector3(c1[0],y,c1[1]),new Vector3(c2[0],y,c2[1]),(dt-idx1));
            }
        }
    }
}
