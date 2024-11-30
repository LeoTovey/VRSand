using CurvedUI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;



public class PCHandUIController : MonoBehaviour
{
    public HandController Hands;
    public Canvas LeftHandUI;
    public Transform SandBoardTransform;
    public Camera Camera;
    public GraphicRaycaster Raycaster;

    GameObject previousGameObject = null;
    
    private Ray _ray = new Ray();
    private float _distance;
    private Vector2 _pointOnCanvas;

    void Start()
    {
        Hands.RightHand.BindHandPoseStartCallback(HandPose.UIActivation, ActiveUI);
        Hands.RightHand.BindHandPoseEndCallback(HandPose.UIActivation, DisActiveUI);
        
     
    }

    void Update()
    {
        if (LeftHandUI.gameObject.activeSelf)
        {
            PointerEventData pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            Raycaster.Raycast(pointer, results);
        }
    }


    public void DisActiveUI()
    {
        LeftHandUI.gameObject.SetActive(false);
    }

    public void ActiveUI()
    {
        Vector3 position = Hands.RightHand.PalmTransform.position;
        LeftHandUI.transform.position = SandBoardTransform.position + Vector3.up * 0.001f;
        LeftHandUI.gameObject.SetActive(true);
    }
}
