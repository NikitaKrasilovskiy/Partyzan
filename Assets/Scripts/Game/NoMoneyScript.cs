using UnityEngine;

public class NoMoneyScript : MonoBehaviour
{
    public ShopWindowScript shop;
    public GameObject cardsUpgrade;

    public void ToShop()
    {
        shop.SelectType(0);

        cardsUpgrade.SetActive(false);
        gameObject.SetActive(false);
    }
}