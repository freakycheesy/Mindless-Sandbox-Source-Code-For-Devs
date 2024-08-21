using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePanoramaCamera : MonoBehaviour
{
    Vector3 rotateVectorValue;
    float rotateFloatValue;
    [SerializeField] float rotationAmount = 7.5f;
    float tempSpeed;
    [SerializeField] float resetSpeedCooldown = 0f;

    private void Start() {
        tempSpeed = rotationAmount;
    }

    void Update()
    {
        rotateFloatValue += rotationAmount * Time.deltaTime;
        rotateVectorValue = new Vector3(0, rotateFloatValue, 0);
        transform.eulerAngles = rotateVectorValue;
    }

    public void ChangeSpeed(float amount) {
        rotationAmount += amount;
        Invoke(nameof(ResetSpeed), resetSpeedCooldown);
    }

    public void ResetSpeed() {
        rotationAmount = Mathf.Lerp(rotationAmount, tempSpeed, 50 * Time.deltaTime);
    }
}
