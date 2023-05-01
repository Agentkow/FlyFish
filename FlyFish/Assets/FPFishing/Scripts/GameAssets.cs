using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
    //this code has to be on a game object, but does not have to be in the scene
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
    public GameObject breakSpark;
    public GameObject fish;
    public GameObject coin;

}
