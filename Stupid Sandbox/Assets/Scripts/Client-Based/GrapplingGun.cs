using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingGun : MonoBehaviour {

    private LineRenderer lr;
    private Vector3 grapplePoint;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, playerCamera, player;
    private float maxDistance = 100f;
    private SpringJoint joint;
    public InputActionProperty shootInput;
    public PhotonView view;
    public Pause pause;
    public SpawnMenu spawnMenu;

    void Awake() {
        lr = GetComponent<LineRenderer>();
        pause = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Pause>();
        spawnMenu = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SpawnMenu>();
    }
    private void OnDisable() {
        StopGrapple();
    }

    //Called after Update
    void LateUpdate() {
        DrawRope();
    }

    void Update() {
        if (!view.IsMine || pause.isPaused || spawnMenu.isMenuOpened) {
            StopGrapple();
            return;     
        }
        if (shootInput.action.WasPerformedThisFrame()) {
            StartGrapple();
        }
        else if (shootInput.action.WasReleasedThisFrame()) {
            StopGrapple();
        }
    }

    /// <summary>
    /// Call whenever we want to start a grapple
    /// </summary>
    void StartGrapple() {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hit, maxDistance, whatIsGrappleable)) {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            //The distance grapple will try to keep from grapple point. 
            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            //Adjust these values to fit your game.
            joint.spring = 4.5f;
            joint.damper = 7f;
            joint.massScale = 4.5f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
    }


    /// <summary>
    /// Call whenever we want to stop a grapple
    /// </summary>
    void StopGrapple() {
        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;
    
    void DrawRope() {
        //If not grappling, don't draw rope
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);
        
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);
    }

    public bool IsGrappling() {
        return joint != null;
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }
}
