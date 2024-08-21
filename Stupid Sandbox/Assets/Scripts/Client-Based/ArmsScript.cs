using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmsScript : MonoBehaviour
{
    public Transform leftHandTarget;
    public Transform rightHandTarget;
    public Transform primaryTarget;
    public Transform secondaryTarget;

    private void LateUpdate() {
        leftHandTarget.position = secondaryTarget.position;
        leftHandTarget.rotation = secondaryTarget.rotation;
        rightHandTarget.position = primaryTarget.position;
        rightHandTarget.rotation = primaryTarget.rotation;
    }
}
