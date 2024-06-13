using UnityEngine;

public interface IMovable
{
    void Move(Vector3 targetPosition);
    void MoveInFormation(Vector3 targetPosition);
}