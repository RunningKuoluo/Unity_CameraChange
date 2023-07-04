using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TouchScript.Gestures.TransformGestures;
using System;

public class Touch : MonoBehaviour
{
    public ScreenTransformGesture OneFingerMoveGesture;
    public ScreenTransformGesture ManipulationGesture;
    public GameObject TREX;

    private void OnEnable()
    {
        OneFingerMoveGesture.Transformed += oneFingerTransformHandler;
        ManipulationGesture.Transformed += mainpulationTransformedHandler;
    }

    private void OnDisable()
    {
        OneFingerMoveGesture.Transformed -= oneFingerTransformHandler;
        ManipulationGesture.Transformed -= mainpulationTransformedHandler;
    }

    private void mainpulationTransformedHandler(object sender, EventArgs e)
    {
        Debug.Log(ManipulationGesture.DeltaScale);
        TREX.transform.localScale += new Vector3(ManipulationGesture.DeltaScale - 1f, ManipulationGesture.DeltaScale - 1f, ManipulationGesture.DeltaScale - 1f);
    }

    private void oneFingerTransformHandler(object sender, EventArgs e)
    {
        Debug.Log(OneFingerMoveGesture.DeltaPosition.x + "_____" + OneFingerMoveGesture.DeltaPosition.y);
        float y = OneFingerMoveGesture.DeltaPosition.x + TREX.transform.rotation.y;
        TREX.transform.eulerAngles += new Vector3(0, y, 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
