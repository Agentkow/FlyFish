using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPool : MonoBehaviour
{
    [field: SerializeField] public static ObjectPool pool { get; private set; }

    public enum FishType {common, rare, boss}
    [System.Serializable] public class PoolItemGroup
    {
        public FishType typeOfFish;
        public List<GameObject> fishPrefab;
    }
    [SerializeField] private List<FishStats> commonFishList;
    [SerializeField] private List<FishStats> rareFishList;

    [Header("Object Lists")]
    [SerializeField] private List<GameObject> damageObjects;
    [SerializeField] private List<GameObject> breakSparkObjects;
    [SerializeField] private List<PoolItemGroup> allFishObjects;

    [Header("Values")]
    [SerializeField] private int poolAmount = 20;


    private void Awake()
    {
        pool = this;
        SetDamagePopups();
        SetFishList();
        SetBreakPopups();
    }

    public void Initialize(List<FishStats> common, List<FishStats> rare)
    {
        commonFishList = common;
        rareFishList = rare;
        SetDamagePopups();
        SetFishList();
    }

    #region Set Object Pools
    private void SetFishList()
    {
        //common fish
        GameObject holder = AddHolder(GameAssets.i.fish.gameObject, transform);
        List<GameObject> commonFishObjects = new List<GameObject>();
        GameObject fish;
        if (commonFishList.Count>0)
        {
            for (int f = 0; f < commonFishList.Count; f++)
            {
                for (int i = 0; i < poolAmount; i++) //fish pool
                {
                    fish = Instantiate(GameAssets.i.fish.gameObject);
                    FishController fsh = fish.GetComponent<FishController>();
                    fish.transform.parent = holder.transform;
                    fish.SetActive(false);
                    commonFishObjects.Add(fish);
                    fsh.Setup(commonFishList[f], null);
                }
            }
        }

        List<GameObject> rareFishObjects = new List<GameObject>();
        if (rareFishList.Count>0)
        {
            for (int f = 0; f < rareFishList.Count; f++)
            {
                for (int i = 0; i < poolAmount/5; i++) //fish pool
                {
                    fish = Instantiate(GameAssets.i.fish.gameObject);
                    FishController fsh = fish.GetComponent<FishController>();
                    fish.transform.parent = holder.transform;
                    fish.SetActive(false);
                    rareFishObjects.Add(fish);
                    fsh.Setup(rareFishList[f], null);
                }
            }
        }
        

        allFishObjects.Add(new PoolItemGroup() { typeOfFish = FishType.common, fishPrefab = commonFishObjects });
        allFishObjects.Add(new PoolItemGroup() { typeOfFish = FishType.rare, fishPrefab = rareFishObjects });


    }
    private void SetDamagePopups()
    {
        GameObject holder = AddHolder(GameAssets.i.damagePopup, transform);
        damageObjects = new List<GameObject>();
        GameObject input;
        for (int i = 0; i < poolAmount; i++) //damage popup pool
        {
            input = Instantiate(GameAssets.i.damagePopup);
            input.transform.parent = holder.transform;
            input.SetActive(false);
            damageObjects.Add(input);
        }
    }
    private void SetBreakPopups()
    {
        GameObject holder = AddHolder(GameAssets.i.breakSpark, transform);
        breakSparkObjects = new List<GameObject>();
        GameObject input;
        for (int i = 0; i < poolAmount; i++) //break line popup pool
        {
            input = Instantiate(GameAssets.i.breakSpark);
            input.transform.parent = holder.transform;
            input.SetActive(false);
            breakSparkObjects.Add(input);
        }
    }

    GameObject AddHolder(GameObject go, Transform pooledObjectHolderTransform) //adds the fish to a game object for organization
    {
        if (pooledObjectHolderTransform.Find(go.name + "Holder") == null)
        {
            var thisPooledItemHolder = new GameObject(go.name + "Holder");
            return thisPooledItemHolder;
        }
        else
            return null;

        
    }

    #endregion

    #region Get Object Pools
    public GameObject GetDamageObject() //gets the next available damage popup
    {
        for (int i = 0; i < poolAmount; i++)
        {
            if (!damageObjects[i].activeInHierarchy)
                return damageObjects[i];
        }
        return null;
    }

    public GameObject GetBreakObject() //gets the next available break spark popup
    {
        for (int i = 0; i < poolAmount; i++)
        {
            if (!breakSparkObjects[i].activeInHierarchy)
                return breakSparkObjects[i];
        }
        return null;
    }

    public GameObject GetFish(FishType t) //gets next randomly available fish
    {
        var pfs = GetPoolForFishTypes(t).fishPrefab;
        
        for (int i = 0; i < pfs.Count; i++)
        {
            if (!pfs[i].activeInHierarchy)
                return pfs[i];
        }
        return null;
    }
    public PoolItemGroup GetPoolForFishTypes(FishType rarity) => allFishObjects.FirstOrDefault(o => o.typeOfFish == rarity);

    #endregion

    public float GetCount()
    {
        return allFishObjects.Sum(o => o.fishPrefab.Count);
    }
}
