using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PreferTextSize : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI textMeshPro;

    RectTransform rectTransform;

    const int BORDER = 50;

    private void Awake()
    { rectTransform = GetComponent<RectTransform>(); }

    public void SetText(string text)
    {
        textMeshPro.text = text;
        Vector2 v = rectTransform.sizeDelta;
        v.y = textMeshPro.preferredHeight+BORDER;
        rectTransform.sizeDelta = v;
    }
}