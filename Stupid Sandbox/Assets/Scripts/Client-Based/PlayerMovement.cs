using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    [Header("Assignables")]
    public Transform playerCam;
    public Transform orientation;
    public LayerMask whatIsGround;
    private Rigidbody rb;
    [Header("Input")]
    public InputActionProperty moveInput;
    public InputActionProperty jumpInput;
    public InputActionProperty crouchInput;
    public InputActionProperty lookInput;
    public Animator animator;
    private float xRotation;
    public float sensitivity = 50f;
    private float sensMultiplier = 1.5f;

    [Header("Movement")]
    public float moveSpeed = 1250f;
    public float maxSpeed = 10f;
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;
    public float maxSlopeAngle = 35f;
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f; 
    [NonSerialized] public float x, y;
    [NonSerialized] public bool jumping, crouching;
    private Vector3 normalVector = Vector3.up;

    [Header("Collisions")]
    public bool grounded;
    private Vector2 moveVector;
    public Pause pause;
    public SpawnMenu spawnMenu;
    public PhotonView view;
    CapsuleCollider bodyCollider;
    float tempSpeed;
    float tempHeight;
    float tempMass;

    // Slide Force
    public float slideForce = 250;
    public float slideCounterMovement = 0.2f;

    public static PlayerMovement Instance { get; private set; }

    void Awake()
    {

        Instance = this;

        rb = GetComponent<Rigidbody>();
        pause = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Pause>();
        spawnMenu = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SpawnMenu>();
        PhysicMaterial mat = new PhysicMaterial("tempMat");

        mat.bounceCombine = PhysicMaterialCombine.Average;

        mat.bounciness = 0;

        mat.frictionCombine = PhysicMaterialCombine.Minimum;

        mat.staticFriction = 0;
        mat.dynamicFriction = 0;

        tempMass = rb.mass;

        gameObject.GetComponent<Collider>().material = mat;
    }

    void Start()
    {
        bodyCollider = GetComponent<CapsuleCollider>();
        readyToJump = true;
        tempSpeed = maxSpeed;
        tempHeight = bodyCollider.height;
    }


    private void FixedUpdate()
    {
        Movement();
        // if (Physics.Raycast(transform.position, transform.forward, 0.5f, whatIsGround) && PlayerPrefs.GetInt("Mobile") == 1) Jump();
        //if (Physics.Raycast(transform.position, orientation.forward, out RaycastHit hit, 1f)) {
            //if (hit.collider.GetComponent<Climbable>() == null) {           
                //rb.mass = tempMass;
                //return;
            //}
           //Climb();
        //}    
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.GetComponent<Climbable>() != null) {
            Climb();
        }
        else {
            rb.mass = tempMass;
        }
    }

    private void OnCollisionStay(Collision other) {
        if (other.gameObject.GetComponent<Climbable>() != null) {
            Climb();
        }
        else {
            rb.mass = tempMass;
        }

        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer)))
            return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal)) {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void Update() {
        animator.SetBool("Fall", !grounded);
        // Animation
        if (view.IsMine) {                       
            if (!crouching) {
                if (moveVector.magnitude > 0.025f) {
                    animator.SetBool("Move", true);
                }
                else {
                    animator.SetBool("Move", false);
                }
            }
        }       
        if (!view.IsMine || pause.isPaused || (spawnMenu != null && spawnMenu.isMenuOpened)) {
            moveVector.x = 0;
            moveVector.y = 0;
            x = 0;
            y = 0;
            jumping = false;
            return;
        }     
        MyInput();
        Look();
    }

    private void MyInput()
    {
        moveVector = moveInput.action.ReadValue<Vector2>();
        x = moveVector.x;
        y = moveVector.y;
        jumping = jumpInput.action.IsPressed();
        if (moveVector.magnitude == 0) {
            maxSpeed = tempSpeed;
        }
        if (crouchInput.action.WasPressedThisFrame()) {
            crouching = true;
            view.RPC(nameof(StartCrouch), RpcTarget.AllBufferedViaServer);
        }
        else if (crouchInput.action.WasReleasedThisFrame()) {
            crouching = false;
            view.RPC(nameof(StopCrouch), RpcTarget.AllBufferedViaServer);
        }
        sensMultiplier = PlayerPrefs.GetFloat("Sens");
    }

    [PunRPC]
    private void StartCrouch() {
        animator.SetBool("Crouch", true);
        bodyCollider.height = tempHeight / 1.6f;
        bodyCollider.center = new Vector3(0, 1.75f, 0);
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 0.5f) {
            if (grounded) {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }

    [PunRPC]
    private void StopCrouch() {
        animator.SetBool("Crouch", false);
        bodyCollider.height = tempHeight;
        bodyCollider.center = new Vector3(0, 1.5f, 0);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void Movement() {
        //Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(x, y, mag);

        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping)
            Jump();

        //Set max speed
        float maxSpeed = this.maxSpeed;

        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump) {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed)
            x = 0;
        if (x < 0 && xMag < -maxSpeed)
            x = 0;
        if (y > 0 && yMag > maxSpeed)
            y = 0;
        if (y < 0 && yMag < -maxSpeed)
            y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;

        // Movement in air
        if (!grounded) {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }

        // Movement while sliding
        if (grounded && crouching) multiplierV = 0f;


        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime * multiplier * multiplierV);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime * multiplier);
    }

    private void Jump() {
        if (grounded && readyToJump) {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0)
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump() {
        readyToJump = true;
    }

    private float desiredX;
    private void Look() {
        float mouseX = 0;
        float mouseY = 0;
        if (view.IsMine) {
            mouseX = lookInput.action.ReadValue<Vector2>().x * sensitivity * Time.fixedDeltaTime * sensMultiplier;
            mouseY = lookInput.action.ReadValue<Vector2>().y * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        }

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    private void CounterMovement(float x, float y, Vector2 mag) {
        if (!grounded || jumping)
            return;

        //Slow down sliding
        if (crouching) {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed) {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    public Vector2 FindVelRelativeToLook() {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    private bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < maxSlopeAngle;
    }

    private bool cancellingGrounded;

    private void StopGrounded() {
        grounded = false;
    }

    void Climb() {
        Debug.Log("Climbing");
        if (jumping) {
            rb.mass = tempMass;
            rb.AddForce(Vector3.up, ForceMode.Impulse);
        }
        else {
            rb.mass = tempMass;
        }
    }

}
