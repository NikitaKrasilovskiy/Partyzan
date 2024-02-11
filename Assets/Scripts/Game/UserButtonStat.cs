using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UserButtonStat : MonoBehaviour
{
    public TextMeshProUGUI Level;
    public TextMeshProUGUI Name;

    public List<GameObject> Progress;

	void Update ()
    {
        Model model = GameManager.Instance.model;

        if (model == null) return;

        if (model.user == null) return;

        Level.text = model.user.level.ToString();
        Name.text = model.user.name;

        int n = Progress.Count * model.user.exp / model.user.needExp;

        for (int i=0;i< Progress.Count;i++)
        { Progress[i].SetActive(i < n); }
    }
}