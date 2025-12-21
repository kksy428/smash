using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class ScreenFadeController : MonoBehaviour
{
    [Header("Fade Target")]
    [Tooltip("카메라 앞 검은 이미지가 달린 오브젝트의 CanvasGroup")]
    public CanvasGroup fadeCanvasGroup;

    [Header("Fade Times (sec)")]
    [Tooltip("0 → 1 까지 어두워지는 시간")]
    public float fadeOutDuration = 0.2f;

    [Tooltip("완전히 까만 상태로 유지할 시간 (이 사이에 텔포 추천)")]
    public float blackHoldDuration = 0.1f;

    [Tooltip("1 → 0 으로 다시 밝아지는 시간")]
    public float fadeInDuration = 0.2f;

    [Header("Events")]
    [Tooltip("화면이 완전히 까매졌을 때(=암전 상태) 호출되는 이벤트")]
    public UnityEvent onBlackScreen;

    bool _isPlaying = false;

    private void Awake()
    {
        if (fadeCanvasGroup != null)
            fadeCanvasGroup.alpha = 0f;   // 시작할 때 무조건 투명
    }

    /// <summary>
    /// 버튼에서 호출할 함수. 페이드 인→암전 유지→페이드 아웃 전체 재생.
    /// </summary>
    public void PlayFade()
    {
        Debug.Log("[ScreenFadeController] PlayFade 호출");

        if (fadeCanvasGroup == null)
        {
            Debug.LogWarning("[ScreenFadeController] fadeCanvasGroup이 비어 있음");
            return;
        }

        if (!_isPlaying)
            StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        _isPlaying = true;

        // 1) 0 → 1 (어둡게)
        yield return StartCoroutine(FadeCanvas(0f, 1f, fadeOutDuration));

        // 2) 완전 암전 상태: 이 타이밍에 텔포 이벤트 호출
        onBlackScreen?.Invoke();

        if (blackHoldDuration > 0f)
            yield return new WaitForSeconds(blackHoldDuration);

        // 3) 1 → 0 (다시 밝게)
        yield return StartCoroutine(FadeCanvas(1f, 0f, fadeInDuration));

        _isPlaying = false;
    }

    private IEnumerator FadeCanvas(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            fadeCanvasGroup.alpha = to;
            yield break;
        }

        float t = 0f;
        fadeCanvasGroup.alpha = from;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            fadeCanvasGroup.alpha = Mathf.Lerp(from, to, lerp);
            yield return null;
        }

        fadeCanvasGroup.alpha = to;
    }
}
