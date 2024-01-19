using UnityEngine;

[CreateAssetMenu]
public class FishSettings : ScriptableObject
{
    // Settings
    [Header("Aquarium Settings")] public float baseTemperature = 288.13f; // 15°C

    public float basePh = 7f;

    [Header("Spawner Settings")] public Color color = Color.white;

    public int fishCount = 100;
    public float spawnRadius = 10f;

    [Header("Fish Settings")] public float minSize = .5f;

    public float maxSize = 2f;
    public float density = .5f;

    [Header("Speed & Rotation")] public float minSpeed = 2f;

    public float maxSpeed = 5f;
    public float maxSteerForce = 3f;

    [Header("Detection")] public float perceptionRadius = 2.5f;

    public float avoidanceRadius = 1f;

    [Header("Behavior Weights")] public float alignWeight = 1f;

    public float cohesionWeight = 1f;
    public float seperateWeight = 1f;
    public float targetWeight = 1f;
    public float fleeWeight = 0.5f;

    [Header("Health")] public float minHealth = 1f;

    public float maxHealth = 1f;
    public float minHealthDecay = 0.1f;
    public float maxHealthDecay = 0.2f;

    [Header("Hunger")] public float minHungerIncrease = 0.01f;

    public float maxHungerIncrease = 0.02f;
    public float hungerThreshold = 0.5f;

    [Header("Collisions")] public LayerMask obstacleMask;

    public float boundsRadius = .27f;
    public float avoidCollisionWeight = 10f;
    public float collisionAvoidDst = 5f;
}