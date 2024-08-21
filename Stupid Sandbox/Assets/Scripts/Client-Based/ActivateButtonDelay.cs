using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActivateButtonDelay : MonoBehaviour
{
    // private vars
    private Button button;
    private float elaspedDuration;

    // public vars
    public string extraText;
    public float delay;
    public TMP_Text timerText;

    // Start is called before the first frame update
    void Awake()
    {
        button = GetComponent<Button>();
        elaspedDuration = delay;
        button.interactable = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!button.interactable) {
            elaspedDuration -= Time.deltaTime;
            timerText.text = extraText + " " + Mathf.RoundToInt(elaspedDuration).ToString();
            if (elaspedDuration <= 0) {
                button.interactable = true;
            }
        }
        else {
            elaspedDuration = delay;
            timerText.text = "";
        }
    }
}
