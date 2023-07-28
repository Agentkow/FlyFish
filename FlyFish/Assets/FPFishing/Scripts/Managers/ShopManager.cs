using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopManager : MonoBehaviour
{
    private static ShopManager _sm;
    public static ShopManager sm { get { return _sm; } }
    [field: SerializeField] public List<Slot> hookSlot { get; private set; }
    [field: SerializeField] public List<Slot> poleSlot { get; private set; }
    [field: SerializeField] public List<Slot> licenseSlot { get; private set; }

    [SerializeField] private List<SalesButton> hookButtons, poleButtons, licenseButtons;
    

    private void Start()
    {
        SetUpShopItems();
        MakeShopItems();
    }

    void SetUpShopItems()
    {
        for (int i = 0; i < GameManager.gm.hooks.Count; i++)
        {
            hookSlot.Add(new Slot(GameManager.gm.hooks[i].name,
                                  GameManager.gm.hooks[i], 
                                  GameManager.gm.hooks[i].priceValue, 
                                  GameManager.gm.hooks[i].hookImage));
        }
        for (int i = 0; i < GameManager.gm.poles.Count; i++)
        {
            poleSlot.Add(new Slot(GameManager.gm.poles[i].name, 
                                  GameManager.gm.poles[i],
                                  GameManager.gm.poles[i].priceValue, 
                                  GameManager.gm.poles[i].poleImage));
        }
        for (int i = 0; i < GameManager.gm.fishLicense.Count; i++)
        {
            licenseSlot.Add(new Slot(GameManager.gm.fishLicense[i].name, 
                                     GameManager.gm.fishLicense[i], 
                                     GameManager.gm.fishLicense[i].price, 
                                     GameManager.gm.fishLicense[i].licenseImage));
        }
    }
    void MakeShopItems()
    {
        for (int i = 0; i < hookSlot.Count; i++)
        {
            if (hookSlot[i]!= null)
            {
                SalesButton getButton = hookButtons[i];
                getButton.Initialize(hookSlot[i].itemName, hookSlot[i].priceTag, hookSlot[i].image);
            }

        }
        for (int i = 0; i < poleSlot.Count; i++)
        {
            if (poleSlot[i] != null)
            {
                SalesButton getButton = poleButtons[i];
                getButton.Initialize(poleSlot[i].itemName, poleSlot[i].priceTag, poleSlot[i].image);
            }
            
        }
        for (int i = 0; i < licenseSlot.Count; i++)
        {
            if (licenseSlot[i] != null)
            {
                SalesButton getButton = licenseButtons[i];
                getButton.Initialize(licenseSlot[i].itemName, licenseSlot[i].priceTag, licenseSlot[i].image);
            }
            
        }
    }

#region Purchases
    public void BuyHook(int index)
    {
        if (GameManager.gm.money>= hookSlot[index].priceTag)
        {
            for (int i = 0; i < GameManager.gm.hooks.Count; i++)
            {
                hookSlot[index].bought = true;
                GameManager.gm.MoneyDecrease(hookSlot[index].priceTag);
            }
        }
       
    }
    public void BuyPole(int index)
    {
        if (GameManager.gm.money >= poleSlot[index].priceTag)
        {
            for (int i = 0; i < GameManager.gm.poles.Count; i++)
            {
                poleSlot[index].bought = true;
                GameManager.gm.MoneyDecrease(poleSlot[index].priceTag);
            }
        }
    }
    public void BuyLicense(int index)
    {
        if (GameManager.gm.money >= licenseSlot[index].priceTag)
        {
            for (int i = 0; i < GameManager.gm.fishLicense.Count; i++)
            {
                licenseSlot[index].bought = true;
                GameManager.gm.MoneyDecrease(licenseSlot[index].priceTag);
            }
        }
    }

    #endregion
}

[System.Serializable]
public class Slot
{
    public string itemName;
    public ScriptableObject itemObj;
    public Sprite image;
    public int priceTag;
    public bool bought;

    public Slot(string name, ScriptableObject obj, int cost, Sprite sprite)
    {
        itemName = name;
        image = sprite;
        itemObj = obj;
        priceTag = cost;
        bought = false;
    }
}
