using UnityEngine;
using System.Collections;

public class ActivateToggleAfterDelay : MonoBehaviour
{
    [Tooltip("몇 초 뒤에 다시 활성화할지")]
    public float delay = 1f;

    [Tooltip("비활성화 후 다시 활성화할 게임오브젝트")]
    public GameObject targetObject;

    private bool _isRunning = false;

    /// <summary>
    /// OnBlackScreen 등에서 호출할 함수
    /// </summary>
    public void StartToggle()
    {
        if (targetObject == null)
        {
            Debug.LogWarning("[ActivateToggleAfterDelay] targetObject가 비어 있음");
            return;
        }

        if (!_isRunning)
            StartCoroutine(ToggleRoutine());
    }

    private IEnumerator ToggleRoutine()
    {
        _isRunning = true;

        // 1) 즉시 비활성화
        targetObject.SetActive(false);

        // 2) 입력한 시간만큼 기다림
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        // 3) 다시 활성화
        targetObject.SetActive(true);

        _isRunning = false;
    }
}
