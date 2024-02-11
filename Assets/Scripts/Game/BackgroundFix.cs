using UnityEngine;
using UnityEngine.UI;

public class BackgroundFix : MonoBehaviour
{
    void Start()
    {
        float height = Screen.height;
        float width = Screen.width;
        float k = (height / width);
        float t = (676.0f / 1200.0f);
        //Debug.Log("Width: " + width.ToString());
        //Debug.Log("Height: " + height.ToString());
        //Debug.Log("K: " + k.ToString());
        //Debug.Log("T: " + t.ToString());

        if (k > t)
        {
            needFix = true;
            rectTransform = GetComponent<RectTransform>();
            scale = new Vector3(k/t* t2, k/t* t2, 1);
            pos = new Vector3(0, 0, 0);
        }
    }

    RectTransform rectTransform;
    Vector3 scale;
    Vector3 pos;
    bool needFix = false;
    public float t2 = 1;

	void Update ()
    {
        if (needFix)
        {
        	rectTransform.localScale = scale;
        	rectTransform.localPosition = pos;
        }
    }
}