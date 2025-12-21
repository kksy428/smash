using UnityEngine;
using System.Collections;
using MagicaCloth2;

public class ClothDropController : MonoBehaviour
{
    [Header("천 움직임 설정")]
    public float dropDuration = 1f;
    public float dropWaitFrames = 1.6f;
    public float windRemoveDuration = 1.6f;
    public float windVelocity = 1500f;
    public Vector3 windDirection = new Vector3(0, 0, 0);

    [Header("리셋 설정")]
    public float resetDelay = 0.03f; // 필요하면 다른 스크립트에서 기다릴 때 사용

    private MagicaCloth cloth;
    private Vector3 originalPos;        // 초기 위치만 기억
    private MeshRenderer meshRenderer;
    private Material clothMaterial;

    /* ------------------------------
     * 머테리얼 관련 유틸
     * ------------------------------*/
    private void SetMaterialZWrite(bool enable)
    {
        if (clothMaterial == null) return;

        float zwriteValue = enable ? 1.0f : 0.0f;

        if (clothMaterial.HasProperty("_ZWrite"))
            clothMaterial.SetFloat("_ZWrite", zwriteValue);

        if (clothMaterial.HasProperty("_ZTest"))
            clothMaterial.SetFloat("_ZTest", (float)UnityEngine.Rendering.CompareFunction.LessEqual);
    }

    private void SetMaterialAlpha(float alpha)
    {
        if (clothMaterial == null) return;

        if (clothMaterial.HasProperty("_BaseColor"))
        {
            Color baseColor = clothMaterial.GetColor("_BaseColor");
            baseColor.a = alpha;
            clothMaterial.SetColor("_BaseColor", baseColor);
        }

        Color col = clothMaterial.color;
        col.a = alpha;
        clothMaterial.color = col;
    }

    /* ------------------------------
     * 초기화
     * ------------------------------*/
    private void Awake()
    {
        cloth = GetComponent<MagicaCloth>();
        meshRenderer = GetComponent<MeshRenderer>();

        originalPos = transform.position;

        if (meshRenderer != null)
        {
            clothMaterial = meshRenderer.material;
            SetMaterialZWrite(true);
            clothMaterial.renderQueue = 2500;
            SetMaterialAlpha(1f);
        }

        if (cloth != null)
        {
            cloth.SetSkipWriting(true);
            cloth.enabled = false;
        }

        if (meshRenderer != null)
            meshRenderer.enabled = false;
    }

    private IEnumerator Start()
    {
        yield return null;
    }

    /* ------------------------------
     * DROP
     * ------------------------------*/
    public IEnumerator DropCloth()
    {
        if (cloth == null) yield break;

        // 드롭 시작 전에 기본 상태 보장하고 싶으면 여기서도 ResetCloth() 호출 가능
        cloth.enabled = true;
        cloth.ResetCloth(true);

        if (meshRenderer != null)
        {
            meshRenderer.enabled = true;
            SetMaterialAlpha(1f);
        }

        // 시뮬 안정화용으로 두 프레임 정도 넘기기
        yield return null;
        yield return null;

        cloth.SetSkipWriting(false);

        // 천이 떨어지는 연출 시간
        yield return new WaitForSeconds(dropDuration);
    }

    /* ------------------------------
     * WIND 제거
     * ------------------------------*/
    public IEnumerator RemoveClothWithWind()
    {
        if (cloth == null) yield break;

        if (!cloth.enabled)
            cloth.enabled = true;

        float elapsed = 0f;
        while (elapsed < windRemoveDuration)
        {
            cloth.AddForce(windDirection.normalized, windVelocity);

            float alpha = 1.0f - (elapsed / windRemoveDuration);
            SetMaterialAlpha(alpha);

            elapsed += Time.deltaTime;
            yield return null;
        }

        SetMaterialAlpha(0f);

        if (meshRenderer != null)
            meshRenderer.enabled = false;
    }

    /* ------------------------------
     * 즉시 RESET (코루틴 아님)
     * ------------------------------*/
    public void ResetCloth()
    {
        if (cloth == null) return;

        // 이 스크립트 안에서 돌고 있던 코루틴들만 정리 (혹시 쓸 일이 생길 때 대비)
        StopAllCoroutines();

        cloth.enabled = false;
        cloth.SetSkipWriting(true);

        if (meshRenderer != null)
            meshRenderer.enabled = false;

        SetMaterialAlpha(1f);

        // 위치만 초기 위치로 되돌림 (회전은 유지)
        transform.position = originalPos;

        // 내부 시뮬레이션 상태 리셋
        cloth.ResetCloth(true);
    }
}
