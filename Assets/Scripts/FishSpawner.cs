using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    public enum GizmoType
    {
        Never,
        SelectedOnly,
        Always
    }

    public FishSettings settings;

    public Fish prefab;
    public GizmoType showSpawnRegion;

    void Awake()
    {
        WaterQualityZone[] waterQualityZones = FindObjectsOfType<WaterQualityZone>();
        
        for (var i = 0; i < settings.fishCount; i++)
        {
            Vector3 pos = transform.position + Random.insideUnitSphere * settings.spawnRadius;
            Fish fish = Instantiate(prefab);
            var t = fish.transform;
            t.position = pos;
            t.forward = Random.insideUnitSphere;

            fish.SetWaterQualityZones(waterQualityZones);
            fish.SetColor(settings.color);
        }
    }

    private void OnDrawGizmos()
    {
        if (showSpawnRegion == GizmoType.Always)
        {
            DrawGizmos();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (showSpawnRegion == GizmoType.SelectedOnly)
        {
            DrawGizmos();
        }
    }

    private void DrawGizmos()
    {
        Gizmos.color = new Color(settings.color.r, settings.color.g, settings.color.b, 0.3f);
        Gizmos.DrawSphere(transform.position, settings.spawnRadius);
    }
}