using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ErrorMessage : MonoBehaviour
{
    public TextMeshProUGUI text;

    public void ShowMessage(string message)
    {
        Debug.Log(message);
        gameObject.SetActive(true);
        text.text = message;
    }

    public void CloseWindow()
    { gameObject.SetActive(false); }

}