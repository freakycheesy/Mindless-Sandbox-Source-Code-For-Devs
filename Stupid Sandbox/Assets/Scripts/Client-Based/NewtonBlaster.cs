using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewtonBlaster : MonoBehaviour
{
    public Animator holderAnimator;
    [SerializeField] PhotonView view;
    [SerializeField] Transform target;
    [SerializeField] float maxGrabDistance = 10f, throwForce = 20f;
    [SerializeField] Transform objectHolder;
    [SerializeField] InputActionProperty grabInput;
    [SerializeField] InputActionProperty launchInput;
    [SerializeField] InputActionProperty rotateInput;
    [SerializeField] ParticleSystem newtonParticle;
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] Transform gunTip;
    [SerializeField] Material multiColorMaterial;
    [SerializeField] Color grabColor, deleteColor;
    private Vector3 currentLinePosition;
    private Vector3 linePoint;
    float rotation;
    Rigidbody grabbedRB;
    PhotonView grabbedView;
    Grabbable grabbable;
    Pause pause;
    SpawnMenu spawnMenu;
    bool isGrabbed;
    bool deleteMode = false;
    TMP_Text ammoText;
    AudioSource audioSource;

    private void Awake() {
        pause = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Pause>();
        spawnMenu = GameObject.FindGameObjectWithTag("Canvas").GetComponent<SpawnMenu>();
        deleteMode = false;
        ammoText = GameObject.FindGameObjectWithTag("Ammo").GetComponent<TMP_Text>();
        audioSource = GetComponent<AudioSource>();
    }
    private void OnDisable() {
        StopNewtonBlaster(false);
        deleteMode = false;
    }
    private void LateUpdate() {
        if(grabbedRB != null && grabbable != null) {
            lineRenderer.enabled = true;
            DrawLine();
        }
        else {
            lineRenderer.SetPosition(0, gunTip.position);
            lineRenderer.SetPosition(1, gunTip.position);
            lineRenderer.enabled = false;
        }
    }
    void Update() {
        holderAnimator.SetBool("Reload", false);
        if (!view.IsMine || pause.isPaused || spawnMenu.isMenuOpened) return;
        if (isGrabbed) {
            if (rotateInput.action.phase == InputActionPhase.Performed) {
                rotation += 150f * Time.deltaTime;
            }
            grabbedRB.position = objectHolder.transform.position;
            grabbedRB.rotation = Quaternion.Euler(grabbedRB.transform.eulerAngles.x, rotation, grabbedRB.transform.eulerAngles.z);
            linePoint = grabbedRB.transform.position;
            if (grabbable.isStatic) {
                grabbable.gameObject.GetComponent<Collider>().enabled = false;
            }
            else {
                grabbable.gameObject.GetComponent<Collider>().enabled = true;
            }
            if (launchInput.action.WasPerformedThisFrame() && !grabbable.isStatic) {
                audioSource.Play();
                StopNewtonBlaster(true);
            }
            ammoText.text = grabbedRB.position.ToString() + grabbedRB.rotation.ToString() + grabbedRB.transform.localScale.ToString();
        }
        else {
            if (rotateInput.action.WasPerformedThisFrame()) {
                deleteMode = !deleteMode;
            }
            if (launchInput.action.WasPerformedThisFrame()) {
                LaunchObject();
            }
            ammoText.text = "Delete Mode: " + deleteMode.ToString();
        }      
        if (deleteMode) {
            multiColorMaterial.color = deleteColor;
            multiColorMaterial.SetColor("_EmissionColor", deleteColor);
        }
        else {
            multiColorMaterial.color = grabColor;
            multiColorMaterial.SetColor("_EmissionColor", grabColor);
        }

        if (grabInput.action.WasPerformedThisFrame()) {
            if (grabbedRB != null) {
                StopNewtonBlaster(false);
            }
            else {
                Grab();               
            }
        }
        if (grabbable == null) {
            if (grabbable != null) {
                return;
            }
            StopNewtonBlaster(false);
        }
    }

    void LaunchObject() {
        RaycastHit hit;
        if (Physics.Raycast(target.position, target.forward, out hit, maxGrabDistance)) {
            Rigidbody tempGrabbedRB = hit.collider.gameObject.GetComponent<Rigidbody>();
            PhotonView tempGrabbedView = hit.collider.gameObject.GetComponent<Grabbable>().photonView;
            Grabbable tempGrabbable = hit.collider.gameObject.GetComponent<Grabbable>();
            if (tempGrabbable.isStatic)
                return;
            if (tempGrabbable != null) {
                audioSource.Play();
                tempGrabbedView.RequestOwnership();
                if (tempGrabbedRB != null) {
                    tempGrabbedRB.AddForce(-hit.normal * throwForce, ForceMode.VelocityChange);
                }
            }
        }
    }

    void StopNewtonBlaster(bool addForce) {
        grabbedRB.isKinematic = false;
        grabbedRB.useGravity = true;
        grabbable.gameObject.GetComponent<Collider>().enabled = true;
        if (addForce) {
            grabbedRB.AddForce(target.transform.forward * throwForce, ForceMode.VelocityChange);
        }
        grabbable = null;
        grabbedRB = null;
        rotation = 0;
        isGrabbed = false;
        newtonParticle.Stop();
    }

    void Grab() {
        RaycastHit hit;
        if (Physics.Raycast(target.position, target.forward, out hit, maxGrabDistance)) {
            grabbedRB = hit.collider.gameObject.GetComponent<Rigidbody>();
            grabbedView = hit.collider.gameObject.GetComponent<Grabbable>().photonView;
            grabbable = hit.collider.gameObject.GetComponent<Grabbable>();
            if (grabbable != null) {
                if (deleteMode) {
                    PhotonNetwork.Destroy(grabbedView.gameObject);
                }
                else {
                    newtonParticle.Play();
                    grabbedView.RequestOwnership();
                    grabbedRB.isKinematic = true;
                    grabbedRB.useGravity = false;
                    rotation = grabbedRB.transform.eulerAngles.y;
                    isGrabbed = true;
                    currentLinePosition = gunTip.position;
                }
            }
        }
    }

    void DrawLine() {
        currentLinePosition = Vector3.Lerp(currentLinePosition, linePoint, Time.deltaTime * 75f);

        lineRenderer.SetPosition(0, gunTip.position);
        lineRenderer.SetPosition(1, currentLinePosition);
    }
}

