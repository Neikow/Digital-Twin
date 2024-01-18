using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MortalityRateHelper
{
    private MortalityRateSettings _settings;

    public MortalityRateHelper(MortalityRateSettings settings)
    {
        _settings = settings;
    }
    
    public static float FromTemperature(float tempAtPoint) {
        var a = 2.2f;
        var b = 5.2f;
        var c = 2.58f;

        var tMin = (float)(273.15 + 6.4);  // 6.4C
        var tMax = (float)(273.15 + 16.2); // 16.2C

        var I = 1.2f;

        return 0.5f - (c - a / (1 + Mathf.Exp(-I * (tempAtPoint - tMin))) + b / (1 + Mathf.Exp(-I * (tempAtPoint - tMax)))) / (c + b);
    }

    public static float FromFishDensity(float fishDensityAtPoint) {
        var c = 0f;
        var a = 1f;
        var p = 0.4f;
        var dMax = 44.4f;

        return c + a / (1 + Mathf.Exp(-p * (fishDensityAtPoint - dMax)));
    }

    public static float FromPh(float pHAtPoint) {
        var pHMin = 6.5f;
        var pHMax = 7.5f;
        var c = 1f;
        var p = 10f;

        return c - p / (1 + Mathf.Exp(-pHAtPoint + pHMin)) + p / (1 + Mathf.Exp(pHAtPoint - pHMax));
    }

    public static float FromFood(float currentFood) {
        var foodQuality = 1f;

        return 1 - currentFood / foodQuality;
    }
}
