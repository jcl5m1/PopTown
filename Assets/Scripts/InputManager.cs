using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public GameObject cursor;
    public GridRoads gridRoads;
    public RoadPreview roadPreview;
    private int[] currPos = new int[]{0,0};

    private int[] prevPos = new int[]{0,0};
    // private bool wasDown = false;
    private bool addingRoad = false;

    private TouchControls touchControls;

    private Vector2 touchPosition;
    private bool touching = false;
    private bool wasTouching = false;

    private void Awake() {
        touchControls = new TouchControls();    
    }

    private void OnEnable() {
        touchControls.Enable();
    }

    private void OnDisable() {
        touchControls.Disable();
    }
    // Start is called before the first frame update
    private void Start()
    {
        touchControls.Touch.TouchPress.started += ctx => StartTouch(ctx);
        touchControls.Touch.TouchPress.canceled += ctx => EndTouch(ctx);
        touchControls.Keyboard.Space.performed += ctx => SpacePressed(ctx);
        touchControls.Keyboard.Reset.performed += ctx => ResetPressed(ctx);
    }

    private void StartTouch(InputAction.CallbackContext context) 
    {
        touching = true;
    }

    private void EndTouch(InputAction.CallbackContext context) 
    {
        touching = false;
    }

    private void SpacePressed(InputAction.CallbackContext context) 
    {
        gridRoads.FindPath(0,5,0,0);
    }
    private void ResetPressed(InputAction.CallbackContext context) 
    {
        gridRoads.ClearGrid();
    }

    IEnumerator FindPath()
    {
        // wait 1 seconds
        yield return new WaitForSeconds(1);
        gridRoads.FindPath(0,5,0,0);

    }
    // Update is called once per frame
    void Update()
    {
        if(touching)
        {
            touchPosition = touchControls.Touch.TouchPosition.ReadValue<Vector2>();
            //touch simulation gives a (0,0) initally which means I can't rely on touch down?
            if(touchPosition == Vector2.zero)
                return;

            RaycastHit hit;
            Ray ray = GetComponent<Camera>().ScreenPointToRay(touchPosition);
            if (Physics.Raycast(ray, out hit)) {
                Vector3 position = new Vector3();
                position.x = (int)Mathf.Round(hit.point.x);
                position.z = (int)Mathf.Round(hit.point.z);

                //visual indicator of touch point
                //cursor.transform.position = position;  
                
                prevPos[0] = currPos[0];
                prevPos[1] = currPos[1];

                currPos[0] = (int)(position.x);
                currPos[1] = (int)(position.z);

//                Debug.Log(touchPosition + " ->  " + currPos[0] + ", " + currPos[1]);

            }

            if(!wasTouching)
            {
                int type = gridRoads.Get(currPos[0], currPos[1]);
                if(type == 0) {
                    addingRoad = true;
                    roadPreview.x1 = currPos[0];
                    roadPreview.z1 = currPos[1];
                    roadPreview.UpdateDestination(currPos[0], currPos[1]);
                }
                else
                {
                    addingRoad = false;
                }
            }

            if(addingRoad) {
                if(prevPos[0] != currPos[0] || prevPos[1] != currPos[1]) {
                    roadPreview.UpdateDestination(currPos[0], currPos[1]);
                }
            }else {
                gridRoads.Set(currPos[0], currPos[1],0);
            }
            wasTouching = true;
        }
        else 
        {
            if(wasTouching) {
                gridRoads.SetPath(roadPreview.path);
                roadPreview.Clear();

                // if execute immediately, game objects aren't instantiated yet
                StartCoroutine("FindPath");
            }
            wasTouching = false;
        }

    //     if (Input.GetMouseButton(0)) {
    //         if(!wasDown)
    //         {
    //             int type = gridRoads.Get(currPos[0], currPos[1]);
    //             if(type == 0) {
    //                 addingRoad = true;
    //                 roadPreview.x1 = currPos[0];
    //                 roadPreview.z1 = currPos[1];
    //                 roadPreview.UpdateDestination(currPos[0], currPos[1]);
    //             }
    //             else
    //             {
    //                 addingRoad = false;
    //             }
    //         }
    //         if(addingRoad) {
    //             if(prevPos[0] != currPos[0] || prevPos[1] != currPos[1]) {
    //                 roadPreview.UpdateDestination(currPos[0], currPos[1]);
    //             }
    //         }else {
    //             gridRoads.Set(currPos[0], currPos[1],0);
    //         }
    //         wasDown = true;
    //     }else {
    //         if(wasDown) {
    //             gridRoads.SetPath(roadPreview.path);
    //             roadPreview.Clear();
    //         }
    //         prevPos[0]=10000;
    //         prevPos[1]=10000;
    //         wasDown = false;
    //     }


    }
}

