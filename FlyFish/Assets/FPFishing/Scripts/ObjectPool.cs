using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPool : MonoBehaviour
{
    [field: SerializeField] public static ObjectPool pool { get; private set; }

    public enum FishType { common, rare, boss}
    [System.Serializable] public class PoolItemGroup
    {
        public FishType typeOfFish;
        public List<GameObject> fishPrefab;
    }
    [SerializeField] private List<FishStats> commonFishList;
    [SerializeField] private List<FishStats> rareFishList;

    [Header("Object Lists")]
    [SerializeField] private List<GameObject> damageObjects;
    [SerializeField] private List<PoolItemGroup> allFishObjects;

    [Header("Values")]
    [SerializeField] private int poolAmount = 20;


    private void Awake()
    {
        pool = this;
    }

    public void Initialize(List<FishStats> common, List<FishStats> rare)
    {
        commonFishList = common;
        rareFishList = rare;
        SetDamagePopups();
        SetFishList();
    }

    private void Start()
    {
        //temp
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
        

        allFishObjects.Add(new PoolItemGroup() { typeOfFish = FishType.common, fishPrefab = commonFishObjects });



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

    GameObject AddHolder(GameObject go, Transform pooledObjectHolderTransform)
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

    public PoolItemGroup GetPoolForFishTypes(FishType T)
    {
        return allFishObjects.First(o => o.typeOfFish == T);
    }

    public GameObject GetFish(FishType t) //gets next randomly available fish
    {
        var pfs = GetPoolForFishTypes(t).fishPrefab;
        for (int i = 0; i < pfs.Count; i++)
        {
            if (!pfs[i].activeInHierarchy)
            {
                return pfs[i];
            }
        }
        return null;
       
        //return pfs.ElementAt(Random.Range(0, pfs.Count));
    }

    #endregion
}
