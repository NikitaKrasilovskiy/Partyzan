using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LoadingLine : MonoBehaviour
{
    Image image;

    float fill;

    [SerializeField]
    Slider slider;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.fillAmount = 0;
    }

    public void SetValue(float f)
    { fill = f+0.1f; }

    public bool IsLineEnd()
    { return image.fillAmount > 0.999f; }

    private void Update()
    {
        float f = Mathf.Lerp(image.fillAmount, fill, Time.deltaTime);

        image.fillAmount = f;
        slider.value = f-0.015f;
    }
}