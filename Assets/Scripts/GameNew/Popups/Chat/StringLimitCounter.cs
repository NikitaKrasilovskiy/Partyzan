using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StringLimitCounter : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI counterText;

    [SerializeField]
    TMP_InputField observableField;

    int lastCalculate = 0;

	void Update ()
    {
        int i = observableField.characterLimit - observableField.text.Length;

        if (i != lastCalculate)
        {
            counterText.text = (i).ToString();
            lastCalculate = i;
        }
	}
}