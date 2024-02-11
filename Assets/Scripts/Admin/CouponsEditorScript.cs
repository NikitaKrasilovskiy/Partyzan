using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CouponsEditorScript : MonoBehaviour
{
    public GameObject panel;
    public CouponEditScript couponCreator;
    public PreloaderAmination preloader;

    void Start ()
    { StartCoroutine(ShowCoupons()); }

    float y = 0;

    public void addLine(Coupon k)
    {
        GameObject go = Instantiate(couponCreator.gameObject);
        CouponEditScript edt = go.GetComponent<CouponEditScript>();
        edt.setEditor();
        edt.ShowCoupon(k);
        Vector3 v = new Vector3( 400, -40 - y, 0);
        go.transform.SetParent(panel.transform,false);
        go.transform.localScale = Vector3.one;
        go.transform.localPosition = v;
        y += 60;
    }

    IEnumerator ShowCoupons()
    {
        couponCreator.setCreator();
        Model model = GameManager.Instance.model;
        yield return model.showCoupons();
        y = 0;
        foreach (Coupon k in model.coupons.Values)
        { addLine(k); }
    }

    public void CreateCoupon(Coupon coupon)
    { StartCoroutine(CreateCouponProcess(coupon)); }

    IEnumerator CreateCouponProcess(Coupon coupon)
    {
        preloader.Activate();
        Model model = GameManager.Instance.model;
        yield return model.createCoupon(coupon);
        if (model.couponEditorResult.result == "OK")
        { addLine(model.coupons[coupon.id]); }
        preloader.Deactivate();
    }

    public void UpdateCoupon(Coupon coupon)
    { StartCoroutine(UpdateCouponProcess(coupon)); }

    IEnumerator UpdateCouponProcess(Coupon coupon)
    {
        preloader.Activate();
        Model model = GameManager.Instance.model;
        yield return model.updateCoupon(coupon);
        if (model.couponEditorResult.result == "OK")
        { }

        preloader.Deactivate();
    }

    public void DeleteCoupon(string id, GameObject go)
    { StartCoroutine(DeleteCouponProcess(id,go)); }

    IEnumerator DeleteCouponProcess(string id, GameObject go)
    {
        preloader.Activate();
        Model model = GameManager.Instance.model;
        yield return model.removeCoupon(id);
        if (model.couponEditorResult.result == "OK")
        { Destroy(go); }

        preloader.Deactivate();
    }
}