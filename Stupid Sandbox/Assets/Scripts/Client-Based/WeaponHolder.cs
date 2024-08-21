using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour {

    public Transform targetTransform;

    public Vector3 offset;

    [Range(0f, 200f)]
    public float followRate = 50f;

    private void LateUpdate() {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetTransform.localPosition + offset, followRate * Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetTransform.localRotation, followRate * Time.deltaTime);
    }
}
