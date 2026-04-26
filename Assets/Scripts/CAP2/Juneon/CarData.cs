using UnityEngine;

[System.Serializable]
public class CarAnimationEntry
{
    public string label;         // ex) "차문"
    public string openTrigger;   // ex) "DoorOpen"
    public string closeTrigger;  // ex) "DoorClose"
}

[CreateAssetMenu(fileName = "CarData", menuName = "Car/CarData")]
public class CarData : ScriptableObject
{
    [Header("차량 프리팹")]
    public GameObject prefab;

    [Header("색상 머테리얼 풀")]
    public Material[] colorMaterials;

    [Header("애니메이션 목록")]
    public CarAnimationEntry[] animations;
}