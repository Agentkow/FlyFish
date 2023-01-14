using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    [SerializeField] private static GameAssets _i;

    
    public static GameAssets i
    {
        get
        {
            if (_i==null)
                _i = Instantiate(Resources.Load<GameAssets>("GameAssets")).GetComponent<GameAssets>();
            return _i;
        }
    }

    public GameObject damagePopup;
    public GameObject fish;
    public GameObject coin;

}
