using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjectOnDelayForever : MonoBehaviourPun
{
    public Transform parent;
    public GameObject spawnObject;
    public bool canSpawnObject = true;

    private void Start() {
        canSpawnObject = true;
    }

    private void Update()
    {
        if (canSpawnObject) {
            canSpawnObject = false;
            GameObject spawnedObject = PhotonNetwork.Instantiate(spawnObject.name, transform.position, transform.rotation);           
            spawnedObject.GetPhotonView().transform.parent = parent;
            Invoke(nameof(ResetCanSpawnValue), 5f);           
        }
    }

    private void ResetCanSpawnValue() {
        canSpawnObject = true;
    }
}
