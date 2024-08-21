using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Enemy enemy;
    public GameObject thisGuysRig;
    public Object[] objects;
    public Transform[] hands;
    public PhotonView view;
    private void Awake() {
        GetRagdollBits();
        RagdollModeOff();    
    }

    private void Start() {        
        enemy = GetComponent<Enemy>();   
    }

    public void Die() {
        view.RPC("DieRPC", RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void DieRPC() {
        GetRagdollBits();
        RagdollModeOn();
        for (int i = 0; i < objects.Length; i++) {
            Destroy(objects[i]);
        }
        StartCoroutine(DieDestroy());
    }

    IEnumerator DieDestroy() {
        yield return new WaitForSeconds(5);
        PhotonNetwork.Destroy(gameObject.GetComponent<PhotonView>().gameObject);
    }
    Collider[] RagdollColiders;
    Rigidbody[] LimbsRigitbodies;


    void GetRagdollBits() {
        RagdollColiders = thisGuysRig.GetComponentsInChildren<Collider>();
        LimbsRigitbodies = thisGuysRig.GetComponentsInChildren<Rigidbody>();
    }
    void RagdollModeOn() {
        foreach (Collider col in RagdollColiders) {
            col.enabled = true;
        }
        foreach (Rigidbody rb in LimbsRigitbodies) {
            rb.isKinematic = false;
        }
    }
    void RagdollModeOff() {
        foreach (Collider col in RagdollColiders) {
            col.enabled = false;
        }
        foreach (Rigidbody rb in LimbsRigitbodies) {
            rb.isKinematic = true;
        }
    }
}
