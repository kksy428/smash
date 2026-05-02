using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class ToggleDrivenAnimator : MonoBehaviour
{
    [Header("References")]
    public Animator animator;

    [Header("Animator Parameters")]
    public string openTrigger = "Open";
    public string closeTrigger = "Close";

    [Header("Initial State (when no Toggle linked)")]
    public bool startsOpen = false;

    [Header("UI Indicator (Optional)")]
    public Image indicatorImage;
    public Color openColor = Color.green;
    public Color closedColor = Color.red;

    [Header("Optional Toggle Link")]
    public Toggle uiToggle;              // 이 파츠를 제어하는 Toggle (없으면 비워둬도 됨)

    [Header("Immediate Close Settings")]
    public string closedStateName = "None";  // 완전히 닫힌 포즈를 가진 Animator 상태 이름

    bool isOpen;
    bool isAnimating;
    bool hasPending;
    bool pendingState;

    void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Start()
    {
        // 시작 상태를 Toggle 또는 startsOpen 기준으로 맞춤
        if (uiToggle != null)
            isOpen = uiToggle.isOn;
        else
            isOpen = startsOpen;

        UpdateIndicator();
    }

    // ─────────────────────────────
    // Toggle 이벤트
    // ─────────────────────────────
    public void OnToggleChanged(bool isOn)
    {
        Debug.Log($"[{name}] OnToggleChanged called. isOn={isOn}, isOpen={isOpen}, isAnimating={isAnimating}");

        // 값이 안 바뀌었으면 무시
        if (isOn == isOpen && !isAnimating)
            return;

        // 애니 도는 중이면 큐에만 넣어둠
        if (isAnimating)
        {
            pendingState = isOn;
            hasPending = true;
            Debug.Log($"[{name}] Currently animating. Queue pendingState={pendingState}");
            return;
        }

        PlayAnimation(isOn);
    }

    // 열기 / 닫기 애니 실행
    void PlayAnimation(bool open)
    {
        Debug.Log($"[{name}] PlayAnimation(open={open})");

        if (animator == null)
        {
            Debug.LogWarning($"[{name}] Animator is NULL!");
            return;
        }

        isAnimating = true;
        isOpen = open;
        UpdateIndicator();

        animator.ResetTrigger(openTrigger);
        animator.ResetTrigger(closeTrigger);

        if (open && !string.IsNullOrEmpty(openTrigger))
            animator.SetTrigger(openTrigger);
        else if (!open && !string.IsNullOrEmpty(closeTrigger))
            animator.SetTrigger(closeTrigger);
    }

    // Open / Close 클립 끝에서 Animation Event로 호출
    public void OnAnimFinished()
    {
        Debug.Log($"[{name}] OnAnimFinished called. hasPending={hasPending}");

        isAnimating = false;

        if (hasPending)
        {
            bool next = pendingState;
            hasPending = false;

            if (next != isOpen)
                PlayAnimation(next);
        }
    }

    void UpdateIndicator()
    {
        if (indicatorImage == null) return;
        indicatorImage.color = isOpen ? openColor : closedColor;
    }

    // ─────────────────────────────
    // ★ 애니 타면서 닫기 (전역 Close용)
    // ─────────────────────────────
    public void ForceClose()
    {
        Debug.Log($"[{name}] ForceClose()");

        if (animator == null)
            return;

        // 토글이 연결돼 있으면, 조용히 OFF로 동기화 (이벤트 X)
        if (uiToggle != null)
            uiToggle.SetIsOnWithoutNotify(false);

        hasPending = false;          // 강제 닫기면 큐 비움

        // 이미 닫혀 있고, 애니도 안 도는 상태면 그냥 무시
        if (!isAnimating && !isOpen)
            return;

        // 그냥 "false로 눌렀다"라고 생각하고 재사용
        PlayAnimation(false);
    }

    // ─────────────────────────────
    // ★ 즉시 닫기 (스냅 리셋)
    // ─────────────────────────────
    public void ForceCloseImmediate()
    {
        Debug.Log($"[{name}] ForceCloseImmediate()");

        isOpen = false;
        isAnimating = false;
        hasPending = false;

        // 토글 상태도 OFF로 맞추되, 이벤트는 안 날리기
        if (uiToggle != null)
            uiToggle.SetIsOnWithoutNotify(false);

        UpdateIndicator();

        if (animator != null && !string.IsNullOrEmpty(closedStateName))
        {
            // layer 0, 해당 스테이트의 시작 포즈(완전 닫힘)로 스냅
            animator.Play(closedStateName, 0, 0f);
        }
    }
}
