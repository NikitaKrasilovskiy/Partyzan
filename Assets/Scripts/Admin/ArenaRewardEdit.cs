using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaRewardEdit : MonoBehaviour
{
    public InputField from;
    public InputField to;
    public InputField gold;
    public InputField exp;
    Model model;
    int i;

    void Start ()
    { model = GameManager.Instance.model; }

    public void setData(int i)
    {
        from.text = model.rules.arenaRewards[i].from.ToString();
        to.text = model.rules.arenaRewards[i].to.ToString();
        gold.text = model.rules.arenaRewards[i].gold.ToString();
        exp.text = model.rules.arenaRewards[i].exp.ToString();
        this.i = i;
    }

    public void getData()
    {
        model.rules.arenaRewards[i].from = int.Parse(from.text);
        model.rules.arenaRewards[i].to = int.Parse(to.text);
        model.rules.arenaRewards[i].gold = int.Parse(gold.text);
        model.rules.arenaRewards[i].exp = int.Parse(exp.text);
    }
}