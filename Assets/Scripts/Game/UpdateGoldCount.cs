using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateGoldCount : MonoBehaviour
{
    private TextMeshProUGUI text;

    private int currentValue = -1;

    void Start ()
    { text = gameObject.GetComponent<TextMeshProUGUI>(); }

	void Update ()
    {
        if ((GameManager.Instance.model != null) && (GameManager.Instance.model.user != null) && (GameManager.Instance.model.user.gold != currentValue))
        {
            currentValue = GameManager.Instance.model.user.gold;
            text.text = currentValue.ToString();
        }
	}
}