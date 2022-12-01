using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
    [Header("Plug inspector")]
    [SerializeField] private FishStats fs;
    [SerializeField] private GameObject playerPos;
    [SerializeField] private Collider col;
    [SerializeField] private List<Transform> wayPoints;
    [SerializeField] private Transform nextPoint;
    [SerializeField] private Transform detectPoint;
    [SerializeField] private LayerMask avoid;

    [SerializeField] private float detectRange = 3;
    [SerializeField] private float swimSpeed;
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 forward;
    [field: SerializeField] public Vector3 velocity { get; private set; }

    private Transform cacheTransform;
    private Transform target;
    

    private float health;
    private Rigidbody rb;
    [field: SerializeField] public FishState state { get; private set; }

    public enum FishState
    {
        Swim,
        StruggleHeavy,
        StruggleLight,
        Stunned,
        Caught,
    }

    private void Awake()
    {
        cacheTransform = transform;
    }

    public void Initialize(FishStats fishStates, Transform target) // set up fish
    {
        this.target = target;
        this.fs = fishStates;
        state = FishState.Swim;
        health = fs.maxHealth;
        swimSpeed = fs.swimSpeed;
        position = cacheTransform.position;
        forward = cacheTransform.forward;
        velocity = transform.forward * fs.swimSpeed;
    }

    private void Start()
    {
        col = GetComponent<CapsuleCollider>();
        rb = gameObject.GetComponent<Rigidbody>();
        playerPos = GameObject.Find("Player");
        health = fs.maxHealth;
        nextPoint = RandomWaypoint();
        swimSpeed = fs.swimSpeed;
    }

    public void SetWaypoints(List<Transform> points)
    {
        wayPoints = points;
    }
    private Transform RandomWaypoint()
    {
        int rand = Random.Range(0, (wayPoints.Count));
        Transform RandomWaypoint = wayPoints[rand].transform;
        return RandomWaypoint;
    }
    void Update()
    {
        switch (state)
        { 
            case FishState.Swim:
                Swimming();
                break;
            case FishState.StruggleHeavy:
                StrugglingHeavy();
                break;
            case FishState.StruggleLight:
                break;
            case FishState.Stunned:
                break;
            case FishState.Caught:
                Catching();
                break;
        }
    }


    #region Fish movement AI

    private void Swimming() // free swim mode
    {
        Vector3 acceleration = Vector3.zero;

        //go to next random waypoint
        if (Vector3.Distance(nextPoint.position, transform.position) < 1f)
        {
            rb.rotation = Quaternion.identity;
            nextPoint = RandomWaypoint();
        }

        if (target != null)
        {
            Vector3 offsetToTarget = (target.position - position);
            acceleration = SteerTowards(offsetToTarget) * fs.targetWeight;
        }

        if (IsHeadingForCollision())
        {
            Vector3 collisionAvoidDir = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * fs.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }
        acceleration += (nextPoint.position - transform.position).normalized * fs.territorialness;

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, fs.minSpeed, fs.maxSpeed);
        velocity = dir * speed;

        cacheTransform.position += velocity * Time.deltaTime;
        cacheTransform.forward = dir;
        position = cacheTransform.position;
        forward = dir;

    }

    Vector3 ObstacleRays()
    {
        Vector3[] rayDirections = BoidHelper.directions;

        for (int i = 0; i < rayDirections.Length; i++)
        {
            Vector3 dir = cacheTransform.TransformDirection(rayDirections[i]);
            Ray ray = new Ray(position, dir);
            if (!Physics.SphereCast(ray, fs.boundsRadius, fs.collisionAvoidDst, fs.obstacleMask))
            {
                return dir;
            }
        }


        return forward;
    }
    bool IsHeadingForCollision()
    {
        RaycastHit hit;
        if (Physics.SphereCast(position, fs.boundsRadius, forward, out hit, fs.collisionAvoidDst, fs.obstacleMask))
        {
            return true;
        }
        else { }
        return false;
    }
    Vector3 SteerTowards(Vector3 vector)
    {
        Vector3 v = vector.normalized * fs.maxSpeed - velocity;
        return Vector3.ClampMagnitude(v, fs.maxSteerForce);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(position, fs.boundsRadius);
        
    }

    #endregion

    private void RunFromPlayer()
    {
        Vector3 acceleration = Vector3.zero;

        if (IsHeadingForCollision())
        {
            Vector3 collisionAvoidDir = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * fs.avoidCollisionWeight;
            acceleration += collisionAvoidForce;
        }

        Vector3 accelDir = (transform.position - playerPos.transform.position);
        accelDir = new Vector3(accelDir.x, 0, accelDir.z);
        acceleration += accelDir;

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, fs.minSpeed, fs.maxSpeed*1.5f);
        velocity = dir * speed;

        cacheTransform.position += velocity * Time.deltaTime;
        cacheTransform.forward = dir;
        position = cacheTransform.position;
        forward = dir;

    }

    

    #region Reeling Phase
    public void TakeDamage(float damage, float rate)
    {
        float actualDamage = Mathf.Ceil(damage/fs.armorLevel);
        DamagePopup.Create(transform.position, actualDamage,rate);
        health -= actualDamage;
    }

    private void StrugglingHeavy() //catching phase
    {
        RunFromPlayer();

        if (health <=0)
        {
            GetStunned();
        }
    }

    private void GetStunned()
    {
        col.isTrigger = false;
        rb.velocity = Vector3.zero;
        rb.AddForce(transform.up * 10);
        state = FishState.Stunned;
    }
    #endregion

    #region Retrieval
    public void StartingCatch()
    {
        col.isTrigger = true;
        state = FishState.Caught;
    }

    private void Catching()
    {
        Vector3 dir = (playerPos.transform.position - transform.position).normalized;
        rb.velocity = dir * 5000f * Time.deltaTime;
    }
    #endregion

    #region Public Hooking Functions
    public void Hooked()
    {
        col.isTrigger = true;
        rb.velocity = Vector3.zero;
        state = FishState.StruggleHeavy;
    }

    public void Released()
    {
        if (health > 0)
        {
            col.isTrigger = false;
            rb.rotation = Quaternion.identity;
            state = FishState.Swim;
        }
        else
            GetStunned();
        
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 dir = (transform.position - collision.transform.position).normalized * 10;
        velocity += dir  * Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {

        if (state == FishState.Caught)
        {
            if (other.tag == "Player")
            {
                rb.velocity = Vector3.zero;
                gameObject.SetActive(false);
                col.isTrigger = false;
            }
        }
    }


}
