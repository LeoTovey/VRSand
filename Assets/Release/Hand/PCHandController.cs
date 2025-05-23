using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PCHandController : MonoBehaviour
{
    
    [Serializable]
    public struct HandPoseMesh
    {
        public HandPose handPose;
        public AiTipHand handJoints;
    }
    
    public HandController HandController;
    [SerializeField] private Camera Camera;
    [SerializeField] private Collider SandBoardCollider;
    [SerializeField] private Collider HandUICollider;
    [SerializeField] private Transform ScatterPouringTransform;
    private float _scatterPouringRadius;
    
    public List<HandPoseMesh> prefabList = new List<HandPoseMesh>();
    private Dictionary<HandPose, AiTipHand> _handJoints = new Dictionary<HandPose, AiTipHand>();

    public float HandScale = 1.0f;
    
    private Vector2 _mousePos;
    private RaycastHit _hit;
    private AiTipHand _currentHand;
    private float _strength = 0.0f;
    
    private float _rotationSpeed = 100.0f;
    private float _currentRotationY = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        foreach (var pair in prefabList)
        {
            _handJoints[pair.handPose] = pair.handJoints;
            pair.handJoints.gameObject.transform.localScale = new Vector3(HandScale, HandScale, HandScale);
            
            HandController.RightHand.BindHandPoseStartCallback(pair.handPose, () =>
            {
                HandController.RightHand.UpdateHandJoints(ref _handJoints[pair.handPose].HandJoints);
            });
        }
        
        HandController.RightHand.BindHandPoseStartCallback(HandPose.ScatterPouring, () =>
        {
            ScatterPouringTransform.gameObject.SetActive(true);
        });
        
        HandController.RightHand.BindHandPoseEndCallback(HandPose.ScatterPouring, () =>
        {
            ScatterPouringTransform.gameObject.SetActive(false);
        });
        
        HandController.RightHand.BindHandPoseStartCallback(HandPose.SkinnyPouring, () =>
        {
            ScatterPouringTransform.gameObject.SetActive(true);
        });
        
        HandController.RightHand.BindHandPoseEndCallback(HandPose.SkinnyPouring, () =>
        {
            ScatterPouringTransform.gameObject.SetActive(false);
        });
        HandController.RightHand.PCScale = HandScale;
        HandController.RightHand.SetHandPose(HandPose.None);
        HandController.RightHand.UpdateHandJoints(ref _handJoints[HandPose.None].HandJoints);
        _currentHand = _handJoints[HandPose.None];
        _scatterPouringRadius = ScatterPouringTransform.localScale.z;
    }
    
    void Update()
    {
        ProcessInput();
        HandController.RightHand.SandSkinnyPouring.EnablePouring = HandController.RightHand.IsInteracting;
        HandController.RightHand.SandScatterPouring.EnablePouring = HandController.RightHand.IsInteracting;
        HandController.RightHand.SetStrength(_strength);
        ScatterPouringTransform.localScale = new Vector3(_scatterPouringRadius, _scatterPouringRadius, _scatterPouringRadius);

        // UI 状态
        if (HandController.RightHand.CurrentHandState == HandState.UI)
        {
            if (Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out _hit) &&
                _hit.collider == HandUICollider)
            {
                Vector3 offset = HandController.RightHand.IndexTipTransform.position -
                                 HandController.RightHand.transform.position;
                HandController.RightHand.transform.position = _hit.point - offset;
            }
        }
        else
        {
            if( Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out _hit) && _hit.collider == SandBoardCollider )
            {

                Vector3 handPos = _hit.point + new Vector3(0, 0.1f, 0);
                if (HandController.RightHand.IsInteracting)
                {
                    handPos.y = _currentHand.gameObject.transform.position.y;
                }
            
                HandController.RightHand.transform.position = handPos;
                
                if (ScatterPouringTransform.gameObject.activeSelf)
                {
                    ScatterPouringTransform.position = _hit.point;
                }
            }
        }
        

        
    }

    private void ProcessInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            HandController.SetRightHandPose(HandPose.SkinnyPouring);
            _currentHand = _handJoints[HandPose.SkinnyPouring];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HandController.SetRightHandPose(HandPose.ScatterPouring);
            _currentHand = _handJoints[HandPose.ScatterPouring];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            HandController.SetRightHandPose(HandPose.FingertipTracing);
            _currentHand = _handJoints[HandPose.FingertipTracing];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            HandController.SetRightHandPose(HandPose.FingerCarving);
            _currentHand = _handJoints[HandPose.FingerCarving];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            HandController.SetRightHandPose(HandPose.HandSweeping);
            _currentHand = _handJoints[HandPose.HandSweeping];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            HandController.SetRightHandPose(HandPose.PalmRubbing);
            _currentHand = _handJoints[HandPose.PalmRubbing];
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            HandController.SetRightHandPose(HandPose.UIActivation);
            _currentHand = _handJoints[HandPose.FingertipTracing];
        }
        
        var scrollWheelValue = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheelValue != 0.0f)
        {
            if (Input.GetKey(KeyCode.A))
            {
                _scatterPouringRadius = _scatterPouringRadius - scrollWheelValue * 0.1f;
                _scatterPouringRadius = Mathf.Clamp(_scatterPouringRadius, 0.0f, 1.0f);
            }
            else
            {
                if (HandController.RightHand.CurrentHandPose == HandPose.SkinnyPouring ||
                    HandController.RightHand.CurrentHandPose == HandPose.ScatterPouring)
                {
                    _strength = _strength - scrollWheelValue;
                    _strength = Mathf.Clamp(_strength, 0.0f, 1.0f);
                }
                else
                {
                    _currentRotationY -= _rotationSpeed * scrollWheelValue;
                    HandController.RightHand.transform.rotation =  Quaternion.Euler(0f, _currentRotationY, 0f);
                }
            }


        }

        HandController.RightHand.IsInteracting = Input.GetMouseButton(0);
        
    }
}
