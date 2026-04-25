using UnityEngine;

public class CarPaintTarget : MonoBehaviour
{
    [Header("페인트 Renderer 목록")]
    public Renderer[] paintRenderers;

    [Header("이 차종 전용 색상 머테리얼 풀")]
    public Material[] colorMaterials;

    public void ApplyColor(int colorIndex)
    {
        if (colorIndex < 0 || colorIndex >= colorMaterials.Length)
        {
            Debug.LogWarning("이 차종에 해당 색상이 없어요!");
            return;
        }

        Material newMaterial = colorMaterials[colorIndex];

        foreach (Renderer r in paintRenderers)
        {
            Material[] mats = r.materials;
            mats[0] = newMaterial;
            r.materials = mats;
        }
    }
}