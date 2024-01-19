using UnityEngine;

public class Food : MonoBehaviour
{
    public float quality;
    public float quantity;
    public float lifeTime;
    private Transform _cachedTransform;
    private Material _material;

    private void Awake()
    {
        _cachedTransform = transform;
        _material = _cachedTransform.GetComponentInChildren<MeshRenderer>().material;
    }

    private void Update()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime < 0f) Destroy(gameObject);
    }

    public void ConsumeBy(Fish fish)
    {
        fish.hunger += quality * quantity;
        Destroy(gameObject);
    }
}