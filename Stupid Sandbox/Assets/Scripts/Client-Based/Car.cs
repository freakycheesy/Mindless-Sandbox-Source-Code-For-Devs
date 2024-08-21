using Photon.Pun;
using UnityEngine;

public class Car : MonoBehaviourPunCallbacks
{
    // Network/Player Stuff
    public Transform seat;
    public bool isSeated;
    public PhotonView view;
    [SerializeField] private Transform carCameraPosition;
    public float horizontal, vertical;
    public GameObject[] enableGameObjectWhenSeated;

    // Car Physics
    public Rigidbody rb;

    public float forwardAccel = 8, reverseAccel = 4f, maxSpeed = 50f, turnStrength = 180, gravityForce = 10f, dragOnGround = 3f;

    private float speedInput, turnInput;

    private bool grounded = false;

    public LayerMask groundLayer;

    public float rayLength = 0.5f;

    public Transform rayPoint;

    public AudioSource engineSound;

    private void Start() {
        isSeated = false;
        engineSound.enabled = false;
        view = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
        engineSound = GetComponent<AudioSource>();
        transform.rotation = Quaternion.Euler(0 , transform.rotation.eulerAngles.y, 0);
    }

    [PunRPC]
    void ToggleSeat() {
        isSeated = !isSeated;
    }

    private void Update() {
        speedInput = 0f;
        if (!view.AmOwner) {
            return;
        }
        else {
            if (isSeated) {
                foreach (GameObject gameObject in enableGameObjectWhenSeated) {
                    gameObject.SetActive(true);
                }
                if (vertical > 0) {
                    speedInput = vertical * forwardAccel * 1000f;
                }
                else if (vertical < 0) {
                    speedInput = vertical * reverseAccel * 1000f;
                }
                turnInput = horizontal;

                if (grounded) {
                    transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime, 0f));
                }
            }
        }
        if (!isSeated) {
            engineSound.enabled = false;
            speedInput = 0f;
            foreach (GameObject gameObject in enableGameObjectWhenSeated) {
                gameObject.SetActive(false);
            }
        }
    }

    private void FixedUpdate() {
        grounded = false;
        RaycastHit hit;
        if (Physics.Raycast(rayPoint.position, -transform.up, out hit, rayLength, groundLayer)) {
            grounded = true;

            // transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }

        if (grounded) {
            rb.drag = dragOnGround;
            if (Mathf.Abs(speedInput) > 0.25f) {
                rb.AddForce(transform.forward * speedInput);
                engineSound.pitch = 1.5f;
            }
            else {
                engineSound.pitch = 1;
            }
        }
        else {
            rb.drag = 0.25f;
            rb.AddForce(Vector3.up * -gravityForce * 75f);
        }
    }
}
