using UnityEngine;

public class CarModelSwitcher : MonoBehaviour
{
    public CarData[] carDataList;
    public Transform carHolder;
    public GameObject currentCarInstance { get; private set; }

    private CarData currentCarData;
    private CarColorSwitcher carColorSwitcher;
    private int currentIndex = -1;

    void Start()
    {
        carColorSwitcher = GetComponent<CarColorSwitcher>();
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

        if (carColorSwitcher != null)
            carColorSwitcher.ApplyColorToCurrent(carColorSwitcher.currentColorIndex);
    }

    public CarData GetCurrentCarData()
    {
        return currentCarData;
    }
}