using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PromoWindowScript : MonoBehaviour
{
    private Model model;
    public TMP_InputField code;
    public TextMeshProUGUI gold;

    public GameObject UsedMessage;
    public GameObject NoCouponMessage;
    public GameObject OverloadMessage;

    public TranslatorScript OverloadText;

    public OpenBackpackWindow openBackpackWindow;

	void Start ()
    { model = GameManager.Instance.model; }

    public void OpenWindow()
    { gameObject.SetActive(true); }

    public void ActivateCode()
    { StartCoroutine(ActivateCodeProcess()); }

    IEnumerator ActivateCodeProcess()
    {
        yield return model.useCoupon(code.text.ToUpper());
        gold.text = model.user.gold.ToString();

        switch (model.couponResult.result)
        {
            case "USED":
                UsedMessage.SetActive(true);
                break;

            case "OK":
                openBackpackWindow.showCouponClosed(model.couponResult);
                break;

            case "NO_COUPON":
                NoCouponMessage.SetActive(true);
                break;

            case "OVERLOAD":
                OverloadMessage.SetActive(true);
                OverloadText.SetValue(model.rules.commonParams.maxCouponsPerDay);
                break;
        }
    }
}