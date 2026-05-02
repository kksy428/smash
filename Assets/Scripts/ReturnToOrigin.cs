using UnityEngine;
using Oculus.Interaction;
using System.Collections;

public class ReturnToOrigin : MonoBehaviour
{
    [Header("Position Return")]
    [SerializeField] float returnTime = 0.4f;

    [Header("Rotation Axis Lock")]
    [SerializeField] bool keepXRotation = false;
    [SerializeField] bool keepYRotation = false;
    [SerializeField] bool keepZRotation = false;

    [Header("UX Options")]
    [Tooltip("true = 돌아가는 중에 잡으면 즉시 멈추고 손에 맡김\nfalse = 돌아가는 동안은 아예 못 잡음")]
    [SerializeField] bool interruptOnGrab = true;

    private Grabbable _grabbable;
    private Vector3 _originPos;
    private Quaternion _originRot;

    private Coroutine _routine;
    private bool _isReturning = false;

    void Awake()
    {
        _grabbable = GetComponent<Grabbable>();

        _originPos = transform.position;
        _originRot = transform.rotation;
    }

    void OnEnable()
    {
        _grabbable.WhenPointerEventRaised += HandlePointerEvent;
    }

    void OnDisable()
    {
        _grabbable.WhenPointerEventRaised -= HandlePointerEvent;
    }

    private void HandlePointerEvent(PointerEvent evt)
    {
        // 잡혔을 때
        if (evt.Type == PointerEventType.Select)
        {
            if (_isReturning)
            {
                if (interruptOnGrab)
                {
                    StopReturnRoutine();
                }
                // interruptOnGrab == false 모드는 그대로 두면 됨
            }
        }
        // 놓였을 때
        else if (evt.Type == PointerEventType.Unselect)
        {
            // 진짜로 아무 손도 안 잡고 있을 때만 원점 복귀 시작
            if (_grabbable.SelectingPointsCount == 0)
            {
                StartReturnRoutine();
            }
            // 아직 다른 손이 잡고 있으면 아무 것도 안 함
        }
    }

    void StartReturnRoutine()
    {
        if (_routine != null)
            StopCoroutine(_routine);

        _routine = StartCoroutine(ReturnRoutine());
    }

    void StopReturnRoutine()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }
        _isReturning = false;

        // interruptOnGrab == false 모드에서,
        // 혹시 비활성화돼 있으면 다시 켜주기 (안전망)
        if (!interruptOnGrab && !_grabbable.enabled)
        {
            _grabbable.enabled = true;
        }
    }

    IEnumerator ReturnRoutine()
    {
        _isReturning = true;

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        Vector3 startEuler = startRot.eulerAngles;
        Vector3 originEuler = _originRot.eulerAngles;

        // "돌아가는 동안 못 잡기" 모드면 Grabbable 잠시 꺼둠
        if (!interruptOnGrab)
        {
            _grabbable.enabled = false;
        }

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / returnTime;

            // 위치 Lerp
            transform.position = Vector3.Lerp(startPos, _originPos, t);

            // 축별 회전 제어
            float x = keepXRotation ? startEuler.x : Mathf.LerpAngle(startEuler.x, originEuler.x, t);
            float y = keepYRotation ? startEuler.y : Mathf.LerpAngle(startEuler.y, originEuler.y, t);
            float z = keepZRotation ? startEuler.z : Mathf.LerpAngle(startEuler.z, originEuler.z, t);

            transform.rotation = Quaternion.Euler(x, y, z);

            yield return null;
        }

        _isReturning = false;
        _routine = null;

        // 못 잡기 모드였다면 다시 켜줌
        if (!interruptOnGrab)
        {
            _grabbable.enabled = true;
        }
    }
}
