using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{

    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float spawnRadius = 10;
    [field: SerializeField] public ObjectPool.FishType typeOfFishSpawner { get; private set; }

    private void Start()
    {
        while (ObjectPool.pool.GetFish(typeOfFishSpawner) != null)
        {
            SpawnFish();
        }
            
    }

    private void Update()
    {
        if (FireInput())
            SpawnFish();

    }

    public void SpawnFish()
    {
        Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
        GameObject fishPrefab = ObjectPool.pool.GetFish(typeOfFishSpawner);
        if (fishPrefab!= null)
        {
            FishController fish = fishPrefab.GetComponent<FishController>();
            fishPrefab.SetActive(true);
            fishPrefab.transform.position = pos;
            fish.Initialize(waypoints);
            fishPrefab.transform.forward = Random.insideUnitSphere * spawnRadius;
        }
        
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

    private bool FireInput()
    {
        return Input.GetButtonDown("Spawn");
    }
}
