using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;


public class SandSimulationKernel : MonoBehaviour 
{
    [SerializeField] private ComputeShader _sandSimulationComputeShader;
    [SerializeField] private RenderTexture _collisionRT;
    [SerializeField] private RenderTexture _heightRT;
    [SerializeField] private RenderTexture _displacementXRT;
    [SerializeField] private RenderTexture _displacementYRT;
    [SerializeField] private RenderTexture _displacementRT;
    [SerializeField] private Color _initColor;
    [SerializeField] private float _initHeight;
    
    private int _initKernel = 0;
    private int _collisionTestKernel = 0;
    private int _displacementVerticalKernel = 0;
    private int _displacementHorizontalKernel = 0;
    private int _displacementKernel = 0;
    private int _erosionKernel = 0;
    private int _skinnyPouringKernel = 0;
    private int _scatterPouringKernel = 0;

    private int _threadGroupsX;
    private int _threadGroupsY;
    private int _threadCountX = 8;
    private int _threadCountY = 8;

    private float[] _displacementRatio = new float[2];
    private int[] _skinnyCenter = new int[2];
    private float _epsilon = 0.0001f;
    private int _maxErosionRangeX = 20;
    private int _maxErosionRangeY = 20;

    private int _collisionMapWidth;
    private int _collisionMapHeight;
    
    

    
    private void Start()
    {
        InitComputeShaderKernel();
        InitThread();
        Init();
    }

    
    void InitThread()
    {
        _threadGroupsX = Mathf.CeilToInt(_heightRT.width / _threadCountX);
        _threadGroupsY = Mathf.CeilToInt(_heightRT.height / _threadCountY);
    }

    public void Init()
    {
        _sandSimulationComputeShader.Dispatch(_initKernel, _threadGroupsX, _threadGroupsY, 1);
    }


    public void CollisionTest(RTPixelAABB area, Vector2 displacementRatio)
    {
        _displacementRatio[0] = displacementRatio[0];
        _displacementRatio[1] = displacementRatio[1];
        _threadGroupsX = Mathf.FloorToInt(area.Width / _threadCountX);
        _threadGroupsY = Mathf.FloorToInt(area.Height / _threadCountY);
        
        _sandSimulationComputeShader.SetInts("StartId", area.StartId);
        _sandSimulationComputeShader.SetFloats("DisplacementRatio", _displacementRatio);
        _sandSimulationComputeShader.Dispatch(_collisionTestKernel, _threadGroupsX, _threadGroupsY, 1);
    }


    public void ScatterPouring(RTPixelAABB area,  float[] sandAmount)
    {
        // _sandSimulationComputeShader.SetInts("SandCenter", area.Center);
        // _sandSimulationComputeShader.SetFloat("SandRadius", 0.5f * (area.MaxY - area.MinY));
        // _sandSimulationComputeShader.SetInts("StartId", area.StartId);
        // _sandSimulationComputeShader.SetFloats("SandAmount", sandAmount);
        //
        // int threadGroupsX_Dev = Mathf.FloorToInt(area.Width / _threadCountX);
        // int threadGroupsY_Dev = Mathf.FloorToInt(area.Height / _threadCountY);
        // _sandSimulationComputeShader.Dispatch(_scatterPouringKernel, threadGroupsX_Dev, threadGroupsY_Dev, 1);
    }

    public void SkinnyPouring(Bounds bounds, Transform planeTransform, Vector3 skinnyPouringCenter, float[] sandAmount, float sandRadius)
    {

        // World2Pixel(bounds, planeTransform, ref _area);
        // World2Pixel(skinnyPouringCenter, planeTransform, _skinnyCenter);
        //
        // _sandSimulationComputeShader.SetInts("StartId", _area.StartId);
        // _sandSimulationComputeShader.SetFloat("SandRadius", sandRadius);
        // _sandSimulationComputeShader.SetFloats("SandAmount", sandAmount);
        // _sandSimulationComputeShader.SetInts("SandCenter", _skinnyCenter);
        //
        // int threadGroupsX_Dev = Mathf.FloorToInt(_area.Width / ThreadCountX);
        // int threadGroupsY_Dev = Mathf.FloorToInt(_area.Height / ThreadCountY);
        // _sandSimulationComputeShader.Dispatch(_skinnyPouringKernel, threadGroupsX_Dev, threadGroupsY_Dev, 1);
    }


    public void Sweep(Vector3 direction)
    {
        // int startX, endX, stepX;
        // int startY, endY, stepY;
        //
        // int threadGroupsX_Dev = Mathf.FloorToInt(_updateArea.Width / _threadCountX);
        // int threadGroupsY_Dev = Mathf.FloorToInt(_updateArea.Height / _threadCountY);
        //
        // if (direction.x < 0)
        // {
        //     startX = _updateArea.MaxX - 1;
        //     endX = _updateArea.MinX;
        //     stepX = -1;
        // }
        // else
        // {
        //     startX = _updateArea.MinX + 1;
        //     endX = _updateArea.MaxX;
        //     stepX = 1;
        // }
        //
        // if (direction.z < 0)
        // {
        //     startY = _updateArea.MaxY - 1;
        //     endY = _updateArea.MinY;
        //     stepY = -1;
        // }
        // else
        // {
        //     startY = _updateArea.MinY + 1;
        //     endY = _updateArea.MaxY;
        //     stepY = 1;
        // }
        //
        // int nextDisplacementRaw = 0;
        // for (int i = startY; i != endY; i += stepY)
        // {
        //     nextDisplacementRaw = Mathf.Min(_collisionMapHeight - 1, Mathf.Max(i + stepY * _maxErosionRangeY, 0));
        //     _sandSimulationComputeShader.SetInt("DisplacementRaw", i);
        //     _sandSimulationComputeShader.SetInt("NextDisplacementRaw", nextDisplacementRaw);
        //     _sandSimulationComputeShader.Dispatch(_displacementVerticalKernel, threadGroupsX_Dev, 1, 1);
        // }
        //
        //
        // _updateArea.ExtendY(nextDisplacementRaw);
        // _updateArea.ExtendY(startY);
        //
        // int nextDisplacementColumn = 0;
        // for (int i = startX; i != endX; i += stepX)
        // {
        //     nextDisplacementColumn = Mathf.Min(_collisionMapWidth - 1, Mathf.Max(i + stepX * _maxErosionRangeX, 0));
        //     _sandSimulationComputeShader.SetInt("DisplacementColumn", i);
        //     _sandSimulationComputeShader.SetInt("NextDisplacementColumn", nextDisplacementColumn);
        //     _sandSimulationComputeShader.Dispatch(_displacementHorizontalKernel, 1, threadGroupsY_Dev, 1);
        // }
        // _updateArea.ExtendX(nextDisplacementColumn);
        // _updateArea.ExtendX(startX);
        //
        //


    }

    public void Displacement()
    {
        // _sandSimulationComputeShader.SetInts("StartId", _updateArea.StartId);
        // int threadGroupsX_Dev = Mathf.FloorToInt(_updateArea.Width / ThreadCountX);
        // int threadGroupsY_Dev = Mathf.FloorToInt(_updateArea.Height / ThreadCountY);
        // _sandSimulationComputeShader.Dispatch(_displacementKernel, threadGroupsX_Dev, threadGroupsY_Dev, 1);
    }




    void InitComputeShaderKernel()
    {
        _initKernel = _sandSimulationComputeShader.FindKernel("Init");
        _collisionTestKernel = _sandSimulationComputeShader.FindKernel("CollisionTest");
        _displacementVerticalKernel = _sandSimulationComputeShader.FindKernel("DisplacementVertical");
        _displacementHorizontalKernel = _sandSimulationComputeShader.FindKernel("DisplacementHorizontal");
        _displacementKernel = _sandSimulationComputeShader.FindKernel("Displacement");
        _erosionKernel = _sandSimulationComputeShader.FindKernel("Erosion");
        _skinnyPouringKernel = _sandSimulationComputeShader.FindKernel("SkinnyPouring");
        _scatterPouringKernel = _sandSimulationComputeShader.FindKernel("ScatterPouring");

        // Init
        _sandSimulationComputeShader.SetTexture(_initKernel, "HeightRT", _heightRT);
        _sandSimulationComputeShader.SetTexture(_initKernel, "DisplacementXRT", _displacementXRT);
        _sandSimulationComputeShader.SetTexture(_initKernel, "DisplacementYRT", _displacementYRT);



        // CollisionTest
        _sandSimulationComputeShader.SetTexture(_collisionTestKernel, "HeightRT", _heightRT);
        _sandSimulationComputeShader.SetTexture(_collisionTestKernel, "DisplacementXRT", _displacementXRT);
        _sandSimulationComputeShader.SetTexture(_collisionTestKernel, "DisplacementYRT", _displacementYRT);
        _sandSimulationComputeShader.SetTexture(_collisionTestKernel, "CollisionRT", _collisionRT);

        // DisplacementVertical
        // _sandSimulationComputeShader.SetTexture(_displacementVerticalKernel, "DisplacementHeightY", _displacementYRT);
        // _sandSimulationComputeShader.SetTexture(_displacementVerticalKernel, "Collision", _collisionRT);

        // DisplacementHorizontal
        // _sandSimulationComputeShader.SetTexture(_displacementHorizontalKernel, "DisplacementHeightX", _displacementXRT);
        // _sandSimulationComputeShader.SetTexture(_displacementHorizontalKernel, "Collision", _collisionRT);

        // Displacement
        // _sandSimulationComputeShader.SetTexture(_displacementKernel, "Height", _heightRT);
        // _sandSimulationComputeShader.SetTexture(_displacementKernel, "DisplacementHeightX", _displacementXRT);
        // _sandSimulationComputeShader.SetTexture(_displacementKernel, "DisplacementHeightY", _displacementYRT);


        // Sand Pouring
        // _sandSimulationComputeShader.SetTexture(_skinnyPouringKernel, "Height", _heightRT);
        // _sandSimulationComputeShader.SetTexture(_scatterPouringKernel, "Height", _heightRT);
        // _sandSimulationComputeShader.SetTexture(_erosionKernel, "Height", _heightRT);


        // params
        _sandSimulationComputeShader.SetFloat("InitHeight", _initHeight);
        _sandSimulationComputeShader.SetFloats("InitColor", new float[] { _initColor.r, _initColor.g, _initColor.b });
        _sandSimulationComputeShader.SetInts("CollisionRTSize", new int[] { _collisionRT.width, _collisionRT.height });
        _sandSimulationComputeShader.SetInts("HeightRTSize", new int[] { _heightRT.width, _heightRT.height });
    }
}