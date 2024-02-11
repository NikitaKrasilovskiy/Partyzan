using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignFixer : MonoBehaviour
{
    void Start()
    {
        float height = Screen.height;
        float width = Screen.width;
        float k = (height / width);
        float t = (784f / 1200f);

        if (k > t)
        {
            needFix = true;
            rectTransform = GetComponent<RectTransform>();
            scale = new Vector3(rectTransform.localScale.x * t / k, rectTransform.localScale.y * t / k, 1);
            pos = new Vector3(0, 0, 0);
        }
        else
        {
            needFix = true;
            rectTransform = GetComponent<RectTransform>();
            scale = new Vector3(rectTransform.localScale.x * k / t, rectTransform.localScale.y * k / t, 1);
            pos = new Vector3(0, 0, 0);
        }
    }
    RectTransform rectTransform;
    Vector3 scale;
    Vector3 pos;
    bool needFix = false;

    void Update()
    {
        if (needFix)
        {
            rectTransform.localScale = scale;
            rectTransform.localPosition = pos;
        }
    }

    public static float factor()
    {
        float height = Screen.height;
        float width = Screen.width;
        float k = (height / width);
        float t = (500f / 775f);

        if (k > t)
            return t / k;
        else
            return 1;
    }
}