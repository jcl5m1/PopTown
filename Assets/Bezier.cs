using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier 
{
    public Vector3 start;
    public Vector3 end;
    public Vector3 startHandle;
    public Vector3 endHandle;

    public ArrayList points = null;
    private float length = -1;
    public Bezier(Vector3 start, Vector3 end, Vector3 startHandle, Vector3 endHandle)
    {
        this.start = start;
        this.end = end;
        this.startHandle = startHandle;
        this.endHandle = endHandle;
    }
    public ArrayList GetPoints(int steps) 
    {
        if(this.points == null)
        {
            float step = 1.0f/steps;
            this.points = new ArrayList();
            for(int i = 0; i <= steps; i++) {
                float t = i*step;
                Vector3 ab = Vector3.Lerp(start, startHandle, t);
                Vector3 cd = Vector3.Lerp(endHandle, end, t);
                points.Add(Vector3.Lerp(ab,cd,t));
            }
        }
        return this.points;
    }    

    public float Length() {
        if(length >= 0)
            return length;

        ArrayList pts = GetPoints(5);
        this.length = 0;
        for(int i =0; i < pts.Count-1; i++) {
            this.length += ((Vector3)points[i] - (Vector3)points[i+1]).magnitude;
        }
        return this.length;
    }

    public void DebugDrawX(Vector3 pt, float radius, Color color)
    {
        Vector3 offset1 = new Vector3(radius, 0, radius);
        Vector3 offset2 = new Vector3(-radius, 0, radius);
        Debug.DrawLine(pt-offset1, pt+offset1, color);
        Debug.DrawLine(pt-offset2, pt+offset2, color);
    }

    public void DebugDraw(int steps)
    {
        float y = 0.05f;
        DebugDrawX(startHandle, y, Color.red);
        Debug.DrawLine(start, startHandle, Color.red);

        DebugDrawX(endHandle, y, Color.green);
        Debug.DrawLine(end, endHandle, Color.green);
        ArrayList points = GetPoints(steps);
        for(int i =0; i < points.Count-1; i++) {
            Debug.DrawLine((Vector3)points[i], (Vector3)points[i+1]);
        }
    }
}