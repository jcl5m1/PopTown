using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
public class Node 
{
    public int x;
    public int y;
    public int id;
    public int value;

    public List<Edge> edges = new List<Edge>();

    public Node(int x, int y, int id, int value) 
    {
        this.x = x;
        this.y = y;
        this.id = id;
        this.value = value;
    }
}

public class Edge 
{
    public Node a;
    public Node b;
    public int distance;
    public Edge(Node a, Node b, int distance) 
    {
        this.a = a;
        this.b = b;
        this.distance = distance;
    }

}

public class Graph {

    public List<Node> nodes = new List<Node>();
    public List<Edge> edges = new List<Edge>();

    public Graph() {
        Debug.Log("init graph");
        InitGrid();
    }

    public void InitGrid(int size = 10) 
    {
        int idx = 0;
        Node[] prevRow = new Node[2*size+1]; 
        for(int y = -size; y <= size; y++) {
            Node prevNode = null;
            for(int x = -size; x <= size; x++) {
                Node n = new Node(x,y,idx,0);
                if(prevNode != null)
                {
                    Edge e = new Edge(prevNode, n,1);
                    prevNode.edges.Add(e);
                    n.edges.Add(e);
                }
                nodes.Add(n);
                prevRow[x+size] = n;
                prevNode = n;
                idx++;
                Debug.Log(idx);
            }
        }
    }
}