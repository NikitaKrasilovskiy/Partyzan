using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterEdit : MonoBehaviour
{
    public InputField cost;
    public InputField size;

    Model model;
    int i;

	void Start ()
    { model = GameManager.Instance.model; }

    public void setData(int i)
    {
        cost.text = model.rules.boosters[i].cost.ToString();
        size.text = model.rules.boosters[i].size.ToString();
        this.i = i;
    }

    public void getData()
    {
        model.rules.boosters[i].cost = int.Parse(cost.text);
        model.rules.boosters[i].size = int.Parse(size.text);
    }
}