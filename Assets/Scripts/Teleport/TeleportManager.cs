using UnityEngine;
using System.Collections;

public class TeleportManager : MonoBehaviour
{
    [Header("XR Rig Root")]
    public Transform xrRigRoot;

    [Header("Teleport Delay (sec)")]
    [Tooltip("텔레포트까지 기다릴 시간 (초)")]
    public float teleportDelay = 0f;

    /// <summary>
    /// 몇 초 후 targetRoot 위치로 텔레포트
    /// </summary>
    public void TeleportRig(Transform targetRoot)
    {
        if (xrRigRoot == null || targetRoot == null)
        {
            Debug.LogWarning("[TeleportManager] xrRigRoot 또는 targetRoot가 비어 있음");
            return;
        }

        StartCoroutine(TeleportRoutine(targetRoot));
    }

    private IEnumerator TeleportRoutine(Transform targetRoot)
    {
        // 지정한 시간만큼 대기
        if (teleportDelay > 0f)
            yield return new WaitForSeconds(teleportDelay);

        // 위치 이동
        xrRigRoot.position = targetRoot.position;

        // Y축 회전만 맞추기
        Vector3 rot = xrRigRoot.eulerAngles;
        rot.y = targetRoot.eulerAngles.y;
        xrRigRoot.eulerAngles = rot;

        Debug.Log("[TeleportManager] 텔레포트 완료");
    }
}
