using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGridNode : MonoBehaviour
{
    public GridRoads gridRoads;
    public GridNode.NodeType type;
    public GridNode.NodeSubType subType;
    public GridNode node;

    // Start is called before the first frame update
    void Start()
    {
        if(gridRoads == null)
            gridRoads = FindObjectOfType<GridRoads>();

        int x = (int)Mathf.Round(transform.position.x);
        int z = (int)Mathf.Round(transform.position.z);

        this.node = gridRoads.SetNodeType(x,z,type);
        this.node.subType = subType;
        gridRoads.InstantiateDirtyQueue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
