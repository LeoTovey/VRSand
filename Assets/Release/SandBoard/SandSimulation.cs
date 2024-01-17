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


    public Hand[] Hands;
    private List<ICollision> _collisions = new List<ICollision>();


    public Pen Pen;
    public TextMeshProUGUI testMesh;

    private int collisionMapWidth => Kernel.MapWidth;
    private int collisionMapHeight => Kernel.MapHeight;

    public RawImage ColorHandleImage;
    public Color SandColor;
    public ParticleSystem SkinnySand;

    public Transform CollisionPlane;

    [SerializeField] private SandScatterPouring _sandScatterPouring;

    // for simulation
    public float InitHeight = 0.5f;
    public float MaxHeight = 1.0f;
    public Color InitColor;

    private float MaxStrength = 0.06f;
    private float MinStrength = 0.02f;
    private float SkinnySandVelocity = 0.5f;
    private float ScatterSandVelocity = 0.04f;
    private int SandRadius = 5;

    private Vector3 _collisionPlaneScale => CollisionPlane.lossyScale;
    private Vector3 _collisionPlaneCenter => CollisionPlane.position;
    private float _halfWidth => _collisionPlaneScale.x * 0.5f;
    private float _halfHeight => _collisionPlaneScale.y * 0.5f;
    private float _planeMinX => _collisionPlaneCenter.x - _halfWidth;
    private float _planeMinZ => _collisionPlaneCenter.z - _halfHeight;
    private float _planeMaxX => _collisionPlaneCenter.x + _halfWidth;
    private float _planeMaxZ => _collisionPlaneCenter.z + _halfHeight;

    private float[] _sandAmount = new float[4];


    private void Awake()
    {
        Application.targetFrameRate = -1;
    }

    void Start()
    {
        _collisions.Add(Hands[0]);
        _collisions.Add(Hands[1]);

        Hands[1].BindHandPoseEndCallback(HandPose.SkinnyPouring, () =>
        {
            SkinnySand.gameObject.SetActive(false);
        });


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



        if (Hands[1].CurrentHandPose == HandPose.SkinnyPouring && Hands[1].CurrentHandStatus == HandStatus.Draw)
        {
            float Strength = Hands[1].Strength;
            float SandStrength = Mathf.InverseLerp(MinStrength, MaxStrength, Strength);
            Vector3 skinnyPouringCenter = Hands[1].SkinnyPouringCenter;
            SkinnySand.gameObject.transform.position = skinnyPouringCenter;

            var main = SkinnySand.main;
            main.simulationSpeed = SandStrength;
            //Color color = ColorHandleImage.color;
            Color color = SandColor;
            color.a = SandStrength;
            main.startColor = color;

            if (SandStrength > 0.0f)
            {
                SkinnySand.gameObject.SetActive(true);
                Bounds bounds = Hands[1].HandBound;

                if (IsIntersectXZ(bounds))
                { 

                    _sandAmount[3] = (color.r + color.g + color.b) * SandStrength * SkinnySandVelocity;
                    _sandAmount[0] = color.r * SandStrength * SkinnySandVelocity;
                    _sandAmount[1] = color.g * SandStrength * SkinnySandVelocity;
                    _sandAmount[2] = color.b * SandStrength * SkinnySandVelocity;

                    Kernel.SkinnyPouring(bounds, CollisionPlane, skinnyPouringCenter, _sandAmount, SandRadius);

                }
            }
            else
            {
                SkinnySand.gameObject.SetActive(false);
            }
        }
        else if(Hands[1].CurrentHandPose == HandPose.ScatterPouring && Hands[1].CurrentHandStatus == HandStatus.Draw)
        {

            Color color = SandColor;
            color.a = _sandScatterPouring.Strenth * 0.2f;
            if (_sandScatterPouring.Strenth > 0.0f)
            {
                Bounds bounds = Hands[1].ScatterPouringCenter;
                if (IsIntersectXZ(bounds))
                {
                    _sandAmount[3] = (color.r + color.g + color.b) * _sandScatterPouring.Strenth * ScatterSandVelocity;
                    _sandAmount[0] = color.r * _sandScatterPouring.Strenth * ScatterSandVelocity;
                    _sandAmount[1] = color.g * _sandScatterPouring.Strenth * ScatterSandVelocity;
                    _sandAmount[2] = color.b * _sandScatterPouring.Strenth * ScatterSandVelocity;

                    Kernel.ScatterPouring(bounds, CollisionPlane, _sandAmount);

                }

            }
        }
        else if (Hands[1].CurrentHandStatus == HandStatus.Tools)
        {
            Displacement(Pen);
            
        }
        else
        {
            for (int c = 0; c < Hands.Length; c++)
            {
                Displacement(Hands[c]);
            }
            

        }

        if (Kernel.IsAreaUpdated())
        {
            Kernel.Displacement();
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
