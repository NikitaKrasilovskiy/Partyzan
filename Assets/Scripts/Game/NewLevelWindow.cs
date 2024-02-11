using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewLevelWindow : MonoBehaviour
{
    public TextMeshProUGUI number;

    public GameObject imageBuster;
    public GameObject imageBackpack;
    public GameObject imageGold;
    public GameObject imagePremium;
    public TextMeshProUGUI count;
    public static bool mastShow = false;

    public void Close()
    { gameObject.SetActive(false); }

    public void Show(Level l)
    {
        number.text = l.number.ToString();
        imageBuster.SetActive(l.rewardType == "buster");
        imageBackpack.SetActive(l.rewardType == "backpack");
        imageGold.SetActive(l.rewardType == "gold");
        imagePremium.SetActive(l.rewardType == "premium");
        count.text = (l.rewardValue>0) ? ("x " + l.rewardValue.ToString()) : "";
    }
}