using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishingPlayerCharacterController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float camVertAngle;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float gravity = -20f;
    [SerializeField] private Vector3 charVelocity;

    [Header("Grappling hook")]
    [SerializeField] private float hookLeway = 1.5f;
    [SerializeField] private float range = 50f;

    [Header("Plug inspector")]
    [SerializeField] private GameObject hook;
    [SerializeField] private GameObject hookPos;
    [SerializeField] private Hook hookInfo;
    [SerializeField] private HookEquipment equippedHook;
    [SerializeField] private PoleEquipment equippedPole;
    [SerializeField] private CharacterController cc;
    [SerializeField] private UIManager ui;

    private float charVelocityY, mSpeed, knockBackCount = 0;
    private Camera cam;
    private Vector3 hitPos, velocityMom;
    private Rigidbody hookRB;
    private SpringJoint joint;
    private FishController fish;

    [field: SerializeField] public PlayerState state { get; private set; }
    public enum PlayerState
    {
        Normal,
        Throw,
        Fly,
        Catch,
        Return,
    }

    public void Initialize(HookEquipment h, PoleEquipment p)
    {
        equippedHook = h;
        equippedPole = p;

    }

    private void Start()
    {
        hookRB = hook.gameObject.GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
        cam = transform.Find("Main Camera").GetComponent<Camera>();

        knockBackCount = 0;
        ui.EndFishingMinigame();
        Cursor.lockState = CursorLockMode.Locked;

    }
   
    void Update()
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
                Catching();
                break;
            case PlayerState.Return:
                //CatchRotate();
                CallbackHook();
                break;
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
                mSpeed = sprintSpeed;
            else
                mSpeed = moveSpeed;


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
        charVelocityY += gravity * Time.deltaTime;

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

    private void HookStart()
    {
        //start the throw
        if (HookInput())
        {
            GetComponent<Rigidbody>().isKinematic = true;
            HookRelease();
            hook.transform.parent = null;
            state = PlayerState.Throw;
        }
    }
    
    private void HookThrow()
    {
        //hook travels
        hook.GetComponent<Rigidbody>().velocity = cam.transform.forward * equippedHook.flySpeed * Time.deltaTime;

        
        if (hookInfo.wallCollided) //when hook hits grabbable terrain
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
                    hookInfo.hitObj.gameObject.GetComponent<FishController>().Hooked();
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
                ui.StartFishingMinigame();
                state = PlayerState.Catch;
            }
            else if (hookInfo.hitObj.gameObject.GetComponent<FishController>().state == FishController.FishState.Stunned)
            {
                hookInfo.hitObj.gameObject.GetComponent<FishController>().StartingCatch();
                ResetGravity();
                ReturnHook();
            }
            
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

    #region Catching phase

    private void CatchRotate()
    {
        //turn towards fish
        var targetRotate = Quaternion.LookRotation(hookInfo.hitObj.transform.position - cam.transform.position);
        Transform t = transform;
        cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotate, 5 * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation,
            new Quaternion(t.rotation.x, cam.transform.rotation.y, t.rotation.z, t.rotation.w), Time.deltaTime * 10);
    }
    private void CatchMove()
    {
        

        if (knockBackCount<=0)
        {
            float moveX = Input.GetAxisRaw("Horizontal");

            charVelocity = transform.forward * (fish.velocity.magnitude * 0.8f) + transform.right * moveX * mSpeed*2;

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
        charVelocityY += gravity * Time.deltaTime;

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
    private void Catching()
    {        
        //fishing minigame here
        if (FireInput())
        {
            if (ui.canHook)
            {
                var c = cam.transform.rotation;
                cam.transform.rotation = new Quaternion(c.x , c.y, c.z + 0.1f, c.w);
                if (ui.sliderVal < ui.minTargetRange)
                {
                    fish.TakeDamage(equippedHook.damage * 0.2f, equippedHook.damage);
                }
                else if (ui.sliderVal >= ui.minTargetRange && ui.sliderVal <= ui.maxTargetRange)
                {
                    fish.TakeDamage(equippedHook.damage * 2f, equippedHook.damage);
                }
                else if (ui.sliderVal > ui.maxTargetRange)
                {
                    fish.TakeDamage(equippedHook.damage * 2.5f, equippedHook.damage);
                    fish.Released();
                    HookBreak();
                }
                ui.ResetTicker();
            }
            
            
        }
       
        if (Vector3.Distance(transform.position, fish.gameObject.transform.position)> range)// get too far from fish
        {
            fish.Released();
            HookBreak();

        }

        if (fish.state == FishController.FishState.Stunned) // when fish is knocked out, detach hook
        {
            HookReleasesFish();
        }

        if (HookInput()) //cancel catching
        {
            fish.Released();
            HookReleasesFish();
        }
    }

    private void HookBreak() //hook snaps back
    {
        HookRemove();
        ReturnHook();
    }

    public void Knockback(Transform knockBackFrom, float force)
    {
        Vector3 dir = (transform.position - knockBackFrom.position).normalized;
        dir.y = force;
        charVelocity = dir * force * Time.deltaTime;
    }

    private void HookReleasesFish()
    {
        HookRemove();
        ResetPosState();
    }

    private void HookRemove()
    {
        var c = cam.transform.rotation;
        cam.transform.rotation = new Quaternion(c.x, c.y, c.z - 0.1f, c.w);
        Destroy(joint);
        HookRelease();
        hook.transform.parent = null;
        ui.EndFishingMinigame();
    }

    #endregion

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

    #region Returning the hook
    private void CallbackHook() // fly back
    {
        hookInfo.ResetHook();
        Vector3 dir = (hookPos.transform.position - hook.transform.position).normalized;

        hook.GetComponent<Rigidbody>().velocity = dir * (equippedHook.flySpeed*2)* Time.deltaTime;
        if (Vector3.Distance(hook.transform.position, hookPos.transform.position) < hookLeway) // when player gets close to grapple point
        {
            hook.transform.parent = hookPos.transform;
            hook.transform.position = hookPos.transform.position;
            HookStick();
            state = PlayerState.Normal;
        }

    }

    private void ReturnHook() //snap back
    {
        hookInfo.ResetHook();
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
        hookRB.collisionDetectionMode = CollisionDetectionMode.Discrete;
    }

    private void HookRelease() // hook releases from object
    {
        hookRB.isKinematic = false;
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
        return Input.GetButton("Sprint");
    }
    #endregion
    private void ResetGravity()
    {
        charVelocityY = 0f;
    }

}
