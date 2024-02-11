using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class ShopItem : MonoBehaviour
{
    [SerializeField]
    int ID;

    public void Buy(Product p)
    { StartCoroutine(GameManager.Instance.model.buy(ID, "1111","111111111")); }
}