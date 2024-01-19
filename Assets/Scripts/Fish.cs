using UnityEngine;
using UnityEngine.Serialization;

public class Fish : MonoBehaviour
{
    [HideInInspector]
    // Size and mass of the fish
    public float fishSize;

    // State
    public Vector3 position;
    public Vector3 forward;
    public Vector3 avgFlockHeading;
    public Vector3 avgAvoidanceHeading;
    public Vector3 centerOfFlockmates;
    public int numPerceivedFlockmates;

    public float health;
    public float healthDecay;

    public float hunger;
    [FormerlySerializedAs("hungerDecay")] public float hungerIncrease;

    public float mass;

    public float neighborDensity;

    private WaterQualityZone[] _waterQualityZones;

    // To update:
    private Vector3 acceleration;
    private Transform cachedTransform;

    // Cached
    private Material material;

    [HideInInspector] private FishSettings settings;

    private Food target;
    private Vector3 velocity;

    private void Awake()
    {
        cachedTransform = transform;
        material = cachedTransform.GetComponentInChildren<MeshRenderer>().material;
    }

    public void SetWaterQualityZones(WaterQualityZone[] waterQualityZones)
    {
        _waterQualityZones = waterQualityZones;
    }

    public void Initialize(FishSettings settings)
    {
        this.settings = settings;

        health = Random.Range(settings.minHealth, settings.maxHealth);
        hunger = 0.0f;
        fishSize = Random.Range(settings.minSize, settings.maxSize);
        healthDecay = Random.Range(settings.minHealthDecay, settings.maxHealthDecay);
        hungerIncrease = Random.Range(settings.minHungerIncrease, settings.maxHungerIncrease);

        cachedTransform.localScale = Vector3.one * fishSize;

        position = cachedTransform.position;
        forward = cachedTransform.forward;

        mass = fishSize * settings.density * Random.Range(0.9f, 1.1f);

        neighborDensity = 12f;

        var startSpeed = Random.Range(settings.minSpeed, settings.maxSpeed);
        velocity = transform.forward * startSpeed;
    }

    public void SetColor(Color col)
    {
        if (material != null) material.color = col;
    }

    private void FindFood()
    {
        if (target != null) return;

        var foods = FindObjectsOfType<Food>();

        if (foods.Length == 0) return;

        var minDistanceSquared = Mathf.Infinity;

        foreach (var food in foods)
        {
            var distanceSquared = (food.transform.position - position).sqrMagnitude;
            if (distanceSquared < minDistanceSquared)
            {
                minDistanceSquared = distanceSquared;
                target = food;
            }
        }
    }

    private bool HandleDeath()
    {
        if (health <= 0)
        {
            var risingDirection = Vector3.up;

            if (Physics.Raycast(cachedTransform.position, risingDirection, out var hit, Mathf.Infinity,
                    settings.obstacleMask))
            {
                if (hit.distance > 0.1f) cachedTransform.position += risingDirection * (fishSize * Time.deltaTime);

                if (cachedTransform.rotation.eulerAngles.z < 90)
                    cachedTransform.Rotate(Vector3.forward * (Time.deltaTime * 10));
            }

            return true;
        }
        else return false;
    }

    private void HandleHunger()
    {
        hunger += hungerIncrease * Time.deltaTime;
        
        if (target != null)
        {
            var offsetToTarget = target.transform.position - position;

            if (offsetToTarget.sqrMagnitude < fishSize * fishSize)
            {
                target.ConsumeBy(this);
                target = null;
                return;
            }
            
            acceleration = SteerTowards(offsetToTarget) * settings.targetWeight;
        }
        else
        {
            if (hunger > settings.hungerThreshold) FindFood();
        }
    }

    private void HandleWaterQuality()
    {
        if (_waterQualityZones.Length != 0)
        {
            var offsetToWaterQualityZone = Vector3.zero;
            var waterQualityZoneTemperature = settings.baseTemperature;
            var waterQualityZonePh = settings.basePh;
            
            foreach (var waterQualityZone in _waterQualityZones)
            {
                if (!waterQualityZone.IsInside(position)) continue;

                var zonePosition = waterQualityZone.transform.position;
                Debug.DrawLine(position, zonePosition, Color.red);
                var outwardDirection = waterQualityZone.GetOutwardDirection(position);
                var distance = Vector3.Distance(position, zonePosition);

                var tempProbability = waterQualityZone.GetTemperatureProbability(position, settings.baseTemperature);
                var phProbability = waterQualityZone.GetPhProbability(position, settings.basePh);

                health -= (tempProbability + phProbability) * healthDecay * Time.deltaTime;
                
                offsetToWaterQualityZone += outwardDirection * distance;
            }

            acceleration += SteerTowards(offsetToWaterQualityZone) * settings.fleeWeight;
        }
    }

    private void HandleFlocking()
    {
        if (numPerceivedFlockmates != 0)
        {
            centerOfFlockmates /= numPerceivedFlockmates;

            var offsetToFlockmatesCenter = centerOfFlockmates - position;

            Debug.DrawLine(position, centerOfFlockmates, Color.green);

            var alignmentForce = SteerTowards(avgFlockHeading) * settings.alignWeight;
            var cohesionForce = SteerTowards(offsetToFlockmatesCenter) * settings.cohesionWeight;
            var separationForce = SteerTowards(avgAvoidanceHeading) * settings.seperateWeight;

            acceleration += alignmentForce + cohesionForce + separationForce;
        }
    }

    private void HandleCollision()
    {
        if (IsHeadingForCollision())
        {
            var collisionAvoidDir = ObstacleRays();
            var collisionAvoidForce = SteerTowards(collisionAvoidDir) * settings.avoidCollisionWeight;

            acceleration += collisionAvoidForce;
        }
    }

    private void HandlePhysics()
    {
        velocity += acceleration * Time.deltaTime;
        var speed = velocity.magnitude;
        var dir = velocity / speed;
        speed = Mathf.Clamp(speed, settings.minSpeed, settings.maxSpeed);
        velocity = dir * speed;
        cachedTransform.position += velocity * Time.deltaTime;
        cachedTransform.forward = dir;
        position = cachedTransform.position;
        forward = dir;
    }
    
    public void UpdateFish()
    {
        acceleration = Vector3.zero;
        
        // Death
        bool isDead = HandleDeath();
        if (isDead) return;
        
        // Food
        HandleHunger();
        
        // Water quality zones
        HandleWaterQuality();
        
        // Flocking
        HandleFlocking();
        
        // Collision avoidance
        HandleCollision();
        
        // Physics
        HandlePhysics();
    }

    private bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(position, fishSize, forward, out hit, settings.collisionAvoidDst, settings.obstacleMask))
            return true;
        return false;
    }

    private Vector3 ObstacleRays()
    {
        var rayDirections = FishHelper.Directions;

        for (var i = 0; i < rayDirections.Length; i++)
        {
            var dir = cachedTransform.TransformDirection(rayDirections[i]);
            var ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, fishSize, settings.collisionAvoidDst, settings.obstacleMask)) return dir;
        }

        return forward;
    }

    private Vector3 SteerTowards(Vector3 vector)
    {
        var v = vector.normalized * settings.maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, settings.maxSteerForce);
    }
}