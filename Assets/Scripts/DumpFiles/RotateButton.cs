using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateButton : MonoBehaviour
{
    [Header("회전 설정")]
    [Tooltip("회전시킬 오브젝트")]
    public Transform targetObject;
    
    [Tooltip("회전 속도 (도/초)")]
    public float rotationSpeed = 90f;
    
    [Tooltip("회전 축")]
    public Vector3 rotationAxis = Vector3.up;
    
    private bool isRotating = false;
    
    // 버튼을 눌렀을 때 호출할 함수
    public void OnButtonPressed()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("RotateButton: 회전시킬 오브젝트가 설정되지 않았습니다!");
            return;
        }
        
        isRotating = !isRotating;
    }
    
    // 회전 시작
    public void StartRotation()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("RotateButton: 회전시킬 오브젝트가 설정되지 않았습니다!");
            return;
        }
        
        isRotating = true;
    }
    
    // 회전 중지
    public void StopRotation()
    {
        isRotating = false;
    }

    void Update()
    {
        if (isRotating && targetObject != null)
        {
            targetObject.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
        }
    }
}
