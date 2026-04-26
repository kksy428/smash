using UnityEngine;

public class CarAnimationController : MonoBehaviour
{
    public CarModelSwitcher carModelSwitcher;

    public void ToggleAnimation(int index)
    {
        if (carModelSwitcher.currentCarInstance == null) return;

        CarData data = carModelSwitcher.GetCurrentCarData();
        if (index < 0 || index >= data.animations.Length) return;

        CarController carController = carModelSwitcher.currentCarInstance.GetComponent<CarController>();
        if (carController == null) return;

        CarAnimationEntry entry = data.animations[index];
        carController.ToggleAnimation(entry.openTrigger, entry.closeTrigger);
    }
}