using CCGKit;
using UnityEngine;
using UnityEngine.UI;

public class RealEdit : MonoBehaviour
{
    public InputField gold;

    public Text rubles;
    public Text euros;

    Model model;

    int i;

    void Start ()
    { model = GameManager.Instance.model; }

    public void setData(int i)
    {
        gold.text = model.rules.real[i].gold.ToString();
        rubles.text = model.rules.real[i].rubles.ToString()+" руб";
        euros.text = model.rules.real[i].euros.ToString()+" EUR";

        this.i = i;
    }

    public void getData()
    { model.rules.real[i].gold = int.Parse(gold.text); }
}