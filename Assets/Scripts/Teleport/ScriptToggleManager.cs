using UnityEngine;

public class ScriptToggleManager : MonoBehaviour
{
    public Behaviour targetComponent;

    public void DisableComponent()
    {
        if (targetComponent != null)
            targetComponent.enabled = false;
    }

    public void EnableComponent()
    {
        if (targetComponent != null)
            targetComponent.enabled = true;
    }
}
