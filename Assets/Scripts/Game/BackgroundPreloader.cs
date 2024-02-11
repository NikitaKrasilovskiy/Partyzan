using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundPreloader : MonoBehaviour
{
    public TextAsset imageTA;
    public Vector2 size;

	void Start ()
    {
        Image Texture = gameObject.GetComponent<Image>();
        Texture2D tex = new Texture2D((int)size.x, (int)size.y);
        tex.LoadImage(imageTA.bytes);

        Sprite mySprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0f, 0f), 100.0f);

        Texture.sprite = mySprite;
    }
}