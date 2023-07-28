using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "License", menuName = ("License"))]
public class Licenses : ScriptableObject
{
   public string location;
   [TextArea(3, 10)]
   public string description;
   public Sprite licenseImage;
   public int level;
   public int price;
}
