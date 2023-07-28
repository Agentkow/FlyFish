using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Inventory", menuName = ("Inventory"))]
public class Inventory : ScriptableObject
{
    [field: SerializeField] public float wallet { get; private set; }
    [field: SerializeField] public float fishInventoryLimit { get; private set; }
    [field: SerializeField] public float currentFishAmount { get; private set; }

    [field: SerializeField] public List<BackpackSlot> fishContainer { get; private set; }

    private float fullPayout;


    public void AddFish(string name, float _weight, float _size, FishStats stats)
    {
        fishContainer.Add(new BackpackSlot(name, _weight, _size, stats));
        currentFishAmount = fishContainer.Count;
    }

    public float SellFish(int choice)
    {
        BackpackSlot fishChoice = fishContainer[choice];
        float cash = fishChoice.fs.value * fishChoice.weight * fishChoice.size;
        fishContainer.Remove(fishContainer[choice]);
        return cash;
    }
    public float SellAllFish()
    {
        fullPayout = 0;
        for (int i = 0; i < fishContainer.Count; i++)
        {
            float cash = fishContainer[i].fs.value * fishContainer[i].weight * fishContainer[i].size;
            fullPayout += cash;
        }

        ClearBackpack();
        return fullPayout;

    }

    public void ClearBackpack()
    {
        fishContainer.Clear();
        currentFishAmount = fishContainer.Count;
    }

}

[System.Serializable]
public class BackpackSlot
{
    public string fishName = "Fish";
    public float weight;
    public float size;
    public FishStats fs;
    public BackpackSlot(string name, float _weight, float _size, FishStats stats)
    {
        fishName = name;
        weight = (Mathf.Round(_weight * 100)) / 100;
        size = (Mathf.Round(_size * 100)) / 100;
        fs = stats;
    }

}


