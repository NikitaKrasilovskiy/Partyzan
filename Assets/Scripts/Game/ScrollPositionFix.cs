using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScrollPositionFix : MonoBehaviour
{
    public ScrollRect myScrollRect;

    int totalGoods = 0;

    void Start()
    {
        //ScrollView = gameObject.GetComponent<ScrollView>();
    }

    public void FixPosition(Vector2 newPosition)
    {
        Debug.Log(newPosition);

        if (newPosition.x > 0.5f)
        { myScrollRect.horizontalNormalizedPosition = 1f; }
        else
        { myScrollRect.horizontalNormalizedPosition = 0f; }
    }
}