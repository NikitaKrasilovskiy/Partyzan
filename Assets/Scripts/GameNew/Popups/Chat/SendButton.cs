using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SendButton : MonoBehaviour
{
    [SerializeField]
    InputField.OnChangeEvent onSend;

    [SerializeField]
    TMP_InputField _InputField;

    public void Send()
    {
        onSend.Invoke(_InputField.text);
        _InputField.text = "";
    }
}