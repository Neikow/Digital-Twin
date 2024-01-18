using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FishHelper {

  const int NumViewDirections = 300;

  public static readonly Vector3[] Directions;

  static FishHelper() {
    Directions = new Vector3[NumViewDirections];

    float goldenRatio = (1 + Mathf.Sqrt (5)) / 2;

    float angleIncrement = Mathf.PI * 2 * goldenRatio;

    for (int i = 0; i < NumViewDirections; i++) {
      float t = (float) i / NumViewDirections;
      float inclination = Mathf.Acos (1 - 2 * t);
      float azimuth = angleIncrement * i;

      float x = Mathf.Sin (inclination) * Mathf.Cos (azimuth);
      float y = Mathf.Sin (inclination) * Mathf.Sin (azimuth);
      float z = Mathf.Cos (inclination);

      Directions[i] = new Vector3 (x, y, z);
    }
  }
}