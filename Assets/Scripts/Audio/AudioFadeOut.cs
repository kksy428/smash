using UnityEngine;
using System.Collections;

public class AudioFadeOut : MonoBehaviour
{
    public AudioSource audioSource;
    public float fadeDuration = 1.0f; // 몇 초 동안 페이드아웃 할지

    void Start()
    {
        StartCoroutine(PlayWithFadeOut());
    }

    IEnumerator PlayWithFadeOut()
    {
        audioSource.volume = 1f;
        audioSource.Play();

        float clipLength = audioSource.clip.length;

        // fadeDuration 만큼 남았을 때까지 기다림
        yield return new WaitForSeconds(clipLength - fadeDuration);

        // Fade-Out 시작
        float startVolume = audioSource.volume;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // 다음 재생 대비 초기화
    }
}
