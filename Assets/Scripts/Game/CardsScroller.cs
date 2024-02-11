using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardsScroller : MonoBehaviour
{
    public ScrollRect myScrollRect;

    public void ScrollLeft()
    { if (myScrollRect.horizontalNormalizedPosition > 0.6f) myScrollRect.horizontalNormalizedPosition = 0.5f; else myScrollRect.horizontalNormalizedPosition = 0.0f; }

    public void ScrollRight()
    { if (myScrollRect.horizontalNormalizedPosition < 0.4f) myScrollRect.horizontalNormalizedPosition = 0.5f; else myScrollRect.horizontalNormalizedPosition = 1.0f; }
}