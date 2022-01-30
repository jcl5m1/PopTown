using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGridNode : MonoBehaviour
{
    public GridRoads gridRoads;
    public GridNode.NodeType type;

    // Start is called before the first frame update
    void Start()
    {
        int x = (int)Mathf.Round(transform.position.x);
        int z = (int)Mathf.Round(transform.position.z);

        gridRoads.SetNodeType(x,z,type);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
