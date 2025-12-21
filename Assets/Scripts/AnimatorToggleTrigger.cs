using UnityEngine;

public class AnimatorToggleTrigger : MonoBehaviour
{
    [Header("토글할 애니메이터")]
    public Animator targetAnimator;

    [Header("트리거 이름")]
    public string openTriggerName = "Open";
    public string closeTriggerName = "Close";

    [Header("시작 상태 (Play 모드 시작 시)")]
    public bool startOpened = true; // None이 Open 모양이면 true

    bool _isOpened;

    void Awake()
    {
        _isOpened = startOpened;
    }

    /// <summary>
    /// Meta Interactable Event Wrapper의 WhenSelect()에 연결할 함수
    /// </summary>
    public void ToggleAnimation()
    {
        if (!targetAnimator) return;

        if (_isOpened)
        {
            // 열려있으면 -> 닫기
            targetAnimator.ResetTrigger(openTriggerName);
            targetAnimator.SetTrigger(closeTriggerName);
        }
        else
        {
            // 닫혀있으면 -> 열기
            targetAnimator.ResetTrigger(closeTriggerName);
            targetAnimator.SetTrigger(openTriggerName);
        }

        _isOpened = !_isOpened;
    }
}
