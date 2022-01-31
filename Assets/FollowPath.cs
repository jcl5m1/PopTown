using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    public ArrayList forwardPts;
    public ArrayList reversePts;
    public float speed;
    public float startTime;
    public int pathIndex;
    public float wheelBase;
    public bool debug;

    public bool forwardDirection = false;
    public int positionIdx = 0;
    public Vector3 frontAxle = new Vector3();
    public Vector3 rearAxle = new Vector3();

//    private float laneOffset = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        pathIndex = 0;
    }

    public void SetPath(ArrayList edgeList, bool forward) {
        if(forward) 
        {
            this.forwardPts = EdgeSequenceToPointSequence(edgeList,speed/30);
            if(edgeList == null)
            {
                transform.gameObject.GetComponent<Renderer>().enabled = false;
            }
            else 
            {
                SetCarPosition((Vector3)this.forwardPts[0]);
                transform.gameObject.GetComponent<Renderer>().enabled = true;
            }       
        }
        else
        {
            this.reversePts = EdgeSequenceToPointSequence(edgeList,speed/30);
        }

        startTime = Time.realtimeSinceStartup;
        positionIdx = 0;
    }

    public ArrayList EdgeSequenceToPointSequence(ArrayList edgeList, float stepDist)
    {
        if(edgeList == null)
            return null;
        ArrayList edgePoints = new ArrayList();
        foreach(RoadEdge e in edgeList)
        {
            Vector3[] points = e.GetPoints();
            foreach(Vector3 p in points)
            {
                edgePoints.Add(p);
            }
        }

        //resample using stepDist
        ArrayList stepPoints = new ArrayList();
        Vector3 prevPt = Vector3.zero;
        Vector3 currPt = Vector3.zero;
        foreach(Vector3 nextPt in edgePoints)
        {
            //first point
            if(prevPt == Vector3.zero) 
            {
                stepPoints.Add(nextPt);
                prevPt = nextPt;
                continue;
            }
            float nextDist = (prevPt - nextPt).magnitude;
            //too close, proceed to next point
            if(nextDist < stepDist)
                continue;
            
            //too far, interpolate
            while(nextDist >= stepDist)
            {
                Vector3 tempPt = Vector3.Lerp(prevPt, nextPt, stepDist/nextDist);
                stepPoints.Add(tempPt);
                prevPt = tempPt;
                nextDist = (prevPt - nextPt).magnitude;
            }
        }

        return stepPoints;
    }

    void SetCarPosition(Vector3 pt) {
        frontAxle = pt;
        Vector3 vehicleDir = rearAxle - frontAxle;
        rearAxle = frontAxle + wheelBase*vehicleDir/vehicleDir.magnitude;

        //position object to align with axle positions
        transform.position = (frontAxle+rearAxle)/2;
        transform.rotation = Quaternion.Euler(0,Mathf.Atan2(vehicleDir.x,vehicleDir.z)*Mathf.Rad2Deg,0);
    }

    // Update is called once per frame
    void Update()
    {
        if((forwardPts == null)||(reversePts == null))
        {
            return;
        }

        if((forwardPts.Count == 0)||(reversePts.Count == 0))
        {
            return;
        }

        if(debug)
        {
            foreach(Vector3 p in forwardPts)
                Util.DebugDrawX(p,0.1f,Color.green);
            foreach(Vector3 p in reversePts)
                Util.DebugDrawX(p,0.1f,Color.red);
        }

        if(forwardDirection)
        {
            SetCarPosition((Vector3)forwardPts[positionIdx]);
            positionIdx += 1;
            if(positionIdx >= forwardPts.Count)
            {
                positionIdx = 0;
                forwardDirection = false;
            }
        }
        else
        {
            SetCarPosition((Vector3)reversePts[positionIdx]);
            positionIdx += 1;
            if(positionIdx >= reversePts.Count)
            {
                positionIdx = 0;
                forwardDirection = true;
            }
        }

    }
}
