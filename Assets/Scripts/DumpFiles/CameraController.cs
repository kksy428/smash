using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    float yRotation = 0f;

    void Start()
    {
        // 현재 씬에서 설정한 카메라 각도를 기준으로 시작
        Vector3 euler = transform.rotation.eulerAngles;
        xRotation = euler.x;
        yRotation = euler.y;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;                  // 상하
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        yRotation += mouseX;                  // 좌우

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}