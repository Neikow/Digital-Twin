using UnityEngine;

public static class FishHelper
{
    private const int NumViewDirections = 300;

    public static readonly Vector3[] Directions;

    static FishHelper()
    {
        Directions = new Vector3[NumViewDirections];

        var goldenRatio = (1 + Mathf.Sqrt(5)) / 2;

        var angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (var i = 0; i < NumViewDirections; i++)
        {
            var t = (float)i / NumViewDirections;
            var inclination = Mathf.Acos(1 - 2 * t);
            var azimuth = angleIncrement * i;

            var x = Mathf.Sin(inclination) * Mathf.Cos(azimuth);
            var y = Mathf.Sin(inclination) * Mathf.Sin(azimuth);
            var z = Mathf.Cos(inclination);

            Directions[i] = new Vector3(x, y, z);
        }
    }
}