using UnityEngine;

public class HeightAdjuster : MonoBehaviour
{
    [Header("Camera Height Root (카메라 부모 오브젝트)")]
    public Transform cameraHeightManager;

    [Header("UI Height Root (UI 전체를 담은 오브젝트)")]
    public Transform uiHeightManager;

    [Header("UI Follow Smooth Speed")]
    public float uiSmoothSpeed = 5f;

    [Header("Camera Offsets")]
    public float lowCameraOffset = -0.3f;
    public float midCameraOffset = 0.0f;
    public float highCameraOffset = 0.3f;

    [Header("UI Offsets")]
    public float lowUiOffset = -0.3f;
    public float midUiOffset = 0.0f;
    public float highUiOffset = 0.3f;

    float _cameraBaseY;
    float _uiBaseY;

    float _cameraOffset;
    float _uiOffset;

    void Start()
    {
        if (cameraHeightManager)
            _cameraBaseY = cameraHeightManager.localPosition.y;

        if (uiHeightManager)
            _uiBaseY = uiHeightManager.localPosition.y;

        SetMid(); // 기본값
    }

    public void SetLow()
    {
        _cameraOffset = lowCameraOffset;
        _uiOffset = lowUiOffset;
        ApplyCamera();
    }

    public void SetMid()
    {
        _cameraOffset = midCameraOffset;
        _uiOffset = midUiOffset;
        ApplyCamera();
    }

    public void SetHigh()
    {
        _cameraOffset = highCameraOffset;
        _uiOffset = highUiOffset;
        ApplyCamera();
    }

    void ApplyCamera()
    {
        if (!cameraHeightManager) return;

        Vector3 pos = cameraHeightManager.localPosition;
        pos.y = _cameraBaseY + _cameraOffset;
        cameraHeightManager.localPosition = pos;
    }

    void Update()
    {
        if (!uiHeightManager) return;

        float targetY = _uiBaseY + _uiOffset;
        Vector3 pos = uiHeightManager.localPosition;
        pos.y = Mathf.Lerp(pos.y, targetY, uiSmoothSpeed * Time.deltaTime);
        uiHeightManager.localPosition = pos;
    }
}
