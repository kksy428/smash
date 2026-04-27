using UnityEngine;

public class CopyRotation : MonoBehaviour
{
    public Transform controller;
    public float followSpeed = 5f; // 숫자를 낮추면 더 느리게 따라옴

    void Update()
    {
        if (controller == null) return;

        // 목표 회전 (y축만)
        Vector3 targetEuler = transform.eulerAngles;
        targetEuler.y = controller.eulerAngles.y;

        // 자연스럽게 따라가기 (딜레이 효과)
        Quaternion targetRot = Quaternion.Euler(targetEuler);

        transform.rotation = Quaternion.Lerp(
            transform.rotation,   // 현재 회전
            targetRot,            // 목표 회전
            Time.deltaTime * followSpeed
        );
    }
}