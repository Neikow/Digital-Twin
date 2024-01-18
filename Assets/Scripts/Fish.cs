using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [HideInInspector]
    FishSettings settings;

    [HideInInspector]
    // Size and mass of the fish
    public float fishSize;

    // State
    public Vector3 position;
    public Vector3 forward;
    Vector3 velocity;

    // To update:
    Vector3 acceleration;
    public Vector3 avgFlockHeading;
    public Vector3 avgAvoidanceHeading;
    public Vector3 centerOfFlockmates;
    public int numPerceivedFlockmates;

    // Cached
    Material material;
    Transform cachedTransform;
    Transform target;
    public bool isDead;

    public float health; 
    
    public float mass;

    private WaterQualityZone[] _waterQualityZones;

    public float neighborDensity;

    void Awake () {
        cachedTransform = transform;
        material = cachedTransform.GetComponentInChildren<MeshRenderer>().material;
        isDead = false;
    }

    public void SetWaterQualityZones(WaterQualityZone[] waterQualityZones)
    {
        _waterQualityZones = waterQualityZones;
    }

    public void Initialize(FishSettings settings, Transform target) {
        this.target = target;
        this.settings = settings;

        health = 1.0f;
        
        fishSize = Random.Range(settings.minSize, settings.maxSize);
        cachedTransform.localScale = Vector3.one * fishSize;

        position = cachedTransform.position;
        forward = cachedTransform.forward;
        
        mass = fishSize * settings.density * Random.Range(0.9f, 1.1f);
        
        neighborDensity = 12f;

        float startSpeed = Random.Range(settings.minSpeed, settings.maxSpeed);
        velocity = transform.forward * startSpeed;
    }

    public void SetColor(Color col) {
        if (material != null) {
            material.color = col;
        }
    }

    public void UpdateFish () {
        if (isDead) {
            Vector3 risingDirection = Vector3.up;

            if (Physics.Raycast(cachedTransform.position, risingDirection, out var hit, Mathf.Infinity, settings.obstacleMask)) {
                if (hit.distance > 0.1f) {
                    cachedTransform.position += risingDirection * (fishSize * Time.deltaTime);
                }
                
                if (cachedTransform.rotation.eulerAngles.z < 90) {
                    cachedTransform.Rotate(Vector3.forward * (Time.deltaTime * 10));
                }

            }
            
            return;
        }

        Vector3 acceleration = Vector3.zero;
        
        if (target != null) {
            Vector3 offsetToTarget = (target.position - position);
            acceleration = SteerTowards(offsetToTarget) * settings.targetWeight;
        }
        
        if (_waterQualityZones.Length != 0)
        {
            Vector3 offsetToWaterQualityZone = Vector3.zero;
            float waterQualityZoneTemperature = settings.baseTemperature;
            float waterQualityZonePh = settings.basePh;
            foreach (var waterQualityZone in _waterQualityZones)
            {
                if (!waterQualityZone.IsInside(position))
                {
                    continue;
                }

                var zonePosition = waterQualityZone.transform.position;
                Debug.DrawLine(position, zonePosition, Color.red);
                Vector3 outwardDirection = waterQualityZone.GetOutwardDirection(position);
                float distance = Vector3.Distance(position, zonePosition);
                
                float temperature = waterQualityZone.GetTemperatureAt(position, settings.baseTemperature);
                float ph = waterQualityZone.GetPhAt(position, settings.basePh);

                float tempMortality = MortalityRateHelper.FromTemperature(temperature);
                float phMortality = MortalityRateHelper.FromPh(ph);
                
                offsetToWaterQualityZone += outwardDirection * distance;
                waterQualityZoneTemperature = (waterQualityZoneTemperature + temperature) / 2;
                waterQualityZonePh = (waterQualityZonePh + ph) / 2;
            }
            acceleration += SteerTowards(offsetToWaterQualityZone) * settings.fleeWeight;
            Debug.Log("Density: " + neighborDensity);
            Debug.Log("Temperature: " + waterQualityZoneTemperature);
            Debug.Log("pH: " + waterQualityZonePh);
        }
        
        if (numPerceivedFlockmates != 0) {
            centerOfFlockmates /= numPerceivedFlockmates;

            Vector3 offsetToFlockmatesCenter = (centerOfFlockmates - position);

            Debug.DrawLine(position, centerOfFlockmates, Color.green);

            var alignmentForce = SteerTowards(avgFlockHeading) * settings.alignWeight;
            var cohesionForce = SteerTowards(offsetToFlockmatesCenter) * settings.cohesionWeight;
            var seperationForce = SteerTowards(avgAvoidanceHeading) * settings.seperateWeight;

            acceleration += (alignmentForce + cohesionForce + seperationForce);
        }

        if (IsHeadingForCollision()) {
            Vector3 collisionAvoidDir = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * settings.avoidCollisionWeight;

            acceleration += collisionAvoidForce;
        }

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;

        Vector3 dir = velocity / speed;

        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        
        velocity = dir * speed;

        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
        forward = dir;
    }

    bool IsHeadingForCollision() {
        RaycastHit hit;
        if (Physics.SphereCast(position, fishSize, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask)) {
            return true;
        } else {
            return false;
        }
    }

    Vector3 ObstacleRays() {
        Vector3[] rayDirections = FishHelper.Directions;

        for (int i = 0; i < rayDirections.Length; i++) {
            Vector3 dir = cachedTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, fishSize, settings.collisionAvoidDst, settings.obstacleMask)) {
                return dir;
            }
        }

        return forward;
    }

    Vector3 SteerTowards (Vector3 vector) {
        Vector3 v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude (v, settings.maxSteerForce);
    }
}
