using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNode {
    public int ix;
    public int iy;
    public int type;
    public int dist;
    public bool goal;
    public GameObject obj;
    public GridNode(int ix, int iy) {
        this.ix = ix;
        this.iy = iy;
        type = 0;
        dist = -1;
        goal = false;
    }
}

public class GridRoads : MonoBehaviour
{
    public static int radius = 100;
    public GridNode[,] grid = new GridNode[2*radius+1, 2*radius+1];
    // Start is called before the first frame update

    public GameObject occupiedPrefab;
    public GameObject vehicle;
    public float roadHeight = 0.01f;

    private ArrayList AstarQueue = new ArrayList();
    private ArrayList LastPath;

    void Start()
    {
        for(int ix = 0; ix <= 2*radius; ix++) {
            for(int iy = 0; iy <= 2*radius; iy++) {
                grid[ix,iy] = new GridNode(ix,iy); 
            }
        }
    }

    public void ResetGrid() {
        //conservative, wipes out entire board state
        foreach(GridNode node in grid){
            node.dist = -1; 
            node.goal = false;
            // if(node.obj != null)
            //     node.obj.GetComponentInChildren<TextMesh>().text = node.dist.ToString();
        }
    }
    public int Get(int x,int y)
    {
        if(x < -radius || x > radius)
            return -1;
        if(y < -radius || y > radius)
            return -1;
        return grid[x+radius,y+radius].type;
    }

    public void Set(int x,int y,int type)
    {
        if(x < -radius || x > radius)
            return;
        if(y < -radius || y > radius)
            return;

        int oldValue = grid[x+radius,y+radius].type;
        if(oldValue != type)
        {
            int ix = x+radius;
            int iy = y+radius;

            GridNode node = grid[ix, iy];
            node.type = type;
            if(node.type == 1)
            {
                node.obj = Instantiate(occupiedPrefab, new Vector3(x, roadHeight, y), Quaternion.Euler(90,0,0));
                node.obj.transform.SetParent(transform);
            }
            if(node.type == 0)
            {
                if(node.obj != null)
                    GameObject.Destroy(node.obj);
            }
        }
    }

    private int AStarIncrementNode(int ix, int iy)
    {
        GridNode node = grid[ix,iy];
        // empty node
        if(node.type == 0)
            return -1;
//        node.obj.GetComponentInChildren<TextMesh>().text = node.dist.ToString();

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
            if(neighbors[i].type == 0)
                continue;
            if(neighbors[i].dist == -1){
                neighbors[i].dist = node.dist+1;
                AstarQueue.Add(new int[]{neighbors[i].ix,neighbors[i].iy});
            }
        }
        return -1;
    }

    public ArrayList RecoverReversePath(int ix, int iy) 
    {

        // add the node
        ArrayList path = new ArrayList();

        GridNode node = grid[ix,iy];
        while(true) {
            path.Add(new int[]{node.ix-radius,node.iy-radius});
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
                if(neighbors[i].type == 0)
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
                LastPath = RecoverReversePath(coord[0], coord[1]);
                vehicle.GetComponent<FollowPath>().SetPath(LastPath);
                return LastPath;
            }
        }
        //Debug.Log("No path to Goal found");
        LastPath = null;
        return null;       
    }

    // Update is called once per frame
    void Update()
    {
        if(LastPath != null) {
            int[] last = null;
            foreach (int[] coord in LastPath) {
                if(last != null) {
                    Debug.DrawLine(new Vector3(last[0], 0.5f, last[1]), new Vector3(coord[0], 0.5f, coord[1]), Color.green);
                }
                last = coord;
            }
        }
    }
}
