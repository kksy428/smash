using UnityEngine;

public class CarColorSwitcher : MonoBehaviour
{
    private CarSwitcher carModelSwitcher;
    public int pendingColorIndex = 0; // 차량 생성 전에 눌린 색상 저장

    void Start()
    {
        carModelSwitcher = GetComponent<CarSwitcher>();

        if (carModelSwitcher == null)
            Debug.LogError("같은 오브젝트에 CarModelSwitcher가 없어요!");
    }

    public void SelectColor(int index)
    {
        pendingColorIndex = index;

        // 차량이 아직 없으면 저장만 하고 종료
        if (carModelSwitcher.currentCarInstance == null) return;

        ApplyColorToCurrent(index);
    }

    public void ApplyColorToCurrent(int index)
    {
        CarPaintTarget paintTarget = carModelSwitcher.currentCarInstance.GetComponent<CarPaintTarget>();

        if (paintTarget == null)
        {
            Debug.LogWarning("현재 차량에 CarPaintTarget이 없어요!");
            return;
        }

        paintTarget.ApplyColor(index);
    }
}