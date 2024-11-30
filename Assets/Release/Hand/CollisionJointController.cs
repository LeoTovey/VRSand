using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionJointController : MonoBehaviour
{
    // Start is called before the first frame update
    public Hand RightHand;
    public Camera CollisionCamera;

    public GameObject FingertipTracing;
    public GameObject FingerCarving;

    int CollisionPlaneLayer;
    int HandsLayer;
    int ToolsLayer;
    int JointsLayer;

    int jointCullingMask;
    int handCullingMask;

    void Start()
    {
        // int CollisionPlaneLayer = LayerMask.NameToLayer("CollisionPlane");
        // int HandsLayer = LayerMask.NameToLayer("Hands");
        // int ToolsLayer = LayerMask.NameToLayer("Tools");
        // int JointsLayer = LayerMask.NameToLayer("PhysicsMesh");
        //
        // FingerCarving.SetActive(false);
        // FingertipTracing.SetActive(false);
        //
        // jointCullingMask = 1 << CollisionPlaneLayer | 1 << ToolsLayer | 1 << JointsLayer;
        // handCullingMask = 1 << CollisionPlaneLayer | 1 << ToolsLayer | 1 << HandsLayer;
        // CollisionCamera.cullingMask = handCullingMask;
        //
        // RightHand.BindHandPoseStartCallback(HandPose.FingerCarving, () => {
        //     FingerCarving.SetActive(true);
        //     CollisionCamera.cullingMask = jointCullingMask;
        // });
        //
        // RightHand.BindHandPoseEndCallback(HandPose.FingerCarving, () => {
        //     FingerCarving.SetActive(false);
        //     CollisionCamera.cullingMask = handCullingMask;
        // });


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
