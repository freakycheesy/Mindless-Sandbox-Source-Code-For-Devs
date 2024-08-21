using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 0;

    public void SetDamage(float amount) {
        damage = amount;
        NetworkManager.Destroy(gameObject, 2.5f);
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "IgnoreBullet") return;
        NetworkManager.Destroy(gameObject);
    }
}
