using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Purchasing;

public class ShopGood : MonoBehaviour
{
    public int ID;
    public GameObject[] background;
    public GameObject SaleBackground;
    public TextMeshProUGUI Sale;
    public GameObject CountBackground;
    public TextMeshProUGUI GoldCount;
    public TextMeshProUGUI CountText;
    public GameObject MoneyBackground;
    public TextMeshProUGUI MoneyCount;
    public GameObject goldCoin;

    [SerializeField]
    string purchaseId;

    void Start ()
    { Show(); }

    public void Show()
    {
        Model model = GameManager.Instance.model;
        Show(model.rules.goods[ID]);
    }

    public void Show(Good good)
    {
        //if (good.discount==0) {
        //    SaleBackground.SetActive(false);
        //} else {
        //    SaleBackground.SetActive(true);
        //    Sale.text = good.discount.ToString() + "%";
        //}
       // Sale.text = "%...";

        if (good.real)
        {
            // MoneyCount.text = good.cost.ToString() + " руб";
            // GoldCount.text = good.cost.ToString();
        }
        else
        {
            MoneyCount.text = good.cost.ToString();
            GoldCount.text = good.cost.ToString();
        }

        if (CountText!=null) CountText.text = good.count.ToString();
       // if (CountBackground != null) CountBackground.SetActive(good.count > 1);
        if (background[0] != null) background[0].SetActive(good.color == 1);
        if (background[1] != null) background[1].SetActive(good.color == 2);
        if (background[2] != null) background[2].SetActive(good.color == 3);
        if (goldCoin != null) goldCoin.SetActive(!good.real);
    }
}