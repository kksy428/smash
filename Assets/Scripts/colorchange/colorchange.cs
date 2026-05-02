using UnityEngine;
using System.Collections;

public class colorchange : MonoBehaviour
{
    [Header("애니메이션 타이밍")]
    [Range(0f, 5f)]
    public float changeDelay = 1.0f;

    [Header("차량 부품 및 컨트롤러")]
    public Renderer[] carParts;
    public ClothDropController clothController;

    [Header("재질 목록")]
    public Material matRed;
    public Material matBlue;
    public Material matBlack;
    public Material matWhite;
    public Material matOrange;
    public Material matGreen;

    [SerializeField] private Coroutine currentRoutine;

    private IEnumerator ChangeMaterialRoutine(Material targetMat)
    {
        if (clothController == null)
        {
            Debug.LogError("clothController 연결 안됨!");
            yield break;
        }

        // 1) 새 시퀀스 시작 전에 항상 한 번 강제 리셋
        clothController.ResetCloth();

        // 2) 천 드롭
        yield return clothController.DropCloth();

        // 3) 약간 기다렸다가 (연출용)
        yield return new WaitForSeconds(changeDelay);

        // 4) 차 재질 변경
        if (carParts != null)
        {
            foreach (Renderer part in carParts)
            {
                if (part != null)
                    part.material = targetMat;
            }
        }

        // 5) 천을 바람으로 날려보내기
        yield return clothController.RemoveClothWithWind();

        // 6) 시퀀스 끝났으니 다시 즉시 리셋해서 다음 드롭 준비
        clothController.ResetCloth();

        // 7) 코루틴 핸들 정리
        currentRoutine = null;
    }

    public void SetMaterialRed() { StartMaterialChange(matRed); }
    public void SetMaterialBlue() { StartMaterialChange(matBlue); }
    public void SetMaterialBlack() { StartMaterialChange(matBlack); }
    public void SetMaterialWhite() { StartMaterialChange(matWhite); }
    public void SetMaterialOrange() { StartMaterialChange(matOrange); }
    public void SetMaterialGreen() { StartMaterialChange(matGreen); }

    private void StartMaterialChange(Material mat)
    {
        if (mat == null)
        {
            Debug.LogWarning("넘겨준 Material 이 null 임");
            return;
        }

        // 1) 이전 시퀀스 중단
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        // 2) 혹시 중간 상태로 남아 있을 cloth를 강제로 초기화
        if (clothController != null)
        {
            clothController.ResetCloth();
        }

        // 3) 새로운 시퀀스 시작
        currentRoutine = StartCoroutine(ChangeMaterialRoutine(mat));
    }
}
