using UnityEngine;
using UnityEngine.Events;

public class AxisRotationWatcher : MonoBehaviour
{
    [Header("Target Object")]
    public Transform target;

    public enum Axis { X, Y, Z }

    [Header("Check Axis Rotation")]
    public Axis axisToCheck = Axis.Y;

    [Header("Target Rotation Value")]
    public float targetRotationValue = 0f;

    [Tooltip("목표 각도 주변 허용 범위 (도 단위)")]
    public float tolerance = 1f;

    [Header("Options")]
    [Tooltip("true면 평생 한 번만 실행, false면 각도 범위에 들어올 때마다 실행")]
    public bool triggerOnce = false;

    [Header("Event")]
    public UnityEvent onReachTargetRotation;

    // 내부 상태
    private bool hasTriggered = false;  // triggerOnce용
    private bool isInRange = false;     // 현재 각도 범위 안에 있는지

    void Update()
    {
        if (target == null) return;

        float current = 0f;

        switch (axisToCheck)
        {
            case Axis.X: current = target.eulerAngles.x; break;
            case Axis.Y: current = target.eulerAngles.y; break;
            case Axis.Z: current = target.eulerAngles.z; break;
        }

        float delta = Mathf.Abs(Mathf.DeltaAngle(current, targetRotationValue));
        bool nowInRange = delta < tolerance;

        // 각도 범위 "밖 → 안" 으로 들어온 순간에만 실행
        if (nowInRange && !isInRange)
        {
            if (!triggerOnce || !hasTriggered)
            {
                onReachTargetRotation?.Invoke();
                hasTriggered = true;
            }
        }

        // 상태 업데이트
        isInRange = nowInRange;
    }
}
