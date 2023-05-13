using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour {

  public enum GizmoType { Never, SelectedOnly, Always }

  public FishSettings settings;

  public Fish prefab;
  public GizmoType showSpawnRegion;

  void Awake () {
    for (int i = 0; i < settings.fishCount; i++) {
      Vector3 pos = transform.position + Random.insideUnitSphere * settings.spawnRadius;
      Fish fish = Instantiate(prefab);
      fish.transform.position = pos;
      fish.transform.forward = Random.insideUnitSphere;

      fish.SetColor(settings.color);
    }
  }

  private void OnDrawGizmos () {
    if (showSpawnRegion == GizmoType.Always) {
      DrawGizmos ();
    }
  }

  void OnDrawGizmosSelected () {
    if (showSpawnRegion == GizmoType.SelectedOnly) {
      DrawGizmos ();
    }
  }

  void DrawGizmos () {
    Gizmos.color = new Color (settings.color.r, settings.color.g, settings.color.b, 0.3f);
    Gizmos.DrawSphere (transform.position, settings.spawnRadius);
  }
}