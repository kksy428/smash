using UnityEngine;

public class RotationSync : MonoBehaviour
{
    [Header("회전을 따라갈 대상")]
    public Transform target;

    void LateUpdate()
    {
        transform.rotation = target.rotation;
    }
}