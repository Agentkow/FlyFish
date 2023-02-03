using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
    [Header("Plug inspector")]
    [SerializeField] private FishStats fs;
    [SerializeField] private GameObject playerPos;
    [SerializeField] private CapsuleCollider col;
    [SerializeField] private List<Transform> wayPoints;
    [SerializeField] private Transform nextPoint;
    [SerializeField] private LayerMask avoid;
    [SerializeField] private GameObject model;
    [SerializeField] private float swimSpeed;
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 forward;
    [SerializeField] private float health;

    [field: SerializeField] public float weight { get; private set; }
    [field: SerializeField] public Vector3 velocity { get; private set; }

    private Transform cacheTransform;
    private Transform target;
    
    private Rigidbody rb;
    [field: SerializeField] public FishState state { get; private set; }

    public enum FishState
    {
        Swim,
        Struggle_Overweight,
        Struggle_Underweight,
        Stunned,
        Caught,
    }

    private void Awake()
    {
        cacheTransform = transform;
    }

    public void Setup(FishStats fish, Transform target) // set up fish when instantiated
    {
        this.target = target;
        fs = fish;
        model = Instantiate(fs.model);
        model.transform.position = transform.position;
        model.transform.parent = gameObject.transform;
        position = cacheTransform.position;
        forward = cacheTransform.forward;
        col = GetComponent<CapsuleCollider>();
    }
    public void Initialize(List<Transform> points) // called by spawner: sets territory
    {
        state = FishState.Swim;
        health = fs.maxHealth;
        wayPoints = points;
        float randSize = (fs.weightRange + Random.Range(-fs.minSpeed, fs.maxSpeed)) * 0.1f;
        float randomizer = Random.Range(-fs.minRandom, fs.maxRandom);
        model.transform.localScale *= randSize;
        col.radius = fs.radiusRange + (randomizer * 0.1f);
        col.height = fs.heightRange + (randomizer * 0.1f);
        weight = fs.weightRange + randomizer;
        nextPoint = RandomWaypoint();
    }
    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        playerPos = GameObject.Find("Player");
        
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
            case FishState.Struggle_Overweight:
                Struggling_PullingPlayer();
                break;
            case FishState.Struggle_Underweight:
                Struggling_PlayerStronger();
                break;
            case FishState.Stunned:
                SlowDown();
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

        
        if (Vector3.Distance(nextPoint.position, transform.position) < 1f)
        {
            rb.rotation = Quaternion.identity;
            nextPoint = RandomWaypoint();
        }

        if (target != null) //go to next random waypoint
        {
            Vector3 offsetToTarget = (target.position - position);
            acceleration = SteerTowards(offsetToTarget) * fs.targetWeight;
        }

        if (IsHeadingForCollision()) //if obstacle detected
        {
            Vector3 collisionAvoidDir = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * fs.avoidCollisionWeight;
            acceleration += collisionAvoidForce; //move away from obstacle
        }

        acceleration += (nextPoint.position - transform.position).normalized * fs.territorialness;

        velocity += acceleration * Time.deltaTime;
        swimSpeed = velocity.magnitude;
        Vector3 dir = velocity / swimSpeed;
        swimSpeed = Mathf.Clamp(swimSpeed, fs.minSpeed, fs.maxSpeed);
        velocity = dir * swimSpeed;

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
    bool IsHeadingForCollision() //obstacle detection
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
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * fs.avoidCollisionWeight*2;
            acceleration += collisionAvoidForce;
        }
        
        Vector3 accelDir = (transform.position - playerPos.transform.position );
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

    private void FightAgainstPlayer()
    {
        Vector3 acceleration = Vector3.zero;

        if (IsHeadingForCollision())
        {
            Vector3 collisionAvoidDir = ObstacleRays();
            Vector3 collisionAvoidForce = SteerTowards(collisionAvoidDir) * fs.avoidCollisionWeight * 2;
            acceleration += collisionAvoidForce;
        }

        if (transform.position.y>=playerPos.transform.position.y)
        {
            acceleration += (new Vector3(Random.Range(-10, 10), Random.Range(-10, 10), Random.Range(-10, 10))) * 5;
        }
        else
        {
            acceleration += (new Vector3(Random.Range(-10, 10), Random.Range(0, 10), Random.Range(-10, 10))) * 5;
        }
        

        velocity += acceleration * Time.deltaTime;
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, fs.minSpeed, fs.maxSpeed * 1.5f);
        velocity = dir * speed;

        cacheTransform.position += velocity * Time.deltaTime;
        cacheTransform.forward = dir;
        position = cacheTransform.position;
        forward = dir;

    }

    private void SlowDown()
    {
        Vector3 move = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime);
        transform.Translate(move * Time.deltaTime);
    }

    #region Reeling Phase
    public void TakeDamage(float damage, float rate)
    {
        float actualDamage = Mathf.Ceil(damage/fs.armorLevel);
        
        DamagePopup.Create(transform.position, actualDamage,rate);
        health -= actualDamage;
    }

    //catching phase
    private void Struggling_PullingPlayer() 
    {
        RunFromPlayer();

        if (health <=0)
            GetStunned();
    }

    private void Struggling_PlayerStronger() 
    {
        FightAgainstPlayer();
        if (health <= 0)
            GetStunned();
    }
    private void GetStunned()
    {
        col.isTrigger = false;
        rb.velocity = Vector3.zero;
        rb.useGravity = true;
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
    public void Hooked(float weightCompare)
    {
        col.isTrigger = true;
        rb.velocity = Vector3.zero;
        if (weightCompare< weight)
        {
            state = FishState.Struggle_Overweight;
        }
        else if (weightCompare>= weight)
        {
            state = FishState.Struggle_Underweight;
        }
        
    }

    public void Released()
    {
        if (health > 0)
        {
            col.isTrigger = false;
            rb.velocity = Vector3.zero;
            rb.rotation = Quaternion.identity;
            state = FishState.Swim;
        }
        else
            GetStunned();
        
    }
    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 dir = (transform.position - collision.transform.position).normalized * 100;
        velocity+=(dir * Time.deltaTime);
    }
    private void OnTriggerEnter(Collider other)
    {

        if (state == FishState.Caught||state == FishState.Stunned)
        {
            if (other.tag == "Player")
            {
                Backpack pack = other.GetComponent<FishingPlayerCharacterController>().bp;
                if (pack.currentFishAmount <pack.fishInventoryLimit)
                {
                    other.GetComponent<FishingPlayerCharacterController>().bp.AddFish(fs.fishName, weight, model.transform.localScale.x, fs);
                    rb.velocity = Vector3.zero;
                    gameObject.SetActive(false);
                    col.isTrigger = false;
                }

            }
        }
    }


}
