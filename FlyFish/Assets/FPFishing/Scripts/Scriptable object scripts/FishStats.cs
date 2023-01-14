using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName =("Fish"))]
public class FishStats : ScriptableObject
{
    public enum FishTypeSpawn
    {
        Common,
        Rare
    }

    public string fishName = "Fish";
    public FishTypeSpawn rarity;
    public float maxHealth;
    public float weightRange;
    public float radiusRange;
    public float heightRange;
    public float minRandom;
    public float maxRandom;
    public int armorLevel;
    public float territorialness;
    public GameObject model;

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
