using UnityEngine;

public interface IUIEventSystem
{
    public void ProcessPointEvent(Vector3 point, Vector3 fingerVelocity);
}