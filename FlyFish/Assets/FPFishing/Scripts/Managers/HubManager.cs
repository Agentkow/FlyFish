using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HubManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moneyDisplay;

    private void Update()
    {
        if (GameManager.gm != null)
            moneyDisplay.text = "Current Balance: " + GameManager.gm.money.ToString() + "$";

    }

    public void HubSellingFish()
    {
        if (GameManager.gm!=null)
            GameManager.gm.SellingAllFish();

    }

    public void BackToTitle()
    {
        if (GameManager.gm != null)
        {
            
            GameManager.gm.ReturnToMainMenu();
        }
    }

    public void GoToField()
    {
        if (GameManager.gm != null)
        {
            GameManager.gm.GoToGame();
        }
    }
}
