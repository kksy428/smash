using UnityEngine;

public class CarColorSwitcher : MonoBehaviour
{
    public CarModelSwitcher carModelSwitcher;

    public void SelectColor(int index)
    {
        if (carModelSwitcher.currentCarInstance == null) return;

        CarData data = carModelSwitcher.GetCurrentCarData();
        if (index < 0 || index >= data.colorMaterials.Length) return;

        CarController paintTarget = carModelSwitcher.currentCarInstance.GetComponent<CarController>();
        if (paintTarget == null) return;

        paintTarget.ApplyColor(data.colorMaterials[index]);
    }
}