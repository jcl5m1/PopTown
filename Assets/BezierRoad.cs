using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BezierRoad : MonoBehaviour
{
    private Bezier lane1;
    private Bezier lane2;
    
    public Bezier CreateTurn(float roadMargin=0.25f, float handleLength=0.5f)
    {
        float y = 0.001f;

        Vector3 pos = transform.position - new Vector3(0.5f,0,0.5f);
        Bezier curve = new Bezier(
            new Vector3(pos.x+roadMargin, y, pos.z),
            new Vector3(pos.x, y, roadMargin+pos.z),
            new Vector3(pos.x+roadMargin, y, handleLength+pos.z),
            new Vector3(pos.x+handleLength, y, roadMargin+pos.z)
        );
        return curve;
    }

    // Start is called before the first frame update
    void Start()
    {
        lane1 = CreateTurn(0.25f, 0.3f);
        lane2 = CreateTurn(0.75f,0.9f);

        Debug.Log(lane1.Length());
        Debug.Log(lane2.Length());
    }

    void Update()
    {
        lane1.DebugDraw(5);
        lane2.DebugDraw(5);
    }
}
