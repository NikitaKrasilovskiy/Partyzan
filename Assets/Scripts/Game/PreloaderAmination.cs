using TMPro;
using UnityEngine;

public class PreloaderAmination : MonoBehaviour
{
    private TextMeshProUGUI text;

    private float time = 0;

    void Start()
    { text = gameObject.GetComponent<TextMeshProUGUI>(); }

    void Update()
    {
        if (gameObject.activeInHierarchy)
        {
            time += Time.deltaTime;
            int t = (int)Mathf.Floor(time * 3);
            t %= 6;
            t++;
            string s = "";

            for (int i = 0; i < t; i++) s += '.';
            text.text = s;
        }
    }

    public void Activate()
    { gameObject.SetActive(true); }

    public void Deactivate()
    { gameObject.SetActive(false); }
}