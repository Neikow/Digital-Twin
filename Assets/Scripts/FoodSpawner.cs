using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    public enum GizmoType
    {
        Never,
        SelectedOnly,
        Always
    }

    public FoodSettings settings;

    public Food prefab;

    public GizmoType showSpawnRegion;

    private float _timeSinceLastSpawn;

    public void Update()
    {
        _timeSinceLastSpawn += Time.deltaTime;
        if (_timeSinceLastSpawn > 1 / settings.footFrequency)
        {
            _timeSinceLastSpawn = 0f;

            if (!ShouldSpawn()) return;

            for (var i = 0; i < settings.foodPerCycle; i++) SpawnFood();
            ;
        }
    }

    private void OnDrawGizmos()
    {
        if (showSpawnRegion == GizmoType.Always) DrawGizmos();
    }

    private void OnDrawGizmosSelected()
    {
        if (showSpawnRegion == GizmoType.SelectedOnly) DrawGizmos();
    }

    private bool ShouldSpawn()
    {
        var foods = FindObjectsOfType<Food>();
        // should take the spawn radius into account
        return foods.Length < settings.maxFood;
    }

    private void SpawnFood()
    {
        var pos = Random.insideUnitSphere * settings.spawnRadius;
        var food = Instantiate(prefab);
        var t = food.transform;
        t.position = pos;

        food.quality = Random.Range(settings.minFoodQuality, settings.maxFoodQuality);
        food.quantity = Random.Range(settings.minFoodQuantity, settings.maxFoodQuantity);
    }

    private void DrawGizmos()
    {
        Gizmos.color = new Color(settings.color.r, settings.color.g, settings.color.b, 0.3f);
        Gizmos.DrawSphere(transform.position, settings.spawnRadius);
    }
}