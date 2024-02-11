using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrizeCounter : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _textMeshProUGUI;

    public void SetCount(int count)
    { _textMeshProUGUI.text = count.ToString(); }
}