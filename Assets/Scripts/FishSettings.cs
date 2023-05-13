using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FishSettings : ScriptableObject {
  // Settings
  [Header ("Spawner Settings")]
  public Color color = Color.white;
  public int fishCount = 100;
  public float spawnRadius = 10;

  [Header ("Speed & Rotation")]
  public float minSpeed = 2;
  public float maxSpeed = 5;
  public float maxSteerForce = 3;

  [Header ("Detection")]
  public float perceptionRadius = 2.5f;
  public float avoidanceRadius = 1;

  [Header ("Behavior Weights")]
  public float alignWeight = 1;
  public float cohesionWeight = 1;
  public float seperateWeight = 1;
  public float targetWeight = 1;


  [Header ("Collisions")]
  public LayerMask obstacleMask;
  public float boundsRadius = .27f;
  public float avoidCollisionWeight = 10;
  public float collisionAvoidDst = 5;
}