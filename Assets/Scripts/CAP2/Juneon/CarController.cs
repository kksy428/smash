using UnityEngine;
using System.Collections.Generic;

public class CarController : MonoBehaviour
{
    public Renderer[] paintRenderers;
    private Animator[] animators;
    private Dictionary<string, bool> toggleStates = new Dictionary<string, bool>();

    void Start()
    {
        animators = GetComponentsInChildren<Animator>();
    }

    public void ApplyColor(Material material)
    {
        foreach (Renderer r in paintRenderers)
        {
            Material[] mats = r.materials;
            mats[0] = material;
            r.materials = mats;
        }
    }

    public void ToggleAnimation(string openTrigger, string closeTrigger)
    {
        // 현재 상태 확인 (없으면 기본값 false = 닫힘)
        if (!toggleStates.ContainsKey(openTrigger))
            toggleStates[openTrigger] = false;

        bool isOpen = toggleStates[openTrigger];
        string triggerToPlay = isOpen ? closeTrigger : openTrigger;

        // 상태 반전
        toggleStates[openTrigger] = !isOpen;

        // 해당 트리거 실행
        foreach (Animator anim in animators)
        {
            foreach (AnimatorControllerParameter param in anim.parameters)
            {
                if (param.name == triggerToPlay && param.type == AnimatorControllerParameterType.Trigger)
                {
                    anim.SetTrigger(triggerToPlay);
                    break;
                }
            }
        }
    }
}