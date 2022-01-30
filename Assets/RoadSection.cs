using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util {
    public static void DebugDrawX(Vector3 pt, float radius, Color color)
    {
        Vector3 offset1 = new Vector3(radius, 0, radius);
        Vector3 offset2 = new Vector3(-radius, 0, radius);
        Debug.DrawLine(pt-offset1, pt+offset1, color);
        Debug.DrawLine(pt-offset2, pt+offset2, color);
    }

    public static void DebugDrawArrow(Vector3 pt, Quaternion rotation, float radius, bool forward, Color color)
    {
        if(forward)
        {
            Vector3 pt2 =  pt + rotation*(new Vector3(0, radius, 0));
            Vector3 offset1 = new Vector3(radius, radius, 0);
            Vector3 offset2 = new Vector3(-radius, radius,0 );
            Debug.DrawLine(pt2-rotation*offset1, pt2, color);
            Debug.DrawLine(pt2-rotation*offset2, pt2, color);
        } 
        else 
        {
            Vector3 offset1 = new Vector3(radius, radius, 0);
            Vector3 offset2 = new Vector3(-radius, radius, 0);
            Debug.DrawLine(pt+rotation*offset1, pt, color);
            Debug.DrawLine(pt+rotation*offset2, pt, color);
        }
    }
}

public class RoadNode
{
    //nodes can connect to other nodes, and contain a collection of lanes/edges
    public Vector3 position;
    public Quaternion rotation;
    public ArrayList edges = new ArrayList();
    public RoadInterface parent;
    public int value;
    public RoadNode(Vector3 position, Quaternion rotation, RoadInterface parent)
    {
        this.position = position;
        this.rotation = rotation;
        this.parent = parent;
        value = 0;
    }

    public void DebugDraw(Quaternion rotation, Color color, bool forward)
    {
        float arrowSize = 0.1f;
        Util.DebugDrawArrow(this.position, rotation, arrowSize, forward, color);
        //over drawing at the moment, every node draws every connected edge
        foreach(RoadEdge edge in edges)
        {
            edge.DebugDraw(Color.white);
        }
    }
}

public abstract class RoadEdge 
{
    //connect two Nodes, had a geometric path, length, and speed
    public RoadNode start;
    public RoadNode end;

    public float length = -1;
    public Vector3[] points = null;
    public abstract Vector3[] GetPoints();
    public abstract void Generate(RoadNode start, RoadNode end);
    
    public void DebugDraw(Color color)
    {
        Vector3[] points = GetPoints();
        for(int i = 0; i < points.Length-1;i++) 
        {
            Debug.DrawLine(
                points[i],
                points[i+1],
                color
                );
        }
    }
}

public class RoadEdgeStraight : RoadEdge 
{
    public override void Generate(RoadNode start, RoadNode end) 
    {
        this.start = start;
        this.end = end;
        GetPoints();
    }
    public override Vector3[] GetPoints() 
    {
        if(this.points != null)
            return this.points;
        this.points = new Vector3[2];
        this.points[0] = start.position;
        this.points[1] = end.position;
        length = 1;
        return this.points;
    }
} 

public class RoadEdgeTurn : RoadEdge 
{
    public Vector3 startHandle;
    public Vector3 endHandle;

    public override Vector3[] GetPoints() 
    {
        int steps = 4;
        if(this.points != null)
            return this.points;
        float step = 1.0f/steps;
        this.points = new Vector3[steps+1];
        length = 0;
        for(int i = 0; i <= steps; i++) {
            float t = i*step;
            Vector3 ab = Vector3.Lerp(start.position, startHandle, t);
            Vector3 cd = Vector3.Lerp(endHandle, end.position, t);
            this.points[i] = Vector3.Lerp(ab,cd,t);
            if(i > 0)
                length += (this.points[i] - this.points[i-1]).magnitude;
        }
        return this.points;
    }    

    public override void Generate(RoadNode start, RoadNode end) 
    {
        this.start = start;
        this.end = end;
        GetPoints();
    }
} 


public class RoadInterface 
{
    //configuration of Nodes that can connect to each other
    public RoadNode node1;
    public RoadNode node2;    
    public Vector3 position;
    public Quaternion rotation;
    public RoadInterface(Vector3 position, Quaternion rotation, float laneOffset)
    {
        this.position = position;
        this.rotation = rotation;
        Vector3 offsetDir = new Vector3(laneOffset, 0,0);

        node1 = new RoadNode(position+rotation*offsetDir, rotation, this);
        node2 = new RoadNode(position-rotation*offsetDir, rotation, this);    
    }

    public void DebugDraw()
    {
        node1.DebugDraw(rotation, Color.green, true);
        node2.DebugDraw(rotation, Color.red, false);
    }
}

public class RoadSection : MonoBehaviour
{
    public enum Type {Straight, Turn, Tee, Fourway};

    public Type type;

    public bool debugDraw = false;

    private ArrayList interfaces = new ArrayList();
    // Start is called before the first frame update
    void Start()
    {
        InitRoads();
    }

    public RoadInterface GetMatchingInterface(RoadSection rs)
    {
        foreach(RoadInterface ri1 in rs.interfaces)
        {
            foreach(RoadInterface ri2 in interfaces)
            {
                float dist = (ri1.position - ri2.position).magnitude;
                if(dist < 0.1) //just distance check?
                    return ri2;
            }
        }
        return null;
    }

    public void InitRoads() 
    {
        if(type == Type.Straight) {
            //create a straight section of road with 2 interfaces along +/-Z
            Vector3 interfaceOffset = new Vector3(0,0.5f,0.0f);
            interfaces.Add(new RoadInterface(transform.position - transform.rotation*interfaceOffset,transform.rotation, 0.25f));
            interfaces.Add(new RoadInterface(transform.position + transform.rotation*interfaceOffset,transform.rotation*Quaternion.Euler(0,0,180), 0.25f));

            RoadInterface ri1 = (RoadInterface)interfaces[0];
            RoadInterface ri2 = (RoadInterface)interfaces[1];

            RoadEdgeStraight edge1 = new RoadEdgeStraight();
            RoadEdgeStraight edge2 = new RoadEdgeStraight();

            edge1.Generate(ri1.node1, ri2.node2);
            ri1.node1.edges.Add(edge1);
            ri2.node2.edges.Add(edge1);

            edge2.Generate(ri2.node1, ri1.node2);
            ri2.node1.edges.Add(edge2);
            ri1.node2.edges.Add(edge2);
        }

        if(type == Type.Turn) {
            //create a left hand turn section of road with 2 bezier curves
            interfaces.Add(new RoadInterface(transform.position - transform.rotation*(new Vector3(0,0.5f,0.0f)),transform.rotation, 0.25f));
            interfaces.Add(new RoadInterface(transform.position - transform.rotation*(new Vector3(0.5f,0,0)),transform.rotation*Quaternion.Euler(0,0,-90), 0.25f));

            RoadInterface ri1 = (RoadInterface)interfaces[0];
            RoadInterface ri2 = (RoadInterface)interfaces[1];

            RoadEdgeTurn edge1 = new RoadEdgeTurn();
            RoadEdgeTurn edge2 = new RoadEdgeTurn();

            float handleLength=0.85f;
            edge1.startHandle = ri1.node1.position + ri1.rotation*(new Vector3(0,handleLength,0));
            edge1.endHandle   = ri2.node2.position + ri2.rotation*(new Vector3(0,handleLength,0));
            edge1.Generate(ri1.node1, ri2.node2);
            ri1.node1.edges.Add(edge1);
            ri2.node2.edges.Add(edge1);

            handleLength=0.3f;
            edge2.startHandle = ri2.node1.position + ri2.rotation*(new Vector3(0,handleLength,0));
            edge2.endHandle   = ri1.node2.position + ri1.rotation*(new Vector3(0,handleLength,0));
            edge2.Generate(ri2.node1, ri1.node2);
            ri2.node1.edges.Add(edge2);
            ri1.node2.edges.Add(edge2);
        }


        if(type == Type.Tee) {
            //create a left hand turn section of road with 2 bezier curves
            interfaces.Add(new RoadInterface(transform.position - transform.rotation*(new Vector3(0,0.5f,0.0f)),transform.rotation, 0.25f));
            interfaces.Add(new RoadInterface(transform.position - transform.rotation*(new Vector3(0.5f,0,0)),transform.rotation*Quaternion.Euler(0,0,-90), 0.25f));
            interfaces.Add(new RoadInterface(transform.position + transform.rotation*(new Vector3(0,0.5f,0.0f)),transform.rotation*Quaternion.Euler(0,0,180), 0.25f));

            RoadInterface ri1 = (RoadInterface)interfaces[0];
            RoadInterface ri2 = (RoadInterface)interfaces[1];
            RoadInterface ri3 = (RoadInterface)interfaces[2];

            RoadEdgeTurn edge1 = new RoadEdgeTurn();
            RoadEdgeTurn edge2 = new RoadEdgeTurn();
            RoadEdgeTurn edge3 = new RoadEdgeTurn();
            RoadEdgeTurn edge4 = new RoadEdgeTurn();

            float handleLength=0.85f;
            edge1.startHandle = ri1.node1.position + ri1.rotation*(new Vector3(0,handleLength,0));
            edge1.endHandle   = ri2.node2.position + ri2.rotation*(new Vector3(0,handleLength,0));
            edge1.Generate(ri1.node1, ri2.node2);
            ri1.node1.edges.Add(edge1);
            ri2.node2.edges.Add(edge1);

            handleLength=0.3f;
            edge2.startHandle = ri2.node1.position + ri2.rotation*(new Vector3(0,handleLength,0));
            edge2.endHandle   = ri1.node2.position + ri1.rotation*(new Vector3(0,handleLength,0));
            edge2.Generate(ri2.node1, ri1.node2);
            ri2.node1.edges.Add(edge2);
            ri1.node2.edges.Add(edge2);

            handleLength=0.85f;
            edge3.startHandle = ri2.node1.position + ri2.rotation*(new Vector3(0,handleLength,0));
            edge3.endHandle   = ri3.node2.position + ri3.rotation*(new Vector3(0,handleLength,0));
            edge3.Generate(ri2.node1, ri3.node2);
            ri2.node1.edges.Add(edge3);
            ri3.node2.edges.Add(edge3);

            handleLength=0.3f;
            edge4.startHandle = ri3.node1.position + ri3.rotation*(new Vector3(0,handleLength,0));
            edge4.endHandle   = ri2.node2.position + ri2.rotation*(new Vector3(0,handleLength,0));
            edge4.Generate(ri3.node1, ri2.node2);
            ri3.node1.edges.Add(edge4);
            ri2.node2.edges.Add(edge4);

            RoadEdgeStraight edge5 = new RoadEdgeStraight();
            RoadEdgeStraight edge6 = new RoadEdgeStraight();

            edge5.Generate(ri1.node1, ri3.node2);
            ri1.node1.edges.Add(edge5);
            ri3.node2.edges.Add(edge5);

            edge6.Generate(ri3.node1, ri1.node2);
            ri3.node1.edges.Add(edge6);
            ri1.node2.edges.Add(edge6);
        }

        if(type == Type.Fourway) {
            //create a left hand turn section of road with 2 bezier curves
            interfaces.Add(new RoadInterface(transform.position - transform.rotation*(new Vector3(0,0.5f,0.0f)),transform.rotation, 0.25f));
            interfaces.Add(new RoadInterface(transform.position - transform.rotation*(new Vector3(0.5f,0,0)),transform.rotation*Quaternion.Euler(0,0,-90), 0.25f));
            interfaces.Add(new RoadInterface(transform.position + transform.rotation*(new Vector3(0,0.5f,0.0f)),transform.rotation*Quaternion.Euler(0,0,180), 0.25f));
            interfaces.Add(new RoadInterface(transform.position + transform.rotation*(new Vector3(0.5f,0,0)),transform.rotation*Quaternion.Euler(0,0, 90), 0.25f));

            RoadInterface ri1 = (RoadInterface)interfaces[0];
            RoadInterface ri2 = (RoadInterface)interfaces[1];
            RoadInterface ri3 = (RoadInterface)interfaces[2];
            RoadInterface ri4 = (RoadInterface)interfaces[3];

            RoadEdgeTurn edge1 = new RoadEdgeTurn();
            RoadEdgeTurn edge2 = new RoadEdgeTurn();

            float handleLength=0.85f;
            edge1.startHandle = ri1.node1.position + ri1.rotation*(new Vector3(0,handleLength,0));
            edge1.endHandle   = ri2.node2.position + ri2.rotation*(new Vector3(0,handleLength,0));
            edge1.Generate(ri1.node1, ri2.node2);
            ri1.node1.edges.Add(edge1);
            ri2.node2.edges.Add(edge1);

            handleLength=0.3f;
            edge2.startHandle = ri2.node1.position + ri2.rotation*(new Vector3(0,handleLength,0));
            edge2.endHandle   = ri1.node2.position + ri1.rotation*(new Vector3(0,handleLength,0));
            edge2.Generate(ri2.node1, ri1.node2);
            ri2.node1.edges.Add(edge2);
            ri1.node2.edges.Add(edge2);

            RoadEdgeTurn edge3 = new RoadEdgeTurn();
            RoadEdgeTurn edge4 = new RoadEdgeTurn();

            handleLength=0.85f;
            edge3.startHandle = ri2.node1.position + ri2.rotation*(new Vector3(0,handleLength,0));
            edge3.endHandle   = ri3.node2.position + ri3.rotation*(new Vector3(0,handleLength,0));
            edge3.Generate(ri2.node1, ri3.node2);
            ri2.node1.edges.Add(edge3);
            ri3.node2.edges.Add(edge3);

            handleLength=0.3f;
            edge4.startHandle = ri3.node1.position + ri3.rotation*(new Vector3(0,handleLength,0));
            edge4.endHandle   = ri2.node2.position + ri2.rotation*(new Vector3(0,handleLength,0));
            edge4.Generate(ri3.node1, ri2.node2);
            ri3.node1.edges.Add(edge4);
            ri2.node2.edges.Add(edge4);

            RoadEdgeStraight edge5 = new RoadEdgeStraight();
            RoadEdgeStraight edge6 = new RoadEdgeStraight();

            edge5.Generate(ri1.node1, ri3.node2);
            ri1.node1.edges.Add(edge5);
            ri3.node2.edges.Add(edge5);

            edge6.Generate(ri3.node1, ri1.node2);
            ri3.node1.edges.Add(edge6);
            ri1.node2.edges.Add(edge6);

            RoadEdgeTurn edge7 = new RoadEdgeTurn();
            RoadEdgeTurn edge8 = new RoadEdgeTurn();

            handleLength=0.3f;
            edge7.startHandle = ri4.node1.position + ri4.rotation*(new Vector3(0,handleLength,0));
            edge7.endHandle   = ri3.node2.position + ri3.rotation*(new Vector3(0,handleLength,0));
            edge7.Generate(ri4.node1, ri3.node2);
            ri4.node1.edges.Add(edge7);
            ri3.node2.edges.Add(edge7);

            handleLength=0.85f;
            edge8.startHandle = ri3.node1.position + ri3.rotation*(new Vector3(0,handleLength,0));
            edge8.endHandle   = ri4.node2.position + ri4.rotation*(new Vector3(0,handleLength,0));
            edge8.Generate(ri3.node1, ri4.node2);
            ri3.node1.edges.Add(edge8);
            ri4.node2.edges.Add(edge8);

            RoadEdgeStraight edge9 = new RoadEdgeStraight();
            RoadEdgeStraight edge10 = new RoadEdgeStraight();

            edge9.Generate(ri2.node1, ri4.node2);
            ri2.node1.edges.Add(edge9);
            ri4.node2.edges.Add(edge9);

            edge10.Generate(ri4.node1, ri2.node2);
            ri4.node1.edges.Add(edge10);
            ri2.node2.edges.Add(edge10);

            RoadEdgeTurn edge11 = new RoadEdgeTurn();
            RoadEdgeTurn edge12 = new RoadEdgeTurn();

            handleLength=0.85f;
            edge11.startHandle = ri4.node1.position + ri4.rotation*(new Vector3(0,handleLength,0));
            edge11.endHandle   = ri1.node2.position + ri1.rotation*(new Vector3(0,handleLength,0));
            edge11.Generate(ri4.node1, ri1.node2);
            ri4.node1.edges.Add(edge11);
            ri1.node2.edges.Add(edge11);

            handleLength=0.3f;
            edge12.startHandle = ri1.node1.position + ri1.rotation*(new Vector3(0,handleLength,0));
            edge12.endHandle   = ri4.node2.position + ri4.rotation*(new Vector3(0,handleLength,0));
            edge12.Generate(ri1.node1, ri4.node2);
            ri1.node1.edges.Add(edge12);
            ri4.node2.edges.Add(edge12);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // if(debugDraw)
        // {
        //     foreach(RoadInterface ri in interfaces)
        //         ri.DebugDraw();
        // }
    }
}
