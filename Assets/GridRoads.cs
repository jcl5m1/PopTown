using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridNode {
    public int ix;
    public int iy;
    public NodeType type;
    public int dist;
    public bool goal;
    public GameObject obj;
    public bool dirty = false;
    public enum NodeType
        {
            Empty,
            Road,
            Building,
            Invalid
        }
    public GridNode(int ix, int iy) {
        this.ix = ix;
        this.iy = iy;
        type = NodeType.Empty;
        dist = -1;
        goal = false;
    }
}

public class GridRoads : MonoBehaviour
{
    public static int radius = 100;
    public GridNode[,] grid = new GridNode[2*radius+1, 2*radius+1];
    // Start is called before the first frame update

    public GameObject fourstopPrefab;
    public GameObject straightPrefab;
    public GameObject turnPrefab;
    public GameObject teePrefab;
    public GameObject fourwayPrefab;
    public GameObject vehicle;
    public float roadHeight = 0.01f;
    private ArrayList AstarQueue = new ArrayList();
    private ArrayList dirtyQueue = new ArrayList();
    private ArrayList prevNodeSequence = new ArrayList();
    private ArrayList nodeSequence = new ArrayList();
    private ArrayList interfaceSequence = new ArrayList();
    private ArrayList forwardEdgeSequence = new ArrayList();
    private ArrayList pointSequence;

    public enum RoadType {StraightNS, StraightWE, TurnWS, TurnSE, TurnEN, TurnNW, TeeS, TeeE, TeeN, TeeW, Fourway};

    void Awake() {
        for(int ix = 0; ix <= 2*radius; ix++) {
            for(int iy = 0; iy <= 2*radius; iy++) {
                grid[ix,iy] = new GridNode(ix,iy); 
            }
        }
    }
    void Start()
    {
    }

    public void ResetGrid() {
        //conservative, wipes out entire board state
        foreach(GridNode node in grid){
            node.dist = -1; 
            node.goal = false;
        }
    }

    public void ClearGrid() 
    {
        foreach(GridNode node in grid){
            node.type = GridNode.NodeType.Empty;
            if(node.obj != null)
                GameObject.Destroy(node.obj);
        }
        ResetGrid();
    }
    public GridNode.NodeType GetNodeType(int x,int y)
    {
        if(x < -radius || x > radius)
            return GridNode.NodeType.Invalid;
        if(y < -radius || y > radius)
            return GridNode.NodeType.Invalid;
        return grid[x+radius,y+radius].type;
    }

    public void SetPath(ArrayList path) 
    {
        // populate grid state
        foreach (int[] coord in path) {
            GridNode.NodeType type = GetNodeType(coord[0], coord[1]);
            if(type == GridNode.NodeType.Empty)
                SetNodeType(coord[0], coord[1],GridNode.NodeType.Road);
        }

        // mark dirtyQueue as dirty
        foreach(GridNode node in dirtyQueue)
            node.dirty = true;
        
        InstantiateDirtyQueue();

    }

    public void InstantiateDirtyQueue()
    {
        //instatiate objects
        foreach(GridNode node in dirtyQueue)
            InstatiatePrefab(node);
    }

    public GameObject DetermineRoadType(GridNode node)
    {
        GridNode[] neighbors = {
            grid[node.ix+1,node.iy],
            grid[node.ix-1,node.iy],
            grid[node.ix,node.iy+1],
            grid[node.ix,node.iy-1]
        };
        int neighborCode = (neighbors[0].type > GridNode.NodeType.Empty) ? 1 : 0;  //x+1 E
        neighborCode += (neighbors[1].type > GridNode.NodeType.Empty) ? 2 : 0; //x-1 W
        neighborCode += (neighbors[2].type > GridNode.NodeType.Empty) ? 4 : 0; //y+1 N
        neighborCode += (neighbors[3].type > GridNode.NodeType.Empty) ? 8 : 0; //y-1 S
        int x = node.ix -radius;
        int y = node.iy -radius;
        switch(neighborCode)
        {
            case 0:
//                return RoadType.StraightNS;
                return Instantiate(straightPrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,0,0));
            case 1:
//                return RoadType.StraightWE;
                return Instantiate(straightPrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,90,0));
            case 2:
//                return RoadType.StraightWE;
                return Instantiate(straightPrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,90,0));
            case 3:
//                return RoadType.StraightWE;
                return Instantiate(straightPrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,90,0));
            case 4:
//                return RoadType.StraightNS;
                return Instantiate(straightPrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,0,0));
            case 5:
//                return RoadType.TurnEN;
                return Instantiate(turnPrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,180,0));
            case 6:
//                return RoadType.TurnNW;
                return Instantiate(turnPrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,90,0));
            case 7:
//                return RoadType.TeeN;
                return Instantiate(teePrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,90,0));
            case 8:
//                return RoadType.StraightNS;
                return Instantiate(straightPrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,0,0));
            case 9:
//                return RoadType.TurnSE;
                return Instantiate(turnPrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,-90,0));
            case 10:
//                return RoadType.TurnWS;
                return Instantiate(turnPrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,0,0));
            case 11:
//                return RoadType.TeeS;
                return Instantiate(teePrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,-90,0));
            case 12:
//                return RoadType.StraightNS;
                return Instantiate(straightPrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,0,0));
            case 13:
//                return RoadType.TeeE;
                return Instantiate(teePrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,180,0));
            case 14:
//                return RoadType.TeeW;
                return Instantiate(teePrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,0,0));
            case 15:
//                return RoadType.Fourway;
                return Instantiate(fourwayPrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,0,0));
            default:
//                return RoadType.StraightNS;
                return Instantiate(straightPrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,0,0));
        }
    }

    public void ClearRoad(int x,int y)
    {
        GridNode.NodeType type = GetNodeType(x,y);
        if(type == GridNode.NodeType.Road)
            SetNodeType(x,y,GridNode.NodeType.Empty);
    }

    public void SetNodeType(int x,int y,GridNode.NodeType type)
    {
        if(x < -radius || x > radius)
            return;
        if(y < -radius || y > radius)
            return;

        int ix = x+radius;
        int iy = y+radius;

        GridNode node = grid[ix, iy];
        node.type = type;

        if(node.type == GridNode.NodeType.Empty)
        {
            if(node.obj != null)
                GameObject.Destroy(node.obj);
        }

        node.dirty = true;
        dirtyQueue.Add(node);
        //add neighbors to the dirty queue
        dirtyQueue.Add(grid[node.ix+1,node.iy]);
        dirtyQueue.Add(grid[node.ix-1,node.iy]);
        dirtyQueue.Add(grid[node.ix,node.iy+1]);
        dirtyQueue.Add(grid[node.ix,node.iy-1]);
    }

    public void InstatiatePrefab(GridNode node)
    {
        if(!node.dirty)
            return;

        if(node.type == GridNode.NodeType.Road)
        {
            if(node.obj != null)
                GameObject.Destroy(node.obj);
            node.obj = DetermineRoadType(node);            
            node.obj.transform.SetParent(transform);
        }
        if(node.type == GridNode.NodeType.Empty)
        {
            if(node.obj != null)
                GameObject.Destroy(node.obj);
        }

        if(node.type == GridNode.NodeType.Building)
        {
            if(node.obj == null)
            {
                //building is same as fourway, can be connected on every side
                // TO DO, should NOT allow drive through
                int x = node.ix -radius;
                int y = node.iy -radius;
                node.obj = Instantiate(fourstopPrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,0,0));
                node.obj.transform.SetParent(transform);
            }
        }
        node.dirty = false;
    }

    private int AStarIncrementNode(int ix, int iy)
    {
        GridNode node = grid[ix,iy];
        // empty node
        if(node.type == GridNode.NodeType.Empty)
            return -1;

        //Debug.Log(System.String.Format("ASInc ({0},{1}):dist={2} goal={3}", ix-radius, iy-radius, node.dist, node.goal));

        // goal node
        if(node.goal)
            return node.dist;

        GridNode[] neighbors = {
            grid[node.ix+1,node.iy],
            grid[node.ix-1,node.iy],
            grid[node.ix,node.iy+1],
            grid[node.ix,node.iy-1]
        };

        for(int i = 0; i < neighbors.Length; i++) 
        {
            if(neighbors[i].type == GridNode.NodeType.Empty)
                continue;

            //if neighbor is a non-building goal
            if(neighbors[i].type == GridNode.NodeType.Building)
            {
                if(!neighbors[i].goal)
                    continue;
            }

            if(neighbors[i].dist == -1){
                neighbors[i].dist = node.dist+1;
                AstarQueue.Add(new int[]{neighbors[i].ix,neighbors[i].iy});
            }
        }
        return -1;
    }

    public ArrayList RecoverReverseNodeSequence(int ix, int iy) 
    {
        // add the node
        ArrayList path = new ArrayList();

        GridNode node = grid[ix,iy];
        while(true) {
            path.Add(node);
            //back home
            if(node.dist == 0)
                break;

            GridNode[] neighbors = {
                grid[node.ix+1,node.iy],
                grid[node.ix-1,node.iy],
                grid[node.ix,node.iy+1],
                grid[node.ix,node.iy-1]
            };
//            Debug.Log(System.String.Format("Recover ({0},{1}):dist={2}", node.ix-radius,node.iy-radius, node.dist, node.goal));

            int lowestDist = node.dist;
            int lowestIdx = -1;
            for(int i = 0; i < neighbors.Length; i++) 
            {
                //Debug.Log(System.String.Format("Neighbor({0},{1}) idx={2} dist={3}",neighbors[i].ix -radius, neighbors[i].iy-radius, i,neighbors[i].dist));
                // not valid node
                if(neighbors[i].type == GridNode.NodeType.Empty)
                    continue;
                // unvisited node
                if(neighbors[i].dist < 0)
                    continue;

                if(neighbors[i].dist < lowestDist)
                {
                    lowestDist = neighbors[i].dist;
                    lowestIdx = i;
                }
            }
            //Debug.Log(System.String.Format("Best Neighbor idx={0} dist={1}", lowestIdx,lowestDist));

            node = neighbors[lowestIdx];
        }
        return path;
    }

    public ArrayList ConvertNodeSequenceToEdgeSequence(ArrayList nodeList)
    {
        this.interfaceSequence.Clear();
        RoadSection previousSection = null;
        foreach(GridNode node in nodeList)
        {
            RoadSection section = node.obj.GetComponent<RoadSection>();
            if(previousSection != null)
            {
                this.interfaceSequence.Add(previousSection.GetMatchingInterface(section));
            }
            previousSection = section;
        }

        RoadInterface prevRi = null;
        ArrayList edgeList = new ArrayList();
        foreach(RoadInterface ri in this.interfaceSequence)
        {
            if(prevRi != null)
            {
                // Debug.Log(String.Format("PrevRI position: {0}  ********",prevRi.position));
                // Debug.Log(String.Format("RI position: {0}",ri.position));
                //find the edge that connects previous interface to current interface
                bool found = false;
                foreach(RoadEdge e in ri.node1.edges)
                {
                    // Debug.Log(String.Format("Node 1 Edge end parent: {0}",e.end.parent.position));
                    // Debug.Log(String.Format("Node 1 Edge start parent: {0}",e.start.parent.position));
                    if( (e.start.parent.position - prevRi.position).magnitude < 0.1) {
                        edgeList.Add(e);
                        found = true;
                        break;
                    }
                }
                if(found)
                    continue;
                foreach(RoadEdge e in ri.node2.edges)
                {
                    // Debug.Log(String.Format("Node 2 Edge end parent: {0}",e.end.parent.position));
                    // Debug.Log(String.Format("Node 2 Edge start parent: {0}",e.start.parent.position));
                    if( (e.start.parent.position - prevRi.position).magnitude < 0.1) {
                        edgeList.Add(e);
                        found = true;
                        break;
                    }
                }
            }

            prevRi = ri;
        }
        return edgeList;
    }

    public ArrayList FindPath(int x1, int y1, int x2, int y2) 
    {
        ResetGrid();
        AstarQueue.Clear();
        //perform A* search for path using grid
        int ix1 = x1+radius;
        int ix2 = x2+radius;
        int iy1 = y1+radius;
        int iy2 = y2+radius;
        grid[ix1,iy1].dist = 0;
        grid[ix2,iy2].goal = true;
        AstarQueue.Add(new int[]{ix1,iy1});
        while(AstarQueue.Count > 0) 
        {
            int[] coord =  (int[])AstarQueue[0];
            int dist = AStarIncrementNode(coord[0], coord[1]); 
            AstarQueue.RemoveAt(0);
            if(dist > 0)
            {
               // Debug.Log(System.String.Format("Goal found in {0} steps", dist));
                prevNodeSequence = nodeSequence;
                nodeSequence = RecoverReverseNodeSequence(coord[0], coord[1]);
     
                bool dirty = false;

                //check if node sequence changed, 
                if(nodeSequence.Count == prevNodeSequence.Count)
                {
                    for(int i = 0; i < nodeSequence.Count; i++) {

                        if( ((GridNode)nodeSequence[i]).ix != ((GridNode)prevNodeSequence[i]).ix)
                        {
                            dirty = true;
                            break;
                        }
                        if( ((GridNode)nodeSequence[i]).iy != ((GridNode)prevNodeSequence[i]).iy)
                        {
                            dirty = true;
                            break;
                        }
                    }
                }else {
                    dirty=true;
                }
                //only update if path changed
                if(dirty)
                {
                    ArrayList forwardEdgeSequence = ConvertNodeSequenceToEdgeSequence(nodeSequence);
                    nodeSequence.Reverse();
                    ArrayList reverseEdgeSequence = ConvertNodeSequenceToEdgeSequence(nodeSequence);
                    //return to original order for diff check
                    nodeSequence.Reverse();
                    vehicle.GetComponent<FollowPath>().SetPath(forwardEdgeSequence, true);
                    vehicle.GetComponent<FollowPath>().SetPath(reverseEdgeSequence, false);
                }
                return nodeSequence;
            }
        }
        // no path found, clear.
        //Debug.Log("No path to Goal found");
        interfaceSequence.Clear();
        vehicle.GetComponent<FollowPath>().SetPath(null, true);
        vehicle.GetComponent<FollowPath>().SetPath(null, false);
        nodeSequence.Clear();
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        // if(nodeSequence != null) {
        //     GridNode last = null;
        //     Vector3 offset = new Vector3(0,0.5f,0);
        //     bool first = true;
        //     foreach (GridNode node in nodeSequence) {
        //         if(first)
        //         {
        //             Util.DebugDrawX(node.obj.transform.position,0.1f,Color.red);
        //             first = false;
        //         }
        //         if(last != null) {
        //             Debug.DrawLine( node.obj.transform.position + offset, 
        //                             last.obj.transform.position + offset,
        //                             Color.red);
        //         }
        //         last = node;
        //     }
        // }

        if(interfaceSequence.Count > 0) {
            Vector3 offset = new Vector3(0,0.25f,0);
            Vector3 prev = Vector3.zero;
            foreach (RoadInterface ri in interfaceSequence) {
                if(prev != Vector3.zero)
                {
                    Debug.DrawLine( ri.position + offset, 
                                    prev,
                                    Color.green);
                }
                prev = ri.position + offset;
            }
        }

        // if(forwardEdgeSequence != null) {
        //     Vector3 offset = new Vector3(0,0.25f,0);
        //     Vector3 prev = Vector3.zero;
        //     bool first = true;
        //     foreach (RoadEdge e in forwardEdgeSequence) {
        //         if(first)
        //         {
        //             Util.DebugDrawX(e.start.position,0.1f,Color.green);
        //             first = false;
        //         }
        //         e.DebugDraw(Color.green);
        //     }
        // }
    }
}
