using UnityEngine;

public class WaterQualityZone : MonoBehaviour
{
    [Header("Zone")] public float radius = 3.0f;

    public bool showRadius = true;
    public Color color = new(1, 0.5f, 1);

    [Header("pH")] public bool pHActive;

    public float pH = 7.0f;

    /* pH loss per meter */
    public float pHDecay = 0.1f;

    [Header("Temperature")] public bool temperatureActive;

    public float temperature = 291.15f; // 18°C
    public float temperatureDecay = 0.2f;

    [Header("Settings")] public MortalityRateSettings mortalityRateSettings;

    private MortalityRateHelper _mortalityRateHelper;

    private void OnDrawGizmos()
    {
        if (showRadius) DrawGizmo();
    }

    private void OnDrawGizmosSelected()
    {
        if (showRadius) DrawGizmo();
    }

    public float GetPhProbability(Vector3 position, float fallback)
    {
        if (_mortalityRateHelper == null) _mortalityRateHelper = new MortalityRateHelper(mortalityRateSettings);

        return _mortalityRateHelper.FromPh(GetPhAt(position, fallback));
    }

    public float GetTemperatureProbability(Vector3 position, float fallback)
    {
        if (_mortalityRateHelper == null) _mortalityRateHelper = new MortalityRateHelper(mortalityRateSettings);

        return _mortalityRateHelper.FromTemperature(GetTemperatureAt(position, fallback));
    }

    public float GetDensityProbability(float density)
    {
        if (_mortalityRateHelper == null) _mortalityRateHelper = new MortalityRateHelper(mortalityRateSettings);

        return _mortalityRateHelper.FromFishDensity(density);
    }

    public float GetFoodProbability(float food)
    {
        if (_mortalityRateHelper == null) _mortalityRateHelper = new MortalityRateHelper(mortalityRateSettings);

        return _mortalityRateHelper.FromFood(food);
    }

    public bool IsInside(Vector3 position)
    {
        return Vector3.Distance(position, transform.position) < radius;
    }

    public float GetPhAt(Vector3 position, float fallback)
    {
        if (!pHActive) return fallback;
        var distance = Vector3.Distance(position, transform.position);
        if (distance > radius) return fallback;
        return pH - pHDecay * distance;
    }

    public float GetTemperatureAt(Vector3 position, float fallback)
    {
        if (!temperatureActive) return fallback;

        var distance = Vector3.Distance(position, transform.position);
        if (distance > radius) return fallback;
        return temperature - temperatureDecay * distance;
    }

    public Vector3 GetOutwardDirection(Vector3 position)
    {
        return (position - transform.position).normalized;
    }

    private void DrawGizmo()
    {
        Gizmos.color = new Color(color.r, color.g, color.b, 0.2f);

        for (var i = 0; i < radius; i++) Gizmos.DrawSphere(transform.position, i);

        Gizmos.DrawSphere(transform.position, radius);
    }
}