using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hook", menuName = ("Hook"))]
public class HookEquipment : ScriptableObject
{
    [TextArea(3, 10)]
    public string description;
    public float damage;
    public Sprite hookImage;
    public float flySpeed;
    public float tension;
    public float armorPenetrationLevel = 1;
    public int priceValue;
}
