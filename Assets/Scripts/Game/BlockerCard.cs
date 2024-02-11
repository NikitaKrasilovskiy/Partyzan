using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockerCard : MonoBehaviour
{
    public TextMeshProUGUI number;

    public void SetNumber(int n)
    { number.text = n.ToString(); }

    public void Hide()
    { gameObject.SetActive(false); }
}