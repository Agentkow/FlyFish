using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName =("Fish"))]
public class FishStats : ScriptableObject
{
    public string fishName = "Fish";
    public float maxHealth;
    public float weightRange;
    public int armorLevel;
    public float swimSpeed;
    public float territorialness;

    #region Boid Stats
    [Header("Boid Stats")]
    public float minSpeed = 2;
    public float maxSpeed = 5;
    public float perceptionRadius = 2.5f;
    public float avoidanceRadius = 1;
    public float maxSteerForce = 3;
    public float targetWeight = 1;

    [Header("Collisions")]
    public LayerMask obstacleMask;
    public float boundsRadius = .27f;
    public float avoidCollisionWeight = 10;
    public float collisionAvoidDst = 5;
    #endregion
}
