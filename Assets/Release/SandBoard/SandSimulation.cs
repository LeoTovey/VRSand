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



    public Pen Pen;
  

    public RawImage ColorHandleImage;
    public Color SandColor;

    public Transform CollisionPlane;

    // for simulation
    public float InitHeight = 0.5f;
    public float MaxHeight = 1.0f;
    public Color InitColor;

    private int SandRadius = 5;

    private Vector3 _collisionPlaneScale => CollisionPlane.lossyScale;
    private Vector3 _collisionPlaneCenter => CollisionPlane.position;
    private float _halfWidth => _collisionPlaneScale.x * 0.5f;
    private float _halfHeight => _collisionPlaneScale.y * 0.5f;
    private float _planeMinX => _collisionPlaneCenter.x - _halfWidth;
    private float _planeMinZ => _collisionPlaneCenter.z - _halfHeight;
    private float _planeMaxX => _collisionPlaneCenter.x + _halfWidth;
    private float _planeMaxZ => _collisionPlaneCenter.z + _halfHeight;



    private void Awake()
    {
        //Application.targetFrameRate = -1;
    }

    void Start()
    {

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
        
    }

    private void FixedUpdate()
    {
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

        Kernel.ResetUpdateArea();

        if (RightHand.CurrentHandPose == HandPose.SkinnyPouring && RightHand.CurrentHandState == HandState.DRAW)
        {


            if (RightHand.SandSkinnyPouring.Strength > 0.0f)
            {
 
                Bounds bounds = RightHand.HandBound;

                if (IsIntersectXZ(bounds))
                { 
                    Kernel.SkinnyPouring(bounds, CollisionPlane, RightHand.SandSkinnyPouring.PouringCenter, RightHand.SandSkinnyPouring.SandAmount, SandRadius);
                }
            }

            _hands._poseCounter[RightHand.CurrentHandPose] += Time.fixedDeltaTime;
        }
        else if(RightHand.CurrentHandPose == HandPose.ScatterPouring && RightHand.CurrentHandState == HandState.DRAW)
        {
            if (RightHand.SandScatterPouring.Strength > 0.0f)
            {
                Bounds bounds = RightHand.ScatterPouringCenter;
                if (IsIntersectXZ(bounds))
                {
                    Kernel.ScatterPouring(bounds, CollisionPlane, RightHand.SandScatterPouring.SandAmount);
                }

            }
            _hands._poseCounter[RightHand.CurrentHandPose] += Time.fixedDeltaTime;
        }
        else if (RightHand.CurrentHandState == HandState.TOOLS)
        {
            Displacement(Pen);
        }
        else
        {
            Displacement(RightHand);
            Displacement(LeftHand);
        }

        if (Kernel.IsAreaUpdated())
        {
            Kernel.Displacement();
            _hands._poseCounter[RightHand.CurrentHandPose] += Time.fixedDeltaTime;
        }


        

    }

    bool Displacement(ICollision collision)
    {
        Bounds bounds = collision.CollisionBound;
        if (collision.DetectCollision() && IsIntersect(bounds))
        {
            Kernel.CollisionTest(bounds, CollisionPlane, collision);
            Kernel.Sweep(collision.CenterVelocity);
            return true;
        }

        return false;
    }
}
