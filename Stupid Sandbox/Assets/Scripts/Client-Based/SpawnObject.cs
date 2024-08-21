using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class SpawnObject : MonoBehaviour
{
    private Grabbable[] grabbables;
    public Transform target;
    [SerializeField]
    Transform objectTransform;

    [Header("Scale")]

    [SerializeField] private TMP_InputField ScaleXInputField;
    [SerializeField] private TMP_InputField ScaleYInputField;
    [SerializeField] private TMP_InputField ScaleZInputField;

    [Header("Rotation")]

    [SerializeField] private TMP_InputField RotationXInputField;
    [SerializeField] private TMP_InputField RotationYInputField;
    [SerializeField] private TMP_InputField RotationZInputField;

    private void Update() {
        grabbables = FindObjectsOfType<Grabbable>();
        foreach (Grabbable grabbable in grabbables) {
            grabbable.transform.SetParent(objectTransform);
        }
        if (target != null) return;
        target = GameObject.FindGameObjectWithTag("objHolder").GetComponent<Transform>();
    }
    public void SpawnGameObject(GameObject gameObject) {
        float rotationXParsed = 0;
        float rotationYParsed = 0;
        float rotationZParsed = 0;
        if (RotationXInputField.text != "") {
            float.TryParse(RotationXInputField.text, out rotationXParsed);
        }
        if (RotationYInputField.text != "") {
            float.TryParse(RotationYInputField.text, out rotationYParsed);
        }
        if (RotationZInputField.text != "") {
            float.TryParse(RotationZInputField.text, out rotationZParsed);
        }
        float rotationX = rotationXParsed;
        float rotationY = rotationYParsed;
        float rotationZ = rotationZParsed;
        Vector3 rotationVector = new Vector3(rotationX, rotationY, rotationZ);
        GameObject gO = PhotonNetwork.Instantiate(gameObject.name, target.position, Quaternion.Euler(rotationVector));
        float scaleXParsed = 1;
        float scaleYParsed = 1;
        float scaleZParsed = 1;
        if (ScaleXInputField.text != "") {
            float.TryParse(ScaleXInputField.text, out scaleXParsed);
        }
        if (ScaleYInputField.text != "") {
            float.TryParse(ScaleYInputField.text, out scaleYParsed);
        }
        if (ScaleZInputField.text != "") {
            float.TryParse(ScaleZInputField.text, out scaleZParsed);
        }
        float scaleX = scaleXParsed;
        float scaleY = scaleYParsed;
        float scaleZ = scaleZParsed;
        gO.transform.localScale = new Vector3(gO.transform.localScale.x * scaleX, gO.transform.localScale.y * scaleY, gO.transform.localScale.z * scaleZ);       
        gO.transform.SetParent(objectTransform);
    }

    public void DeleteGameObjects() {
        int i = 0;
        foreach (Transform objectGameObject in objectTransform) {
            PhotonNetwork.Destroy(objectGameObject.GetComponent<PhotonView>());
            i++;
        }
    }
}
