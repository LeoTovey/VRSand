using Modularify.LoadingBars3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;



public class Pen : MonoBehaviour, ICollision
{
    public Vector3 CenterVelocity => _velocity;
    public Bounds CollisionBound => _penMesh.bounds;

    private MeshRenderer _penMesh;

    [SerializeField] private LoadingBarStraight _loading;

    private Vector3 _velocity ;

    private void Awake()
    {
        _penMesh = GetComponent<MeshRenderer>();
    }

    public void SetVelocity(Vector3 velocity) => _velocity = velocity;

    public bool DetectCollision() => true;
}