using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCamera : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] Transform[] spawnPoints;
    int selectedSpawnPoint = 0;
    bool canTeleport = true;

    private void Awake() {
        Teleport();
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
        if (canTeleport) {
            Invoke(nameof(Teleport), 5f);
            canTeleport = false;
        }      
    }

    void Teleport() {
        if (selectedSpawnPoint == spawnPoints.Length - 1) {
            selectedSpawnPoint = 0;
        }
        else {
            selectedSpawnPoint++;
        }
        transform.position = new Vector3(spawnPoints[selectedSpawnPoint].position.x, spawnPoints[selectedSpawnPoint].position.y + 1.75f, spawnPoints[selectedSpawnPoint].position.z);
        transform.rotation = spawnPoints[selectedSpawnPoint].rotation;       
        canTeleport = true;
    }
}
