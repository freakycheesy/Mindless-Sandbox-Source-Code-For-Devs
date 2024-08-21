using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumberToString : MonoBehaviour
{
    [SerializeField]
    TMP_Text tmp_Text;
    public void IntToString(int value) {
        tmp_Text.text = value.ToString();
    }

    public void FloatToString(float value) {
        tmp_Text.text = value.ToString();
    }
}
