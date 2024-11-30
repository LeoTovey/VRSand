using UnityEngine;
using Unity.Mathematics;


public interface ICollision
{
    Vector3 Movement { get;}
    Bounds CollisionBound { get; }
    public abstract bool DetectCollision();
    public abstract void ClearMovement();

}
