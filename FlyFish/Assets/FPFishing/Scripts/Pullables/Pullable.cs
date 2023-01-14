using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Pullable : MonoBehaviour
{
    [SerializeField] private UnityEvent<float> pulledAction;
    [SerializeField] private float maxHealth = 10;
    [SerializeField] private float health;
    
    [field: SerializeField] public PullState state { get; private set; }

    public enum PullState
    {
        Idle,
        Pulling,
        Pulled
    }

    public void Initiate()
    {
        health = maxHealth;
        state = PullState.Idle;
    }

    private void Start()
    {
        Initiate();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case PullState.Idle:
                break;
            case PullState.Pulling:
                Pulling();
                break;
            case PullState.Pulled:
                break;
        }
    }

    public void StartPulling()
    {
        state = PullState.Pulling;
    }

    public void TakeDamage(float damageDone)
    {
        health -= damageDone;
    }
    private void Pulling()
    {
        if (health<0)
        {
            pulledAction.Invoke(5);
            state = PullState.Pulled;
        }
    }

    public void Released()
    {
        state = PullState.Idle;
    }

    public void ResetPullable()
    {
        state = PullState.Idle;
    }
}
