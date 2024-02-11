using CCGKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatEditorScript : MonoBehaviour
{
    public ScrollRect scrollRect;
    public Transform content;

    public InputField messageInput;
    public MessagePreviewScript messagePreview;
    public PreloaderAmination preloader;

    Model model;

    void Start ()
    {
        model = GameManager.Instance.model;
        CancelInvoke("UpdateList");
        InvokeRepeating("UpdateList", 0, 5);
    }

    public void SendMessage()
    { StartCoroutine(SendMessageProcess()); }

    IEnumerator SendMessageProcess()
    {
        preloader.Activate();
        yield return model.sendMessage(0, messageInput.text);
        yield return model.getChat(0);
        ShowChat();
        messageInput.text = "";
        preloader.Deactivate();
    }

    public void UpdateList()
    { StartCoroutine(UpdateListProcess()); }

    IEnumerator UpdateListProcess()
    {
        preloader.Activate();
        yield return model.getChat(0);
        preloader.Deactivate();
        ShowChat();
    }

    List<MessagePreviewScript> messagePreviewList = new List<MessagePreviewScript>();

    public void ShowChat()
    {
        float f = scrollRect.verticalNormalizedPosition;
        foreach (MessagePreviewScript ups in messagePreviewList)
        { Destroy(ups.gameObject); }

        messagePreviewList.Clear();
        for (int i = 0; i < model.chat.messages.Length; i++)
        {
            MessagePreviewScript up = Instantiate(messagePreview);
            up.preloader = preloader;
            up.Show(model.chat.messages[i]);
            up.transform.SetParent(content, false);
            up.transform.localScale = Vector3.one;
            up.transform.position = new Vector3(0, 0.3f - i * 0.6f, 0);
            messagePreviewList.Add(up);
        }
        scrollRect.verticalNormalizedPosition = f;
    }
}