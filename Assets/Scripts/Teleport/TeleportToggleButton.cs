using UnityEngine;
using UnityEngine.UI; // Meta Toggle이 Unity UI Toggle 기반이면 이걸로 충분

public class TeleportToggleButton : MonoBehaviour
{
    [Header("Meta Toggle")]
    public Toggle toggle; // Meta에서 제공한 토글 컴포넌트

    [Header("Teleport")]
    public TeleportManager teleportManager;
    public Transform targetRoot; // 이 버튼을 눌렀을 때 텔포할 리그 위치(차 안/차 밖 포인트)

    private void Awake()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }
        else
        {
            Debug.LogWarning("[TeleportToggleButton] Toggle이 연결되어 있지 않음");
        }
    }

    private void OnDestroy()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
    }

    private void OnToggleChanged(bool isOn)
    {
        // 토글이 ON 되었을 때만 텔포 실행 (버튼처럼 사용)
        if (!isOn) return;

        if (teleportManager != null && targetRoot != null)
        {
            teleportManager.TeleportRig(targetRoot);
        }
        else
        {
            Debug.LogWarning("[TeleportToggleButton] teleportManager 또는 targetRoot가 비어 있음");
        }

        // 한 번 누르면 다시 OFF로 되돌려서 재사용 가능하게
        toggle.isOn = false;
    }
}
