using UnityEngine;
using UnityEngine.UI;

public class ToggleController : MonoBehaviour
{
    public Toggle targetToggle; // 자기 자신 토글 넣어도 됨

    // OnValueChanged(bool)에서 호출되는 함수
    public void TurnOffIfOn(bool value)
    {
        if (targetToggle == null) return;

        // value == true → 토글을 강제로 끄기
        if (value)
        {
            targetToggle.isOn = false;
        }
        // value == false → 아무것도 하지 않음
    }
}
