using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VehicleManager : MonoBehaviour
{
    public GameObject vehiclePrefab;
    public int count;

    public ArrayList vehicles = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {


    }

    public void SetVehicle() {
        GameObject vehicle;
        if(vehicles.Count == count)
        {
            vehicle = (GameObject)vehicles[0];
        }
        else
        {
            vehicle = Instantiate(vehiclePrefab, Vector3.zero, Quaternion.Euler(0,0,0));
            vehicle.transform.parent = transform;
            vehicles.Add(vehicle);
        }

        MissionPicker mp = vehicle.GetComponent<MissionPicker>();
        GridNode node = GetComponent<SetGridNode>().node;
        mp.start = node;

        SetGridNode[] buildings = FindObjectsOfType<SetGridNode>();
        foreach(SetGridNode b in buildings)
        {
            if(b.node.subType == GridNode.NodeSubType.Store)
            {
                mp.destination = b.node;
                break;
            }

        }

        mp.FindPath();
        //Debug.Log(mp);

        // // look for all mission pickers in the scene        
        // // TODO just find ones affected by the change
        // MissionPicker[] pickers = FindObjectsOfType<MissionPicker>();
        // foreach(MissionPicker mp in pickers)
        // {
        //     mp.FindPath();
        // }

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
