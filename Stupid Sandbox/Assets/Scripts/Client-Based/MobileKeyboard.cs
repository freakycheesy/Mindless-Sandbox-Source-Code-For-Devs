using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MobileKeyboard : MonoBehaviour
{
    TMP_InputField tMP_InputField;

    void Start()
    {
        tMP_InputField = GetComponent<TMP_InputField>();
        tMP_InputField.onSelect.AddListener((string input) =>
        {
            TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        });
    }
}
