using CCGKit;
using UnityEngine;
using UnityEngine.UI;

public class PremiumEdit : MonoBehaviour
{
    public InputField cost;
    public InputField time;
    public InputField extraExp;
    public InputField extraGold;

    Model model;

    int i;

    void Start ()
    { model = GameManager.Instance.model; }

    public void setData(int i)
    {
        cost.text = model.rules.premium[i].cost.ToString();
        time.text = model.rules.premium[i].time.ToString();
        extraExp.text = model.rules.premium[i].extraExp.ToString();
        extraGold.text = model.rules.premium[i].extraGold.ToString();
        this.i = i;
    }

    public void getData()
    {
        model.rules.premium[i].cost = int.Parse(cost.text);
        model.rules.premium[i].time = int.Parse(time.text);
        model.rules.premium[i].extraExp = int.Parse(extraExp.text);
        model.rules.premium[i].extraGold = int.Parse(extraGold.text);
    }
}