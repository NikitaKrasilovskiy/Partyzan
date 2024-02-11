using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CouponEditScript : MonoBehaviour
{
    public Color used;
    public Color unused;
    public Color deleted;

    public string usedText;
    public string unusedText;
    public string deletedText;

    public InputField ID;
    public Dropdown Type;
    public InputField Gold;
    public TextMeshProUGUI Active;

    public GameObject CreateCodeButton;
    public GameObject EditCodeButtons;

    public CouponsEditorScript couponsEditor;

    Coupon coupon;

    public void ShowCoupon(Coupon coupon)
    {
        ID.text = coupon.id;
        Type.value = coupon.type;
        Gold.text = coupon.gold.ToString();

        if (coupon.used) {
            Active.color = used;
            Active.text = usedText;
        } else {
            Active.color = unused;
            Active.text = unusedText;
        }
        this.coupon = coupon;
    }

    Coupon getCoupon()
    {
        coupon.id = ID.text.ToUpper();
        coupon.type = Type.value;
        coupon.gold = int.Parse(Gold.text);

        if (coupon.used)
        {
            Active.color = used;
            Active.text = usedText;
        }
        else
        {
            Active.color = unused;
            Active.text = unusedText;
        }
        return coupon;
    }

    public void setCreator()
    {
        CreateCodeButton.SetActive(true);
        EditCodeButtons.SetActive(false);
        Active.gameObject.SetActive(false);
        coupon = new Coupon();
        ID.readOnly = false;
    }

    public void setEditor()
    {
        CreateCodeButton.SetActive(false);
        EditCodeButtons.SetActive(true);
        Active.gameObject.SetActive(true);
        ID.readOnly = true;
    }

    public void CreateCoupon()
    { couponsEditor.CreateCoupon(getCoupon()); }

    public void UpdateCoupon()
    { couponsEditor.UpdateCoupon(getCoupon()); }

    public void DeleteCoupon()
    { couponsEditor.DeleteCoupon(coupon.id, gameObject); }
}