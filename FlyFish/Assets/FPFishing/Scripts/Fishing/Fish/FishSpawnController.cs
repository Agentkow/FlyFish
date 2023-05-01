using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishSpawnController : MonoBehaviour
{
    [SerializeField] private FishSpawner fp;

    // Start is called before the first frame update
    void Awake()
    {
        fp = GetComponent<FishSpawner>();
    }
    void Start()
    {
        for (int i = 0; i < ObjectPool.pool.GetCount(); i++)
        {
            if (ObjectPool.pool.GetFish(ObjectPool.FishType.common) !=null)
                fp.SpawnFish(ObjectPool.FishType.common);
            else if (ObjectPool.pool.GetFish(ObjectPool.FishType.rare) != null)
                fp.SpawnFish(ObjectPool.FishType.rare);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private bool FireInput()
    {
        return Input.GetButtonDown("Spawn");
    }
}
