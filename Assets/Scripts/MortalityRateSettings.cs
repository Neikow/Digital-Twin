using UnityEngine;


[CreateAssetMenu]
public class MortalityRateSettings : ScriptableObject
{
    [Header("Temperature")] public float tempMin = (float)(273.15 + 6.4); // 6.4°C

    public float tempMax = (float)(273.15 + 16.2); // 16.2°C
    public float tempDecay = 10f; // 10°C^-1

    [Header("Fish Density")] public float densityMax = 40f; // 40kg/m^3

    public float densityDecay = 0.3f; // 0.3m^3/kg

    [Header("pH")] public float pHMin = 6.5f;

    public float pHMax = 7.5f;
    public float pHDecay = 10f; // 10pH^-1

    [Header("Food")] public float foodDecay = 0.1f; // 0.1kg^-1
}