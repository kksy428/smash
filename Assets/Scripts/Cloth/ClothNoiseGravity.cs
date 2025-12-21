using UnityEngine;
using MagicaCloth2;

public class ClothNoiseForce : MonoBehaviour
{
    public MagicaCloth cloth;
    public float noiseScale = 2f;
    public float velocityAmount = 8f;  // m/s 단위 속도 변화량

    void Update()
    {
        if (cloth == null || cloth.IsValid() == false)
            return;

        float t = Time.time * noiseScale;
        Vector3 noiseDir = new Vector3(
            (Mathf.PerlinNoise(t, 0f) - 0.5f),
            (Mathf.PerlinNoise(0f, t) - 0.5f),
            (Mathf.PerlinNoise(t, t) - 0.5f)
        ).normalized;

        cloth.AddForce(noiseDir, velocityAmount, ClothForceMode.VelocityAdd);
    }
}
