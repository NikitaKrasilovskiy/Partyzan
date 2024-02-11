using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class DigiFont : MonoBehaviour
{
    public Color color;
    private Color invisible = new Color(0,0,0,0);

    private int value = 0;

    public List<Sprite> digits;
    public Sprite plus;
    public Sprite minus;

    public SpriteRenderer oneDigit;
    public SpriteRenderer twoDigit1;
    public SpriteRenderer twoDigit2;
    public SpriteRenderer threeDigit1;
    public SpriteRenderer threeDigit2;
    public SpriteRenderer threeDigit3;

    public SpriteRenderer background;

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

        if (v < 10)
        {
            oneDigit.sprite = digits[v];
            oneDigit.gameObject.SetActive(true);
        }
        else if (v < 100)
        {
            twoDigit1.sprite = digits[v / 10];
            twoDigit2.sprite = digits[v % 10];
            twoDigit1.gameObject.SetActive(true);
            twoDigit2.gameObject.SetActive(true);
        }
        else if (v < 1000)
        {
            threeDigit1.sprite = digits[v / 100];
            threeDigit2.sprite = digits[((v - v % 10) / 10) % 10];
            threeDigit3.sprite = digits[v % 10];
            threeDigit1.gameObject.SetActive(true);
            threeDigit2.gameObject.SetActive(true);
            threeDigit3.gameObject.SetActive(true);
        }
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

        if (absV < 10)
        {
            twoDigit1.sprite = (v < 0)?minus:plus;
            twoDigit2.sprite = digits[absV];
            twoDigit1.gameObject.SetActive(true);
            twoDigit2.gameObject.SetActive(true);

            if (v < 0)
            { if (background != null) background.gameObject.SetActive(true); }
        }
        else if (absV < 100)
        {
            threeDigit1.sprite = (v < 0) ? minus : plus;
            threeDigit2.sprite = digits[absV / 10];
            threeDigit3.sprite = digits[absV % 10];
            threeDigit1.gameObject.SetActive(true);
            threeDigit2.gameObject.SetActive(true);
            threeDigit3.gameObject.SetActive(true);

            if (v < 0)
            { if (background != null) background.gameObject.SetActive(true); }
        }
        fadeout = true;
        a = 2;
    }

    public int getV()
    { return value; }
}