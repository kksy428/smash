using UnityEngine;

public class CarSwitcher : MonoBehaviour
{
    [Header("차 프리팹 목록")]
    public GameObject[] carPrefabs;

    [Header("차가 배치될 부모 오브젝트 (Cylinder의 자식)")]
    public Transform carHolder;

    private int currentIndex = -1;
    public GameObject currentCarInstance;

    private CarColorSwitcher carColorSwitcher;

    void Start()
    {
        carColorSwitcher = GetComponent<CarColorSwitcher>();
        SelectCar(0);
    }

    private void SpawnCar(int index)
    {
        if (currentCarInstance != null)
            Destroy(currentCarInstance);

        currentCarInstance = Instantiate(carPrefabs[index], carHolder);

        // 차량 생성 후 현재 선택된 색상 바로 적용
        if (carColorSwitcher != null)
            carColorSwitcher.ApplyColorToCurrent(carColorSwitcher.pendingColorIndex);
    }

    public void SelectCar(int index)
    {
        if (index < 0 || index >= carPrefabs.Length) return;
        if (index == currentIndex) return;

        currentIndex = index;
        SpawnCar(index);
    }
}