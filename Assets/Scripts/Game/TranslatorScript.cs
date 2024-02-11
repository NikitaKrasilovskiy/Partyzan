using CCGKit;
using TMPro;
using UnityEngine;

public class TranslatorScript : MonoBehaviour
{
    private TextMeshProUGUI text;
    private int currentLang = -1;
    private Model model;
    int value = 0;
    int value2 = 0;
    int value3 = 0;

    public bool hilightValue;
    public int hilightFont;
    public Color hilightColor;

    int id = -1;
    string idText = "";

    public void Start()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();

        if (idText == "") idText = text.text;
        { model = GameManager.Instance.model; }

        UpdateText();
    }

    //private void OnEnable()
    //{
    //    text = gameObject.GetComponent<TextMeshProUGUI>();
    //    if (idText == "") idText = text.text;
    //    model = GameManager.Instance.model;
    //    UpdateText();
    //}

    void Update ()
    {
        if (text == null) return;
        if (model.user == null) return;
        if (currentLang == model.user.lang) return;
        UpdateText();
	}

    private TranslatorFonts translatorFonts;

    public void UpdateText()
    {
        if (text == null) return;
        if (model.user == null) return;
        if (currentLang != model.user.lang)
        { UpdateFont(); }

        currentLang = model.user.lang;
        id = model.getTextId(idText);
        //Debug.Log("idText: " + idText);
        //Debug.Log("id: " + id.ToString());

        if (id == -1)
        {
            if (translatorFonts != null)
            { text.font = translatorFonts.FontEU; }

            string v1 = value.ToString();
            string v2 = value2.ToString();
            string v3 = value3.ToString();

            if (hilightValue)
            {
                v1 = "<size=" + hilightFont + "><color=#" + ColorUtility.ToHtmlStringRGB(hilightColor) + ">" + value.ToString() + "</color></size>";
                v2 = "<size=" + hilightFont + "><color=#" + ColorUtility.ToHtmlStringRGB(hilightColor) + ">" + value2.ToString() + "</color></size>";
                v3 = "<size=" + hilightFont + "><color=#" + ColorUtility.ToHtmlStringRGB(hilightColor) + ">" + value3.ToString() + "</color></size>";
            }
            text.text = idText.Replace("%d", v1).Replace("%2d", v2).Replace("%3d", v3).Replace("\\n", "\n");
        }
        else
        {
            if (model.haveText(id))
            {
                //Debug.Log("getText: " + model.getText(id));
                string v1 = value.ToString();
                string v2 = value2.ToString();
                string v3 = value3.ToString();

                if (hilightValue)
                {
                    v1 = "<size=" + hilightFont + "><color=#" + ColorUtility.ToHtmlStringRGB(hilightColor) + ">" + value.ToString() + "</color></size>";
                    v2 = "<size=" + hilightFont + "><color=#" + ColorUtility.ToHtmlStringRGB(hilightColor) + ">" + value2.ToString() + "</color></size>";
                    v3 = "<size=" + hilightFont + "><color=#" + ColorUtility.ToHtmlStringRGB(hilightColor) + ">" + value3.ToString() + "</color></size>";
                }
                text.text = model.getText(id).Replace("%d", v1).Replace("%2d", v2).Replace("%3d", v3).Replace("\\n", "\n");
            }
        }
    }

    public void SetValue(int v)
    {
        value = v;
        UpdateText();
    }

    public void SetValue2(int v)
    {
        value2 = v;
        UpdateText();
    }

    public void SetText(string s)
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
        idText = s;
        model = GameManager.Instance.model;
        id = model.getTextId(idText);
        UpdateFont();
        UpdateText();
    }

    public void UpdateFont()
    {
        Object[] o = FindObjectsOfType(typeof(TranslatorFonts));
        if(model!=null)

        if (o.Length > 0)
        {
                translatorFonts = (TranslatorFonts)o[0];

                switch (model.user.lang)
                {
                    case 7: text.font = translatorFonts.FontCN; break;
                    case 8: text.font = translatorFonts.FontKR; break;
                    case 9: text.font = translatorFonts.FontJP; break;
                    default: text.font = translatorFonts.FontEU; break;
                }
        }
    }
}