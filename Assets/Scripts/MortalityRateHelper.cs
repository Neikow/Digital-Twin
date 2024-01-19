using UnityEngine;

public class MortalityRateHelper
{
    private readonly MortalityRateSettings _settings;

    public MortalityRateHelper(MortalityRateSettings settings)
    {
        _settings = settings;
    }

    public float FromTemperature(float tempAtPoint)
    {
        return 1
               - 1 / (1 + Mathf.Exp(-_settings.tempDecay * (tempAtPoint - _settings.tempMax)))
               + 1 / (1 + Mathf.Exp(-_settings.tempDecay * (tempAtPoint - _settings.tempMin)));
    }

    public float FromPh(float pHAtPoint)
    {
        return 1 
               - 1 / (1 + Mathf.Exp(-_settings.pHDecay * (pHAtPoint - _settings.pHMax))) 
               + 1 / (1 + Mathf.Exp(-_settings.pHDecay * (pHAtPoint - _settings.pHMin)));
    }

    public float FromFood(float currentFood)
    {
        return Mathf.Min(1 - currentFood, 0.5f);
    }
    
    public float FromFishDensity(float fishDensityAtPoint)
    {
        return 2 / (1 + Mathf.Exp(-_settings.densityDecay * (fishDensityAtPoint - _settings.densityMax)));
    }
}