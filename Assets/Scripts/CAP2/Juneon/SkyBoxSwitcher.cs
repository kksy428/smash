using UnityEngine;

public class SkyboxSwitcher : MonoBehaviour
{
    [Header("Skybox Materials")]
    public Material studioSkybox;
    public Material[] outdoorSkyboxes; // 외부조명 HDRI 배열

    [Header("Scene Objects")]
    public GameObject[] ceilingObjects; // 단수 -> 배열

    [Header("Reflection")]
    public ReflectionProbe reflectionProbe; // Inspector에서 할당

    private void RefreshReflection()
    {
        if (reflectionProbe != null)
            reflectionProbe.RenderProbe();
        else
            Debug.LogWarning("Reflection Probe가 할당되지 않았습니다.");
    }
    
    /// <summary>
    /// "스튜디오조명" 버튼에 연결
    /// </summary>
    public void OnStudioLighting()
    {
        SetCeiling(true);
        ApplySkybox(studioSkybox);
    }

    /// <summary>
    /// "외부조명" 버튼에 연결 - On Click 인자로 인덱스 전달 (0, 1, 2...)
    /// </summary>
    public void OnOutdoorLighting(int index)
    {
        if (index < 0 || index >= outdoorSkyboxes.Length)
        {
            Debug.LogWarning($"outdoorSkyboxes 인덱스 {index}가 범위를 벗어났습니다.");
            return;
        }

        SetCeiling(false);
        ApplySkybox(outdoorSkyboxes[index]);
    }

    private void SetCeiling(bool isActive)
    {
        if (ceilingObjects == null || ceilingObjects.Length == 0)
        {
            Debug.LogWarning("Ceiling Objects가 할당되지 않았습니다.");
            return;
        }

        foreach (var ceiling in ceilingObjects)
        {
            if (ceiling != null)
                ceiling.SetActive(isActive);
        }
    }

    private void ApplySkybox(Material skybox)
    {
        if (skybox != null)
        {
            RenderSettings.skybox = skybox;
            DynamicGI.UpdateEnvironment();
            RefreshReflection(); // 추가
        }
        else
        {
            Debug.LogWarning("Skybox Material이 할당되지 않았습니다.");
        }
    }
}