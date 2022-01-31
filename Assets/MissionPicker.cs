using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionPicker : MonoBehaviour
{
    public GridNode start;
    public GridNode destination;
    public GridRoads gridRoads;
    // Start is called before the first frame update
    public ArrayList prevNodeSequence = new ArrayList();
    public ArrayList nodeSequence = new ArrayList();
    public ArrayList interfaceSequence = new ArrayList();
    
    void Start()
    {

    }

    public void FindPath()
    {
        if(start == null)
            return;
        if(destination == null)
            return;

        prevNodeSequence = nodeSequence;
        if(gridRoads == null)
            gridRoads = FindObjectOfType<GridRoads>();

        ArrayList seq = gridRoads.FindPath(start, destination);

        if(seq == null) {
            // no path found, clear.
            interfaceSequence.Clear();
            GetComponent<FollowPath>().SetPath(null, true);
            GetComponent<FollowPath>().SetPath(null, false);
            nodeSequence.Clear();
            prevNodeSequence.Clear();
            return;
        }

        bool dirty = false;
        nodeSequence = seq;

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
            nodeSequence.Reverse();
            interfaceSequence = gridRoads.ConvertNodeSequenceToInterfaceSequence(nodeSequence);
            ArrayList forwardEdgeSequence = gridRoads.ConvertInterfaceSequenceToEdgeSequence(interfaceSequence);
            nodeSequence.Reverse();
            interfaceSequence = gridRoads.ConvertNodeSequenceToInterfaceSequence(nodeSequence);
            ArrayList reverseEdgeSequence = gridRoads.ConvertInterfaceSequenceToEdgeSequence(interfaceSequence);
            //reverse again, so we can do change detection
            GetComponent<FollowPath>().SetPath(forwardEdgeSequence, true);
            GetComponent<FollowPath>().SetPath(reverseEdgeSequence, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
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
    }
}
