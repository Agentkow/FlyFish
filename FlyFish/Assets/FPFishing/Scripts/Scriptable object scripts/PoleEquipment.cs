using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pole", menuName = ("Pole"))]
public class PoleEquipment : ScriptableObject
{
    public Sprite poleImage;
    [TextArea(3, 10)]
    public string description;
    public int spareHooks = 10;
    public float weightRange;
    public float lineRange;
    public float ticker;
    public int priceValue;
}
