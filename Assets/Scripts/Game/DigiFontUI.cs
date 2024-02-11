using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class DigiFontUI : MonoBehaviour
{
    public Color color;
    private Color invisible = new Color(0,0,0,0);

    private int value = 0;

    public List<Sprite> digits;
    public Sprite plus;
    public Sprite minus;

    public Image oneDigit;
    public Image twoDigit1;
    public Image twoDigit2;
    public Image threeDigit1;
    public Image threeDigit2;
    public Image threeDigit3;
    public Image background;

    public TextMeshProUGUI text;

    [NonSerialized]
    private Color currentColor;

    [NonSerialized]
    private Color backGroundColor;

    void Start()
    {
        Assert.IsNotNull(digits);
        Assert.AreEqual(digits.Count, 10);
        Assert.IsNotNull(oneDigit);
        Assert.IsNotNull(twoDigit1);
        Assert.IsNotNull(twoDigit2);
        Assert.IsNotNull(threeDigit1);
        Assert.IsNotNull(threeDigit2);
        Assert.IsNotNull(threeDigit3);
        currentColor = color;
        updateColor();
        oneDigit.gameObject.SetActive(false);
    }

    private void updateColor()
    {
        oneDigit.color = currentColor;
        twoDigit1.color = currentColor;
        twoDigit2.color = currentColor;
        threeDigit1.color = currentColor;
        threeDigit2.color = currentColor;
        threeDigit3.color = currentColor;
        if (background != null)
        {
            if (value >= 0)
                background.color = invisible;
            else
                background.color = currentColor;

            background.color = backGroundColor;
        }
        text.color = currentColor;
    }

    private bool fadeout = false;
    private float a = 0;

    public void hideFide()
    {
        a = 0;
        fadeout = false;
        currentColor = new Color(color.r, color.g, color.b, a);
        backGroundColor = new Color(1, 1, 1, a);
        updateColor();
    }

    void Update()
    {
        if (fadeout)
        {
            a -= Time.deltaTime*2;
            if (a <= 0)
            {
                a = 0;
                fadeout = false;
            }
            currentColor = new Color(color.r, color.g, color.b, a);
            backGroundColor = new Color(1, 1, 1, a);
            updateColor();
        }
    }

    public void set(int v)
    {
        if (value == v) return;
        value = v;
        oneDigit.gameObject.SetActive(false);
        twoDigit1.gameObject.SetActive(false);
        twoDigit2.gameObject.SetActive(false);
        threeDigit1.gameObject.SetActive(false);
        threeDigit2.gameObject.SetActive(false);
        threeDigit3.gameObject.SetActive(false);
        if (v < 0) v = 0;
        text.text = v.ToString();
    }

    public void delta(int v)
    {
        oneDigit.gameObject.SetActive(false);
        twoDigit1.gameObject.SetActive(false);
        twoDigit2.gameObject.SetActive(false);
        threeDigit1.gameObject.SetActive(false);
        threeDigit2.gameObject.SetActive(false);
        threeDigit3.gameObject.SetActive(false);
        if (background!=null) background.gameObject.SetActive(false);
        int absV = Mathf.Abs(v);
        if (absV == 0) return;
        fadeout = true;
        text.text = ((v>0)?"+":"")+v.ToString();
        a = 2;
    }

    public int getV()
    { return value; }
}