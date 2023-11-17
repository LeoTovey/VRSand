using CurvedUI;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class HandUIController : MonoBehaviour
{
    public HandController Hands;
    public Canvas LeftHandUI;
    public VRUIEventSystem eventSystem;

    void Start()
    {
        Hands.LeftHand.BindHandPoseStartCallback(HandPose.UIActivation, ActiveUI);
        Hands.LeftHand.BindHandPoseEndCallback(HandPose.UIActivation, DisActiveUI);
    }

    void Update()
    {
        if (LeftHandUI.gameObject.activeSelf)
        {
            float canvasHeight = LeftHandUI.GetComponent<RectTransform>().rect.height;
            float canvasWidth = LeftHandUI.GetComponent<RectTransform>().rect.width;
            LeftHandUI.transform.position = Hands.LeftHand.PalmTransform.position + new Vector3(0.5f * canvasWidth, 0.05f + 0.5f * canvasHeight, 0);
            Vector3 cameraPos = Camera.main.transform.position;
            LeftHandUI.transform.LookAt(cameraPos, Vector3.up);
            LeftHandUI.transform.Rotate(0, 180, 0);

            eventSystem.ProcessPointEvent(Hands.RightHand.IndexTipTransform.position);
        }
    }


    public void DisActiveUI()
    {
        LeftHandUI.gameObject.SetActive(false);
    }

    public void ActiveUI()
    {
        LeftHandUI.gameObject.SetActive(true);
    }
}
