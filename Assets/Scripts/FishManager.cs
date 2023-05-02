using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour {

  const int threadGroupsSize = 1024;

  public FishSettings settings;
  public ComputeShader compute;
  Fish[] fishes;

  void Start () {
    fishes = FindObjectsOfType<Fish> ();
    foreach (Fish f in fishes) {
      f.Initialize (settings, null);
    }
  }

  void Update () {
    if (fishes != null) {

      int numFishes = fishes.Length;
      var fishData = new FishData[numFishes];

      for (int i = 0; i < fishes.Length; i++) {
        fishData[i].position = fishes[i].position;
        fishData[i].direction = fishes[i].forward;
      }

      var fishBuffer = new ComputeBuffer (numFishes, FishData.Size);

      fishBuffer.SetData (fishData);

      compute.SetBuffer (0, "fishes", fishBuffer);
      compute.SetInt ("numFishes", fishes.Length);
      compute.SetFloat ("viewRadius", settings.perceptionRadius);
      compute.SetFloat ("avoidRadius", settings.avoidanceRadius);

      int threadGroups = Mathf.CeilToInt (numFishes / (float) threadGroupsSize);
      compute.Dispatch (0, threadGroups, 1, 1);

      fishBuffer.GetData (fishData);

      for (int i = 0; i < fishes.Length; i++) {
        fishes[i].avgFlockHeading = fishData[i].flockHeading;
        fishes[i].centerOfFlockmates = fishData[i].flockCenter;
        fishes[i].avgAvoidanceHeading = fishData[i].avoidanceHeading;
        fishes[i].numPerceivedFlockmates = fishData[i].numFlockmates;

        fishes[i].UpdateFish ();
      }

      fishBuffer.Release ();
    }

  }

  public struct FishData {
    public Vector3 position;
    public Vector3 direction;

  
    public Vector3 flockHeading;
    public Vector3 flockCenter;
    public Vector3 avoidanceHeading;
    public int numFlockmates;

    public static int Size {
        get {
            return sizeof (float) * 3 * 5 + sizeof (int);
        }
    }
  }
}