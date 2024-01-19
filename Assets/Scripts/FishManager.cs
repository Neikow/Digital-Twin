using UnityEngine;

public class FishManager : MonoBehaviour
{
    private const int ThreadGroupsSize = 1024;

    private static readonly int ViewRadius = Shader.PropertyToID("view_radius");
    private static readonly int AvoidRadius = Shader.PropertyToID("avoid_radius");
    private static readonly int NumFishes = Shader.PropertyToID("num_fishes");
    private static readonly int Fishes = Shader.PropertyToID("fishes");
    public FishSettings settings;
    public ComputeShader compute;
    private Fish[] _fishes;

    private void Start()
    {
        _fishes = FindObjectsOfType<Fish>();
        foreach (var f in _fishes) f.Initialize(settings);
    }

    private void Update()
    {
        if (_fishes != null)
        {
            var numFishes = _fishes.Length;
            var fishData = new FishData[numFishes];

            // Setting up the fish data inside the compute shader
            for (var i = 0; i < _fishes.Length; i++)
            {
                fishData[i].position = _fishes[i].position;
                fishData[i].direction = _fishes[i].forward;
                fishData[i].mass = _fishes[i].mass;
                fishData[i].isAlive = (uint)(_fishes[i].health > 0f ? 1 : 0);
            }

            var fishBuffer = new ComputeBuffer(numFishes, FishData.Size);

            fishBuffer.SetData(fishData);

            compute.SetBuffer(0, Fishes, fishBuffer);
            compute.SetInt(NumFishes, _fishes.Length);
            compute.SetFloat(ViewRadius, settings.perceptionRadius);
            compute.SetFloat(AvoidRadius, settings.avoidanceRadius);

            var threadGroups = Mathf.CeilToInt(numFishes / (float)ThreadGroupsSize);
            compute.Dispatch(0, threadGroups, 1, 1);

            // Getting the fish data from the compute shader
            fishBuffer.GetData(fishData);

            // Updating the fish data
            for (var i = 0; i < _fishes.Length; i++)
            {
                _fishes[i].avgFlockHeading = fishData[i].flockHeading;
                _fishes[i].centerOfFlockmates = fishData[i].flockCenter;
                _fishes[i].avgAvoidanceHeading = fishData[i].avoidanceHeading;
                _fishes[i].numPerceivedFlockmates = (int)fishData[i].numFlockmates;
                _fishes[i].neighborDensity = fishData[i].neighborDensity;
                
                _fishes[i].UpdateFish();
            }

            fishBuffer.Release();
        }
    }

    private struct FishData
    {
        public Vector3 position;
        public Vector3 direction;
        public Vector3 flockHeading;
        public Vector3 flockCenter;
        public Vector3 avoidanceHeading;
        public uint isAlive;
        public uint numFlockmates;
        public float mass;
        public float neighborDensity;


        public static int Size => 3 * 5 * sizeof(float)
                                  + 2 * sizeof(uint)
                                  + 2 * sizeof(float);
    }
}