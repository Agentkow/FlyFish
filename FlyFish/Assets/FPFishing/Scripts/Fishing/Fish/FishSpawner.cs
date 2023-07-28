using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawner : MonoBehaviour
{

    [SerializeField] private List<Transform> commonFishWayPoints;
    [SerializeField] private List<Transform> rareFishWayPoints;
    [SerializeField] private const float spawnRadius = 10;

    public void SpawnFish(ObjectPool.FishType typeOfFishSpawner)
    {
        Vector3 pos = transform.position + Random.insideUnitSphere * spawnRadius; //gets a random spawn position
        GameObject fishPrefab = ObjectPool.pool.GetFish(typeOfFishSpawner);
        if (fishPrefab!= null)
        {
            FishController fish = fishPrefab.GetComponent<FishController>();
            fishPrefab.SetActive(true);
            fishPrefab.transform.position = pos;
            if (fish.GetFishStats().rarity == FishStats.FishTypeSpawn.Common)
                fish.Initialize(commonFishWayPoints);
            else if (fish.GetFishStats().rarity == FishStats.FishTypeSpawn.Rare)
                fish.Initialize(rareFishWayPoints);

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
