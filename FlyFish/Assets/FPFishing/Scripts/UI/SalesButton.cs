using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SalesButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI priceTag;
    [SerializeField] private Image itemImage;
    [SerializeField] private Button itemButton;
    public void Initialize(string itemName, float price, Sprite image)
    {
        title.text = itemName;
        priceTag.text = price.ToString("00");
        itemImage.sprite = image;
    }

}
