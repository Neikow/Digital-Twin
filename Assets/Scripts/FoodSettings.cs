using UnityEngine;


[CreateAssetMenu]
public class FoodSettings : ScriptableObject
{
    [Header("Food Settings")] public float footFrequency = 0.5f; // 0.5s^-1

    public float lifeTime = 30f; // 30s

    [Header("Quality")] public float minFoodQuality = 0.5f;

    public float maxFoodQuality = 1f;

    [Header("Quantity")] public float minFoodQuantity = 0.5f;

    public float maxFoodQuantity = 1f;

    [Header("Spawner Settings")] public Color color = Color.white;

    public int foodPerCycle = 2; // 2 food per cycle
    public float spawnRadius = 3f; // 3m
    public int maxFood = 10;
}