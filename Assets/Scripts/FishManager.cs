using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishManager : MonoBehaviour {
  private const int ThreadGroupsSize = 1024;
  private Fish[] _fishes;
  public FishSettings settings;
  public ComputeShader compute;

  private static readonly int ViewRadius = Shader.PropertyToID("view_radius");
  private static readonly int AvoidRadius = Shader.PropertyToID("avoid_radius");
  private static readonly int NumFishes = Shader.PropertyToID("num_fishes");
  private static readonly int Fishes = Shader.PropertyToID("fishes");

  void Start () {
    _fishes = FindObjectsOfType<Fish>();
    foreach (Fish f in _fishes) {
      f.Initialize(settings, null);
    }
  }

  void Update () {
    if (_fishes != null) {
      var numFishes = _fishes.Length;
      var fishData = new FishData[numFishes];
      
      // Setting up the fish data inside the compute shader
      for (var i = 0; i < _fishes.Length; i++) {
        fishData[i].position = _fishes[i].position;
        fishData[i].direction = _fishes[i].forward;
        fishData[i].mass = _fishes[i].mass;
      }

      var fishBuffer = new ComputeBuffer(numFishes, FishData.Size);

      fishBuffer.SetData(fishData);

      compute.SetBuffer(0, Fishes, fishBuffer);
      compute.SetInt(NumFishes, _fishes.Length);
      compute.SetFloat(ViewRadius, settings.perceptionRadius);
      compute.SetFloat(AvoidRadius, settings.avoidanceRadius);

      var threadGroups = Mathf.CeilToInt(numFishes / (float) ThreadGroupsSize);
      compute.Dispatch(0, threadGroups, 1, 1);

      // Getting the fish data from the compute shader
      fishBuffer.GetData(fishData);

      // Updating the fish data
      for (var i = 0; i < _fishes.Length; i++) {
        _fishes[i].avgFlockHeading = fishData[i].flockHeading;
        _fishes[i].centerOfFlockmates = fishData[i].flockCenter;
        _fishes[i].avgAvoidanceHeading = fishData[i].avoidanceHeading;
        _fishes[i].numPerceivedFlockmates = fishData[i].numFlockmates;
        _fishes[i].neighborDensity = fishData[i].neighborDensity;

        _fishes[i].UpdateFish();
      }

      fishBuffer.Release();
    }

  }

  private struct FishData {
    public Vector3 position;
    public Vector3 direction;
    public Vector3 flockHeading;
    public Vector3 flockCenter;
    public Vector3 avoidanceHeading;
    public int numFlockmates;
    public float mass;
    public float neighborDensity;

    
    public static int Size => 3 * 5 * sizeof(float) + 1 * sizeof(int) + 2 * sizeof(float);
  }
}