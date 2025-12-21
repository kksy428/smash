using UnityEngine;

public class UIFaceCamera : MonoBehaviour
{
    [Tooltip("바라볼 카메라 (비워두면 MainCamera 자동 사용)")]
    public Transform targetCamera;

    void LateUpdate()
    {
        // 카메라 자동 할당
        if (targetCamera == null && Camera.main != null)
        {
            targetCamera = Camera.main.transform;
        }

        if (targetCamera == null) return;

        // 카메라를 바라보게
        transform.LookAt(targetCamera);

        // UI가 뒤집히는 경우 180도 회전시켜서 앞면 보이게
        transform.Rotate(0f, 180f, 0f);

    }
}
