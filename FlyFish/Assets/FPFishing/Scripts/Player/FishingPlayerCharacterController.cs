using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingPlayerCharacterController : MonoBehaviour
{
//#pragma warning disable 0414
    [Header("Camera")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float camVertAngle;

    [Header("Movement")]
    [SerializeField] private float mSpeed;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private float gravMod = 0.1f;
    [SerializeField] private Vector3 charVelocity;

    [Header("Grappling hook")]
    [SerializeField] private float hookLeway = 1.5f;
    [SerializeField] private float range = 50f;

    [Header("Plug inspector")]
    [SerializeField] private GameObject hook;
    [SerializeField] private GameObject hookPos;
    [SerializeField] private CableComponent cabComp;
    [SerializeField] private TrailRenderer hookTrail;
    [SerializeField] private Hook hookInfo;
    [SerializeField] private UIManager ui;

    [Header("Equipment")]
    [SerializeField] private HookEquipment e_Hook;
    [SerializeField] private PoleEquipment e_Pole;
    [SerializeField] private int numOfHooks;
    [field: SerializeField] public Inventory bp { get; private set; }

    private float charVelocityY, knockBackCount = 0;
    private Camera cam;
    private Vector3 hitPos, velocityMom;
    private Rigidbody hookRB;
    private SpringJoint joint;
    private FishController fish;
    private Pullable pull;
    private CharacterController cc;


    [field: SerializeField] public PlayerState state { get; private set; }
    public enum PlayerState
    {
        Normal,
        Throw,
        Fly,
        Catch,
        Pulling,
        Return,
    }
    private void Awake()
    {
        if (GameManager.gm)
            Initialize(GameManager.gm.currentHook, GameManager.gm.currentPole, GameManager.gm.currentInv);
        
    }
    public void Initialize(HookEquipment h, PoleEquipment p, Inventory i)
    {
        e_Hook = h;
        e_Pole = p;
        ui.SetTicker(e_Pole.ticker);
        bp = i;
        range = p.lineRange;
        numOfHooks = p.spareHooks;
    }

    private void Start()
    {
        hookRB = hook.gameObject.GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
        cam = transform.Find("Main Camera").GetComponent<Camera>();
        knockBackCount = 0;
        ui.EndFishingMinigame();
        range = e_Pole.lineRange;
        numOfHooks = e_Pole.spareHooks;
        hookTrail.enabled = false;
        mSpeed = moveSpeed;
    }
   
    void Update()
    {
        if (Time.timeScale!=0)
        {
            switch (state)
            {
                case PlayerState.Normal:
                    HandleCharacterLook();
                    HandleCharacterMovement();
                    HookStart();
                    break;
                case PlayerState.Throw:
                    HookThrow();
                    break;
                case PlayerState.Fly:
                    HandleCharacterLook();
                    HookshotMove();
                    break;
                case PlayerState.Catch:
                    CatchMove();
                    CatchRotate();
                    CatchingFish();
                    break;
                case PlayerState.Pulling:
                    CatchRotate();
                    CatchPulling();
                    break;
                case PlayerState.Return:
                    CallbackHook();
                    break;
            }
        }
        

    }
    #region Base Controls
    private void HandleCharacterLook() //camera controls
    {
        if (Input.GetAxisRaw("Mouse X")!=0)
        {
            float lookX = Input.GetAxisRaw("Mouse X");
            float lookY = Input.GetAxisRaw("Mouse Y");

            transform.Rotate(new Vector3(0f, lookX * mouseSensitivity, 0f), Space.Self);
            

            camVertAngle -= lookY * mouseSensitivity;

            camVertAngle = Mathf.Clamp(camVertAngle, -89f, 89f);

            cam.transform.localEulerAngles = new Vector3(camVertAngle, 0, 0);
            var t = transform.rotation;
            t = Quaternion.Slerp(t, new Quaternion(t.x, cam.transform.rotation.y, t.z, t.w), 5 * Time.deltaTime);
        }
       
    }

    private void HandleCharacterMovement() // normal movements
    {
        if (knockBackCount<=0)
        {
            float moveX = Input.GetAxisRaw("Horizontal");
            float moveZ = Input.GetAxisRaw("Vertical");
            
            if (ShiftInput())
            {
                if (mSpeed == moveSpeed)
                    mSpeed = sprintSpeed;
                else
                    mSpeed = moveSpeed;
            }

            charVelocity = transform.right * moveX * mSpeed + transform.forward * moveZ * mSpeed;
            if (cc.isGrounded)
            {
                charVelocityY = 0f;
                if (JumpInput())
                {
                    charVelocityY = jumpSpeed;
                }
            }
        }
        else
        {
            knockBackCount -= Time.deltaTime;
        }


        //apply gravity to velocity
        charVelocityY += (Physics.gravity.y) * Time.deltaTime;

        //apply Jump to move vector
        charVelocity.y = charVelocityY;

        //apply momentum
        charVelocity += velocityMom;

        //move character controller
        cc.Move(charVelocity * Time.deltaTime);

        //damp momentum
        if (velocityMom.magnitude >= 0f)
        {
            float drag = 3f;
            velocityMom -= velocityMom * drag * Time.deltaTime;
            if (velocityMom.magnitude < .01f)
                velocityMom = Vector3.zero;
        }
    }
    #endregion

    #region Hookshot

    private void HookStart()
    {
        //start the throw
        if (HookInput())
        {
            if (numOfHooks>0)
            {
                GetComponent<Rigidbody>().isKinematic = true;
                HookRelease();
                cabComp.TurnOnLine();
                hook.transform.parent = null;
                hookTrail.enabled = true;
                state = PlayerState.Throw;
            }
            
        }
    }
    
    private void HookThrow()
    {
        //hook travels
        hook.GetComponent<Rigidbody>().velocity = cam.transform.forward * e_Hook.flySpeed * Time.fixedDeltaTime;

        
        if (hookInfo.wallCollided) //when hook hits grabbable terrain: grapple hook
        {
            hitPos = hookInfo.hookHit;
            GetComponent<Rigidbody>().isKinematic = false;
            HookStick();
            state = PlayerState.Fly;
        }
        else if (Vector3.Distance(hook.transform.position, transform.position) > range-0.5f) // if hook hits line limit
        {
            HookRelease();
            ResetPosState();
        }
        else if (hookInfo.fishCatch) //if we hit a fish
        {
            if (hookInfo.hitObj.gameObject.GetComponent<FishController>().state == FishController.FishState.Swim)
            {
                if (hookInfo.hitObj.gameObject.GetComponent<FishController>())
                    hookInfo.hitObj.gameObject.GetComponent<FishController>().Hooked(e_Pole.weightRange);
                ResetGravity();

                //make a joint on hook
                joint = hook.gameObject.AddComponent<SpringJoint>();
                joint.autoConfigureConnectedAnchor = false;
                joint.connectedAnchor = transform.position;

                joint.maxDistance = range + hookLeway;
                joint.minDistance = range * 0.8f;

                joint.spring = 1f;
                joint.damper = 1f;
                joint.massScale = 1f;

                //sticks hook to fish
                hook.transform.parent = hookInfo.hitObj.transform;
                HookStick();

                //get the fish object
                fish = hookInfo.hitObj.gameObject.GetComponent<FishController>();
                ui.StartFishingMinigame(fish.GetFishStats());
                fish.StartSweat();
                state = PlayerState.Catch;
            }
            else if (hookInfo.hitObj.gameObject.GetComponent<FishController>().state == FishController.FishState.Stunned)
            {
                hookInfo.hitObj.gameObject.GetComponent<FishController>().StartingCatch();
                ResetGravity();
                ReturnHook();
            }
            
        }
        else if (hookInfo.pullable) //when hook hits a pullable object
        {
            if (hookInfo.hitObj.GetComponent<Pullable>())
            {
                hookInfo.hitObj.GetComponent<Pullable>().StartPulling();
            }
            ResetGravity();

            //make a joint on hook
            joint = hook.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = transform.position;

            joint.maxDistance = range + hookLeway;
            joint.minDistance = range * 0.8f;

            joint.spring = 1f;
            joint.damper = 1f;
            joint.massScale = 1f;

            //sticks hook to pullable object
            hook.transform.parent = hookInfo.hitObj.transform;
            HookStick();
            pull = hookInfo.hitObj.gameObject.GetComponent<Pullable>();
            state = PlayerState.Pulling;
        }
        else if (hookInfo.bad) // hook hits anything else
        {
            HookRelease();
            ResetPosState();
        }

        // cancel mid throw
        if (HookInput()) 
        {
            HookRelease();
            ResetPosState();
        }
    }
    private void HookshotMove()
    {
        Vector3 dir = (hook.transform.position - transform.position).normalized;

        float min = 5f;
        float max = 30f;
        float hookShotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hitPos), min, max);
        float multiplier = 2f;

        cc.Move(dir * hookShotSpeed * multiplier * Time.deltaTime);

        //when player meets hook point
        if (Vector3.Distance(transform.position, hookInfo.hookHit) < hookLeway)
        {
            float extra = 7f;
            velocityMom = dir * hookShotSpeed * extra;
            velocityMom += Vector3.up * jumpSpeed;
            ResetGravity();
            ReturnHook();
        }

        //cancelling grapple
        if (HookInput())
        {
            HookRelease();
            ResetGravity();
            state = PlayerState.Return;
        }

        //jumping mid grapple
        if (JumpInput())
        {
            float extra = 7f;
            velocityMom = dir * hookShotSpeed * extra;
            velocityMom += Vector3.up * jumpSpeed;
            ResetGravity();
            ReturnHook();
        }
    }
    #endregion

    public void Knockback(Transform knockBackFrom, float force)
    {
        Vector3 dir = (transform.position - knockBackFrom.position).normalized;
        dir.y = force;
        charVelocity = dir * force * Time.deltaTime;
    }

    #region Catching phase

    private void CatchRotate() //Camera movement while catching
    {
        //turn towards fish
        var targetRotate = Quaternion.LookRotation(hookInfo.hitObj.transform.position - cam.transform.position);
        Transform t = transform;
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotate, 5 * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation,
            new Quaternion(t.rotation.x, cam.transform.rotation.y, t.rotation.z, t.rotation.w), Time.deltaTime * 10);
    }
    private void CatchMove() //Player movement while catching
    {
        if (knockBackCount<=0)
        {
            float moveX = Input.GetAxisRaw("Horizontal");

            if (e_Pole.weightRange < fish.weight)
                charVelocity = transform.forward * (fish.velocity.magnitude * 0.9f) + transform.right * moveX * mSpeed * 2;
            else if (e_Pole.weightRange >= fish.weight)
                charVelocity = transform.right * moveX * mSpeed * 2;



            if (cc.isGrounded)
            {
                charVelocityY = 0f;
                if (JumpInput())
                {
                    charVelocityY = jumpSpeed;
                }
            }
        }
        else
            knockBackCount -= Time.deltaTime;

        //apply gravity to velocity
        charVelocityY += (gravity * gravMod) * Time.deltaTime;

        //apply Jump to move vector
        charVelocity.y = charVelocityY;

        //apply momentum
        charVelocity += velocityMom;

        //move character controller
        cc.Move(charVelocity * Time.deltaTime);

        //damp momentum
        if (velocityMom.magnitude >= 0f)
        {
            float drag = 3f;
            velocityMom -= velocityMom * drag * Time.deltaTime;
            if (velocityMom.magnitude < .01f)
                velocityMom = Vector3.zero;
        }

    }
    private void CatchingFish()
    {        
        //fishing minigame here
        if (FireInput())
        {
            if (ui.canHook)
            {
                CameraJerk();
                if (ui.sliderVal > ui.minBreakRange && ui.sliderVal <ui.maxBreakRange) // break;
                {
                    fish.TakeDamage(e_Hook.damage * 2.5f, e_Hook.damage);
                    fish.StopSweat();
                    fish.Released();
                    HookBreak();
                }
                else if (ui.sliderVal > ui.minTargetRange && ui.sliderVal < ui.maxTargetRange) // strong;
                {
                    fish.TakeDamage(e_Hook.damage * 2f, e_Hook.damage);
                }
                else// weak;
                {
                    fish.TakeDamage(e_Hook.damage * 0.2f, e_Hook.damage);
                }
                
                ui.ResetTicker();
            }
            
            
        }
       
        if (Vector3.Distance(transform.position, fish.gameObject.transform.position)> range)// get too far from fish
        {
            fish.StopSweat();
            fish.Released();
            HookBreak();

        }

        if (fish.state == FishController.FishState.Stunned) // when fish is knocked out, detach hook
        {
            fish.StopSweat();
            ResetCameraJerk();
            HookReleased();
        }

        if (HookInput()) //cancel catching
        {
            fish.StopSweat();
            fish.Released();
            HookReleased();
        }
    }
    private void CameraJerk()
    {
        var c = cam.transform.rotation;
        cam.transform.rotation = new Quaternion(c.x, c.y, c.z + 0.1f, c.w);
    }

    private void ResetCameraJerk()
    {
        var c = cam.transform.rotation;
        cam.transform.rotation = new Quaternion(c.x, c.y, c.z - 0.1f, c.w);
    }

    private void CatchPulling()
    {
        //pulling minigame here
        if (FireInput())
        {
            CameraJerk();
            pull.TakeDamage(e_Hook.damage);

        }

        if (Vector3.Distance(transform.position, pull.gameObject.transform.position) > range)// get too far from fish
        {
            pull.Released();
            HookBreak();

        }

        if (pull.state == Pullable.PullState.Pulled)
        {
            ResetCameraJerk();
            HookReleased();
        }

        if (HookInput()) //cancel catching
        {
            pull.Released();
            HookReleased();
        }
    }
    private void HookBreak() //hook snaps back
    {
        BreakSpark.Create(hook.transform.position);
        numOfHooks -= 1;
        ui.UpdateHookNum(numOfHooks);
        HookRemove();
        ReturnHook();
    }

    private void HookReleased() //hook is slowly released
    {
        HookRemove();
        ResetPosState();
    }

    private void HookRemove() //Removes hook from target
    {
        Destroy(joint);
        HookRelease();
        hook.transform.parent = null;
        ui.EndFishingMinigame();
    }

    #endregion

    #region Returning the hook
    private void CallbackHook() // fly back
    {
        
        Vector3 dir = (hookPos.transform.position - hook.transform.position).normalized;
        hook.GetComponent<Rigidbody>().velocity = dir * (e_Hook.flySpeed*1.5f)* Time.fixedDeltaTime;

        if (Vector3.Distance(hook.transform.position, hookPos.transform.position) < 1) // when player gets close to grapple point
        {
            ReturnHook();
        }

    }

    private void ReturnHook() //snap back
    {
        hookInfo.ResetHook();
        cabComp.TurnOffLine();
        hook.transform.parent = hookPos.transform;
        hook.transform.position = hookPos.transform.position;
        HookStick();
        state = PlayerState.Normal;
        
    }

    public void ResetPosState()
    {
        ResetGravity();
        state = PlayerState.Return;
    }
    #endregion

    #region Hook attaching
    private void HookStick() //hook sticks to object
    {
        hookRB.isKinematic = true;
        hookTrail.enabled = false;
        hook.transform.localRotation = Quaternion.identity;
        hookRB.collisionDetectionMode = CollisionDetectionMode.Discrete;
    }

    private void HookRelease() // hook releases from object
    {
        hookRB.isKinematic = false;
        hookTrail.enabled = true;
        hookRB.collisionDetectionMode = CollisionDetectionMode.Discrete;
    }
    #endregion

    #region Extra inputs
    private bool HookInput()
    {
        return Input.GetButtonDown("Action");
    }

    private bool JumpInput()
    {
        return Input.GetButtonDown("Jump");
    }

    private bool FireInput()
    {
        return Input.GetButtonDown("Fire1");
    }

    private bool ShiftInput()
    {
        return Input.GetButtonDown("Sprint");
    }
    #endregion
    private void ResetGravity()
    {
        charVelocityY = 0f;
    }

    private void OnApplicationQuit()
    {
        bp.ClearBackpack();
    }

}
