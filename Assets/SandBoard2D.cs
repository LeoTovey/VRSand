using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SandBoard2D : MonoBehaviour
{
    
    [SerializeField] private ComputeShader SandBoard2DComputeShader;
    [SerializeField] private RenderTexture HeightMap;
    [SerializeField] private Camera Camera;
    [SerializeField] private Collider SandBoardCollider;
    [SerializeField] private Transform SpherePainterTransform;
    [SerializeField] private Renderer SpherePainterRenderer;
    [SerializeField] private Renderer ColorTipRenderer;
    [SerializeField] private Transform SandBoardTransform;
    
    
    public float InitHeight = 0.5f;
    public Color InitColor;
    public Color SandColor;
    
    private Color _currentColor;
    // 0.0 - 1.0f 以长度为1.0
    private float _sandRadius;
    public float SandVelocity;
    
    private int _threadCountX = 8;
    private int _threadCountY = 8;
    
    private int _initKernel = 0;
    private int _skinnyPouringKernel = 0;
    
    private int _threadGroupsX;
    private int _threadGroupsY;
    
    private RaycastHit _hit;
    private Vector2 _mousePos;
    private float _sphereRadius;
    
    private int _collisionMapWidth;
    private int _collisionMapHeight;
    private float _sandBoardAspectRatio;

    
    private SandPixelArea _updateArea = new SandPixelArea(0, 0, 0, 0);
    private SandPixelArea _area = new SandPixelArea(0, 0, 0, 0);

    private float[] _sandCenter = new float[2];
    private float[] _sandAmount = new float[4];
    
    // Start is called before the first frame update
    void Start()
    {
        
        _sandBoardAspectRatio = SandBoardTransform.localScale.y / SandBoardTransform.localScale.x;
        
        _threadGroupsX = Mathf.CeilToInt(HeightMap.width / _threadCountX);
        _threadGroupsY = Mathf.CeilToInt(HeightMap.height / _threadCountY);
        
        _collisionMapWidth = HeightMap.width;
        _collisionMapHeight = HeightMap.height;

        
        
        _initKernel = SandBoard2DComputeShader.FindKernel("Init");
        _skinnyPouringKernel = SandBoard2DComputeShader.FindKernel("SkinnyPouring");
        
        
        SandBoard2DComputeShader.SetTexture(_initKernel, "Height", HeightMap);
        SandBoard2DComputeShader.SetTexture(_skinnyPouringKernel, "Height", HeightMap);
        SandBoard2DComputeShader.SetFloats("InitColor", new float[] { InitColor.r, InitColor.g, InitColor.b });
        SandBoard2DComputeShader.SetFloat("InitHeight", InitHeight);
        
        
        SandBoard2DComputeShader.SetFloat("InverseWidth", 1.0f / HeightMap.width);
        SandBoard2DComputeShader.SetFloat("InverseHeight", 1.0f *_sandBoardAspectRatio / HeightMap.height);
        
        
        SandBoard2DComputeShader.Dispatch(_initKernel, _threadGroupsX, _threadGroupsY, 1);
    }

    // Update is called once per frame
    void Update()
    {
       
        _mousePos = Input.mousePosition;
        Ray ray = Camera.ScreenPointToRay(_mousePos);
        if( Physics.Raycast(Camera.ScreenPointToRay(Input.mousePosition), out _hit) && _hit.collider == SandBoardCollider )
        {
            SpherePainterTransform.position = _hit.point;
        }
        
        float scrollWheelValue = Input.GetAxis("Mouse ScrollWheel");
        if (scrollWheelValue!= 0)
        {
            float newScale = SpherePainterTransform.localScale.x - scrollWheelValue;
            newScale = Mathf.Clamp(newScale, 0.0f, 10.0f);
            SpherePainterTransform.localScale = new Vector3(newScale, newScale, newScale);
            _sphereRadius = SpherePainterTransform.localScale.x / SandBoardTransform.localScale.x;
        }
        
        SwitchColor();
        ColorTipRenderer.material.color = _currentColor;
        _currentColor = SandColor;
        _sandAmount[0] = _currentColor.r;
        _sandAmount[1] = _currentColor.g;
        _sandAmount[2] = _currentColor.b;
        _sandRadius = SpherePainterTransform.localScale.x * 0.5f / SandBoardTransform.localScale.x;
        
        if ( Input.GetMouseButton(0) || Input.GetMouseButton(1) )
        {
            Debug.Log("12");

            Debug.Log("123");
            //_mousePos = _hit.textureCoord;
            Debug.Log(_mousePos);
            _sandAmount[3] = SandVelocity * Time.fixedDeltaTime;
            
            Debug.Log("sand amount: " + _sandAmount[3]);
            Debug.Log("sand velocity: " + SandVelocity);
            Debug.Log("sand fixedDeltaTime: " + Time.deltaTime);
                
            SkinnyPouring(SpherePainterRenderer.bounds, SandBoardTransform, SpherePainterTransform.position, _sandAmount, _sandRadius);
        }
    }
    
    public void World2Pixel(Bounds bounds, Transform planeTransform, ref SandPixelArea pixelArea)
    {
        Vector3 localMin = bounds.min - planeTransform.position;
        Vector3 localMax = bounds.max - planeTransform.position;
        Vector3 collisionPlaneScale = planeTransform.lossyScale;
        int minX = Mathf.Clamp(Mathf.CeilToInt((localMin.x / collisionPlaneScale.x + 0.5f) * (_collisionMapWidth - 1)) - 1, 0, _collisionMapWidth - 1);
        int maxX = Mathf.Clamp(Mathf.CeilToInt((localMax.x / collisionPlaneScale.x + 0.5f) * (_collisionMapWidth - 1)) + 1, 0, _collisionMapWidth - 1);
        int minY = Mathf.Clamp(Mathf.CeilToInt((localMin.z / collisionPlaneScale.y + 0.5f) * (_collisionMapHeight - 1)) - 1, 0, _collisionMapHeight - 1);
        int maxY = Mathf.Clamp(Mathf.CeilToInt((localMax.z / collisionPlaneScale.y + 0.5f) * (_collisionMapHeight - 1)) + 1, 0, _collisionMapHeight - 1);
        pixelArea.SetBounds(minX, minY, maxX, maxY);
    }
    
    // 注意可能超出边界！！！
    public void WorldRelativePos(Vector3 pos, Transform planeTransform, float[] relativePos, bool limited = true)
    {
        Vector3 localCenter = pos - planeTransform.position;
        Vector3 collisionPlaneScale = planeTransform.lossyScale;
        
        
        if (limited)
        {
            relativePos[0] = Mathf.Clamp(localCenter.x / collisionPlaneScale.x + 0.5f , 0.0f, 1.0f);
            relativePos[1] = Mathf.Clamp(localCenter.z / collisionPlaneScale.y + 0.5f, 0.0f, 1.0f) * _sandBoardAspectRatio;
        }
        else
        {
            relativePos[0] = localCenter.x / collisionPlaneScale.x + 0.5f;
            relativePos[1] = (localCenter.z / collisionPlaneScale.y + 0.5f) * _sandBoardAspectRatio;
        }
        
        Debug.Log("local center" + limited +  " " + relativePos[0] );

    }
    
    public void World2Pixel(Vector3 pos, Transform planeTransform, int[] pixelPos)
    {
        Vector3 localCenter = pos - planeTransform.position;
        Vector3 collisionPlaneScale = planeTransform.lossyScale;
        pixelPos[0] = Mathf.Clamp(Mathf.CeilToInt((localCenter.x / collisionPlaneScale.x + 0.5f) * (_collisionMapWidth - 1)) - 1, 0, _collisionMapWidth - 1);
        pixelPos[1] = Mathf.Clamp(Mathf.CeilToInt((localCenter.z / collisionPlaneScale.y + 0.5f) * (_collisionMapHeight - 1)) - 1, 0, _collisionMapHeight - 1);
    }

    
    public void SkinnyPouring(Bounds bounds, Transform planeTransform, Vector3 skinnyPouringCenter, float[] sandAmount, float sandRadius)
    {
        World2Pixel(bounds, planeTransform, ref _area);
        WorldRelativePos(skinnyPouringCenter, planeTransform, _sandCenter, false);
        
        SandBoard2DComputeShader.SetInts("StartId", _area.StartId);
        SandBoard2DComputeShader.SetFloats("SandAmount", sandAmount);
        SandBoard2DComputeShader.SetFloats("SandCenter", _sandCenter);
        SandBoard2DComputeShader.SetFloat("SandRadius", sandRadius);
        
        int threadGroupsX_Dev = Mathf.FloorToInt(_area.Width / _threadCountX);
        int threadGroupsY_Dev = Mathf.FloorToInt(_area.Height / _threadCountY);
        SandBoard2DComputeShader.Dispatch(_skinnyPouringKernel, threadGroupsX_Dev, threadGroupsY_Dev, 1);
    }

    private void FixedUpdate()
    {

    }

    private void SwitchColor()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _currentColor = Color.red;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _currentColor = Color.green;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _currentColor = Color.blue;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _currentColor = Color.yellow;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _currentColor = Color.magenta;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _currentColor = Color.cyan;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            _currentColor = Color.black;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            _currentColor = Color.white;
        }
    }
}
