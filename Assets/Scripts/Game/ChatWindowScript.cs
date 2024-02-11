using CCGKit;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatWindowScript : MonoBehaviour
{
    public TMP_InputField messageText;
    public ChatMessageScript chatMessage;
    public ScrollRect myScrollRect;
    public ChatMessageScript instance;
    public ChatMessageScript outInstance;

    public PreloaderAmination preloader;

    Model model;

    void Start ()
    { model = GameManager.Instance.model; }

    public void OpenWindow()
    {
        model = GameManager.Instance.model;
        gameObject.SetActive(true);
        messageText.characterLimit = model.rules.commonParams.maxMessageLength;
        InvokeRepeating("updateData", 0, model.rules.commonParams.chatUpdateTime);
        updateData();
    }

    public void CloseWindow()
    {
        CancelInvoke("updateData");
        preloader.Deactivate();
        gameObject.SetActive(false);
    }

    void updateData()
    { StartCoroutine(ShowChatProcess()); }

    public void sendMessage()
    {
        string s = messageText.text;
        if (s == "") return;
        if (s.Length > model.rules.commonParams.maxMessageLength) s.Remove(model.rules.commonParams.maxMessageLength);
        StartCoroutine(SendMessageProcess(s));
    }

    IEnumerator SendMessageProcess(string s)
    {
        preloader.Activate();
        yield return model.sendMessage(0, s);
        yield return ShowChatProcess();
        messageText.text = "";
        preloader.Deactivate();
    }
    

    private IEnumerator ShowChatProcess()
    {
        preloader.Activate();
        yield return model.getChat(0);
        showChat();
        preloader.Deactivate();
    }

    public float K = 0;

    private void showChat()
    {
        float f = myScrollRect.verticalNormalizedPosition;
        Vector2 size = myScrollRect.content.sizeDelta;
        size.y = 300;
        myScrollRect.verticalNormalizedPosition = 0;
        instance.gameObject.SetActive(true);
        outInstance.gameObject.SetActive(true);

        foreach (Transform child in myScrollRect.content)
        {
            if (child.gameObject == instance.gameObject) continue;
            if (child.gameObject == outInstance.gameObject) continue;
            Destroy(child.gameObject);
        }

        //Debug.Log(model.chat);
        float y = -4.5f;
        for (int i= model.chat.messages.Length-1; i>=0;i--) {
            ChatMessage msg = model.chat.messages[i];
            bool outCome = msg.author == model.user.name;
            if (outCome) {
                ChatMessageScript cm = Instantiate(outInstance);
                cm.transform.SetParent(myScrollRect.content,false);
                cm.transform.position = new Vector3(0.53f, y, 0);
                cm.transform.localScale = Vector3.one * 0.947f;
                cm.SetText(msg.getText());
                y += 0.85f * cm.Lines();
                cm.transform.position = new Vector3(0.53f, y +( 0.85f * cm.Lines()-1) * K, 0);
                cm.gameObject.SetActive(true);
            }
            else
            {
                ChatMessageScript cm = Instantiate(instance);
                cm.transform.SetParent(myScrollRect.content, false);
                cm.transform.position = new Vector3(-0.53f, y, 0);
                cm.transform.localScale = Vector3.one * 0.947f;
                cm.SetText(msg.getText());
                y += 0.85f * cm.Lines();
                cm.transform.position = new Vector3(-0.53f, y +(0.85f * cm.Lines()-1) * K, 0);
                cm.gameObject.SetActive(true);
            }
        }
        size = myScrollRect.content.sizeDelta;
        size.y = (6.5f+y) * 70;
        myScrollRect.content.sizeDelta = size;
        instance.gameObject.SetActive(false);
        outInstance.gameObject.SetActive(false);
        myScrollRect.verticalNormalizedPosition = f;
    }
}