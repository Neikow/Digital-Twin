using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FishSettings : ScriptableObject {
  // Settings
  [Header ("Spawner Settings")]
  public Color color = Color.white;
  public int fishCount = 100;
  public float spawnRadius = 10f;

  [Header ("Fish Settings")]
  public float minSize = .5f;
  public float maxSize = 2f;

  [Header ("Speed & Rotation")]
  public float minSpeed = 2f;
  public float maxSpeed = 5f;
  public float maxSteerForce = 3f;

  [Header ("Detection")]
  public float perceptionRadius = 2.5f;
  public float avoidanceRadius = 1f;

  [Header ("Behavior Weights")]
  public float alignWeight = 1f;
  public float cohesionWeight = 1f;
  public float seperateWeight = 1f;
  public float targetWeight = 1f;


  [Header ("Collisions")]
  public LayerMask obstacleMask;
  public float boundsRadius = .27f;
  public float avoidCollisionWeight = 10f;
  public float collisionAvoidDst = 5f;
}