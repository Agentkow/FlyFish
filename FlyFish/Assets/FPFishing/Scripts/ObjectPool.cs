using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [field: SerializeField] public static ObjectPool pool { get; private set; }

    [Header("Object Lists")]
    [SerializeField] private List<GameObject> damageObjects;
    [SerializeField] private List<GameObject> fishList;

    [Header("Values")]
    [SerializeField] private int poolForDamageAmount = 20;
    [SerializeField] private int fishAmount = 20;


    private void Awake()
    {
        pool = this;
    }

    private void Start()
    {
        damageObjects = new List<GameObject>();
        GameObject input;
        for (int i = 0; i < poolForDamageAmount; i++) //damage popup pool
        {
            input = Instantiate(GameAssets.i.damagePopup);
            input.SetActive(false);
            damageObjects.Add(input);
        }

        fishList = new List<GameObject>();
        GameObject fish;
        for (int i = 0; i < fishAmount; i++) //fish pool
        {
            fish = Instantiate(GameAssets.i.fish.gameObject);
            fish.SetActive(false);
            fishList.Add(fish);
        }
    }

    public GameObject GetDamageObject() //gets the next available damage popup
    {
        for (int i = 0; i < poolForDamageAmount; i++)
        {
            if (!damageObjects[i].activeInHierarchy)
                return damageObjects[i];
        }
        return null;
    }

    public GameObject GetFish() //gets next available fish
    {
        for (int i = 0; i < fishAmount; i++)
        {
            if (!fishList[i].activeInHierarchy)
                return fishList[i];
        }
        return null;
    }
}
