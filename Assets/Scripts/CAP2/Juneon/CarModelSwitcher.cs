using UnityEngine;

public class CarModelSwitcher : MonoBehaviour
{
    [Header("차량 데이터 목록")]
    public CarData[] carDataList;

    public Transform carHolder;
    public GameObject currentCarInstance { get; private set; }
    private CarData currentCarData;
    private int currentIndex = -1;

    void Start()
    {
        SelectCar(0);
    }

    public void SelectCar(int index)
    {
        if (index < 0 || index >= carDataList.Length) return;
        if (index == currentIndex) return;

        currentIndex = index;
        currentCarData = carDataList[index];

        if (currentCarInstance != null)
            Destroy(currentCarInstance);

        currentCarInstance = Instantiate(currentCarData.prefab, carHolder);
    }

    public CarData GetCurrentCarData()
    {
        return currentCarData;
    }
}