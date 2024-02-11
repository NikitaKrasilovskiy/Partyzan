using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SellResult : MonoBehaviour
{
    [SerializeField]
    TranslatorScript translatorScript;

    public void ShowResult(int cost)
    {
        gameObject.SetActive(true);
        translatorScript.SetValue(cost);
    }
}