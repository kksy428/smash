using UnityEngine;

public class CarColorSwitcher : MonoBehaviour
{
    public CarModelSwitcher carModelSwitcher;
    public int currentColorIndex = 0;

    public void SelectColor(int index)
    {
        currentColorIndex = index;
        ApplyColorToCurrent(index);
    }

    public void ApplyColorToCurrent(int index)
    {
        if (carModelSwitcher.currentCarInstance == null) return;

        CarData data = carModelSwitcher.GetCurrentCarData();
        if (index < 0 || index >= data.colorMaterials.Length) return;

        CarController carController = carModelSwitcher.currentCarInstance.GetComponent<CarController>();
        if (carController == null) return;

        carController.ApplyColor(data.colorMaterials[index]);
    }
}