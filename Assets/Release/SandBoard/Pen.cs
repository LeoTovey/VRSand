using Modularify.LoadingBars3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



public class Pen : MonoBehaviour, ICollision
{
    public Bounds CollisionBound => _penMesh.bounds;
    public Vector3 Movement => Vector3.zero;

    private MeshRenderer _penMesh;

    [SerializeField] private LoadingBarStraight _loading;

    private Vector3 _movement;
    private Vector3 _lastPosition;

    private void Awake()
    {
        _penMesh = GetComponent<MeshRenderer>();
        _lastPosition = transform.position;
    }

    private void Update()
    {
        _movement += transform.position - _lastPosition;
        _lastPosition = transform.position;
    }
    
    public bool DetectCollision() => true;
    public void ClearMovement() => _movement = Vector3.zero;
}