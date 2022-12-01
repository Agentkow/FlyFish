using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Pole", menuName = ("Pole"))]
public class PoleEquipment : ScriptableObject
{
    public string poleName = "pole";
    public float weightRange;
    public float lineRange;
}
