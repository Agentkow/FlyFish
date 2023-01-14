using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Hook", menuName = ("Hook"))]
public class HookEquipment : ScriptableObject
{
    public float damage;
    public float flySpeed;
    public float tension;
    public float armorPenetrationLevel = 1;
}
