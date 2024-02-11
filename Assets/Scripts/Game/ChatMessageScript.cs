using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatMessageScript : MonoBehaviour
{
    public TextMeshProUGUI text;
    public Image background;
    RectTransform rectTransform;
    Vector2 size;
    Vector3 pos;
    int lines;
    float hPlus = 0;

    void Start ()
    { precalc(); }

    void precalc()
    {
        rectTransform = (RectTransform)background.transform;
        text.ForceMeshUpdate();
        hPlus = rectTransform.rect.height - text.textBounds.size.y;
        size = rectTransform.sizeDelta;
        pos = rectTransform.position;
    }

    public void SetText(string s)
    {
        //Debug.Log("SetText: " + s);
        precalc();
        text.text = s;
        text.ForceMeshUpdate();
        //Debug.Log("Height: " + text.textBounds.size.y.ToString());
        lines = Mathf.RoundToInt(text.textBounds.size.y  / 47.47794f);
        size.y = hPlus + text.textBounds.size.y + 2;
        pos.y = rectTransform.position.y - text.textBounds.size.y * 0.47f;
        rectTransform.sizeDelta = size;
        rectTransform.localPosition = new Vector3(0,-21*lines+28,0);
        //Debug.Log(rectTransform.localPosition);
        text.lineSpacing = -14;
    }

    public float Lines()
    { return lines; }
}