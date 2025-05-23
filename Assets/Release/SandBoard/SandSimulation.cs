using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.IO;
using TMPro;
using System;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.InputSystem;

public class SandSimulation : MonoBehaviour
{
    public SandSimulationKernel Kernel;

    public Hand RightHand => _hands.RightHand;
    public Hand LeftHand => _hands.LeftHand;

    [SerializeField] private HandController _hands;
    [SerializeField] private Camera _collisionRenderingCamera;
    [SerializeField] private RenderTexture _collisionMap;
    [SerializeField] private Transform _collisionPlaneTransform;

    public RawImage ColorHandleImage;
    public Color SandColor;

    public Transform CollisionPlane;

    // for simulation
    public float InitHeight = 0.5f;
    public float MaxHeight = 1.0f;
    public Color InitColor;

    private int SkinnyPouringRadius = 5;
    public Renderer ScatterPouringRenderer;

    private Vector3 _collisionPlaneScale => CollisionPlane.lossyScale;
    private Vector3 _collisionPlaneCenter => CollisionPlane.position;
    private float _halfWidth => _collisionPlaneScale.x * 0.5f;
    private float _halfHeight => _collisionPlaneScale.y * 0.5f;
    private float _planeMinX => _collisionPlaneCenter.x - _halfWidth;
    private float _planeMinZ => _collisionPlaneCenter.z - _halfHeight;
    private float _planeMaxX => _collisionPlaneCenter.x + _halfWidth;
    private float _planeMaxZ => _collisionPlaneCenter.z + _halfHeight;
    

    
    [SerializeField] private RenderTexture HeightRT;
    [SerializeField] private RenderTexture CollisionRT;
    
    // _updateArea 每次fixed update都需要更新的区域
    private RTPixelAABB _updateArea;
    private RTPixelAABB _area;
    private Vector2 _displacementRatio = new Vector2(0.5f, 0.5f);
    private bool _finishCollision = false;
    
    private void Awake()
    {
        //Application.targetFrameRate = -1;
    }

    void Start()
    {
        _collisionRenderingCamera.clearFlags = CameraClearFlags.Nothing;
        _updateArea = new RTPixelAABB(HeightRT.width, HeightRT.height);
        _area = new RTPixelAABB(HeightRT.width, HeightRT.height);
    }

    public bool IsIntersect(Bounds bounds)
    {
        bool isIntersectX = bounds.min.x < _planeMaxX && bounds.max.x > _planeMinX;
        bool isIntersectZ = bounds.min.z < _planeMaxZ && bounds.max.z > _planeMinZ;
        bool isIntersectY = bounds.min.y < _collisionPlaneCenter.y && bounds.max.y > _collisionPlaneCenter.y;
        return isIntersectX && isIntersectY && isIntersectZ;
    }

    public bool IsIntersectXZ(Bounds bounds)
    {
        bool isIntersectX = bounds.min.x < _planeMaxX && bounds.max.x > _planeMinX;
        bool isIntersectZ = bounds.min.z < _planeMaxZ && bounds.max.z > _planeMinZ;
        return isIntersectX && isIntersectZ;
    }
    
    

    void Update()
    {
        UpdateHandArea(RightHand);
        UpdateHandArea(LeftHand);
        
        UpdateCollisionRenderingState();
    }

    void UpdateHandArea(Hand hand)
    {
        if (hand.CurrentHandState == HandState.TOOLS && hand.Pen.gameObject.activeSelf)
        {
            if (hand.Pen.DetectCollision() && IsIntersect(hand.Pen.CollisionBound))
            {
                _updateArea.MergeBounds(hand.Pen.CollisionBound, _collisionPlaneTransform);
            }
        }
        else if (hand.DetectCollision() && IsIntersect(hand.CollisionBound))
        {
            _updateArea.MergeBounds(hand.CollisionBound, _collisionPlaneTransform);
        }
    }

    void UpdateCollisionRenderingState()
    {
        if (_finishCollision)
        {
            _collisionRenderingCamera.clearFlags = CameraClearFlags.SolidColor;
            _finishCollision = false;
        }
        else
        {
            _collisionRenderingCamera.clearFlags = CameraClearFlags.Nothing;
        }
    }
    
    private void FixedUpdate()
    {
        if (!_updateArea.IsEmpty())
        {
            Kernel.CollisionTest(_updateArea, _displacementRatio);
            
            
            _updateArea.Reset();
            _finishCollision = true;
        }
        
        
        /*
        if (Hands[0].CurrentHandPose == HandPose.SkinnyPouring && !test)
        {
            int width = HeightMap.width;
            int height = HeightMap.height;
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
            RenderTexture.active = HeightMap;
            texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture2D.Apply();
            byte[] bytes = texture2D.EncodeToPNG();
            File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "SavedScreen.png"), bytes);
            test = true;
        }
        */

        // ResetUpdateArea();
        //
        // bool usePen = false;
        //
        // if (RightHand.CurrentHandPose == HandPose.SkinnyPouring && RightHand.CurrentHandState == HandState.DRAW)
        // {
        //     if (RightHand.SandSkinnyPouring.Strength > 0.0f && RightHand.SandSkinnyPouring.EnablePouring)
        //     {
        //         Bounds bounds = RightHand.HandBound;
        //         if (IsIntersectXZ(bounds))
        //         { 
        //             Kernel.SkinnyPouring(bounds, CollisionPlane, ScatterPouringRenderer.transform.position, RightHand.SandSkinnyPouring.SandAmount, SkinnyPouringRadius);
        //         }
        //     }
        //     _hands._poseCounter[RightHand.CurrentHandPose] += Time.fixedDeltaTime;
        // }
        // else if(RightHand.CurrentHandPose == HandPose.ScatterPouring && RightHand.CurrentHandState == HandState.DRAW)
        // {
        //     if (RightHand.SandScatterPouring.Strength > 0.0f && RightHand.SandScatterPouring.EnablePouring)
        //     {
        //         //Bounds bounds = RightHand.ScatterPouringCenter;
        //         Bounds bounds = ScatterPouringRenderer.bounds;
        //         if (IsIntersectXZ(bounds))
        //         {
        //             Kernel.ScatterPouring(bounds, CollisionPlane, RightHand.SandScatterPouring.SandAmount);
        //         }
        //     }
        //     _hands._poseCounter[RightHand.CurrentHandPose] += Time.fixedDeltaTime;
        // }
        // else if (LeftHand.CurrentHandState == HandState.TOOLS && LeftHand.Pen.gameObject.activeSelf)
        // {
        //     Displacement(LeftHand.Pen);
        //     usePen = true;
        // }
        // else if (RightHand.CurrentHandState == HandState.TOOLS && RightHand.Pen.gameObject.activeSelf)
        // {
        //     Displacement(RightHand.Pen);
        //     usePen = true;
        // }
        // else
        // {
        //     Displacement(RightHand);
        //     Displacement(LeftHand);
        // }
        //
        // if (Kernel.IsAreaUpdated())
        // {
        //     Kernel.Displacement();
        //     if (usePen == true)
        //     {
        //         _hands._poseCounter[HandPose.ToolHolding] += Time.fixedDeltaTime;
        //     }
        //     else
        //     {
        //         _hands._poseCounter[RightHand.CurrentHandPose] += Time.fixedDeltaTime;
        //     }
        // }
    }

    bool Displacement(ICollision collision)
    {
        // Bounds bounds = collision.CollisionBound;
        // if (collision.DetectCollision() && IsIntersect(bounds))
        // {
        //     Kernel.CollisionTest(bounds, CollisionPlane, collision);
        //     Kernel.Sweep(collision.Movement);
        //     
        //     return true;
        // }
        // collision.MergeBounds();
        return false;
    }
}
