using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{
    [SerializeField] private List<FishStats> fishList;
    [SerializeField] private List<Transform> waypoints;
    [SerializeField] private float spawnRadius = 10;

    private void Update()
    {
        if (FireInput())
        {
            SpawnFish();
        }
    }

    public void SpawnFish()
    {
        Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius;
        GameObject fishPrefab = ObjectPool.pool.GetFish();
        if (fishPrefab!= null)
        {
            FishController fish = fishPrefab.GetComponent<FishController>();
            fishPrefab.SetActive(true);
            fishPrefab.transform.position = pos;
            fish.SetWaypoints(waypoints);
            fish.Initialize(fishList[Random.Range(0, fishList.Count)], null);
            fishPrefab.transform.forward = Random.insideUnitSphere * spawnRadius;
            Debug.Log(Random.insideUnitSphere * spawnRadius);
        }
        
    }

    private bool FireInput()
    {
        return Input.GetButtonDown("Spawn");
    }
}
