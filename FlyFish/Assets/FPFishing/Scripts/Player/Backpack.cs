using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Backpack", menuName = ("Backpack/Inventory"))]
public class Backpack : ScriptableObject
{
    [field: SerializeField] public float wallet { get; private set; }
    [field: SerializeField] public float fishInventoryLimit { get; private set; }
    [field: SerializeField] public float currentFishAmount { get; private set; }

    [SerializeField] private List<BackpackSlot> fishContainer = new List<BackpackSlot>();


    public void AddFish(string name, float _weight, float _size)
    {
        fishContainer.Add(new BackpackSlot(name, _weight, _size));
        currentFishAmount = fishContainer.Count;
    }

    public void ClearBackpack()
    {
        fishContainer.Clear();
    }

}

[System.Serializable]
public class BackpackSlot
{
    public string fishName = "Fish";
    public float weight;
    public float size;
    public BackpackSlot(string name, float _weight, float _size)
    {
        fishName = name;
        weight = (Mathf.Round(_weight * 100))/100;
        size = (Mathf.Round(_size * 100)) / 100;
    }
}
