using System;
using OpenAi;
using UnityEngine;
using UnityEngine.UI;


public class ChatGptDemo : MonoBehaviour
{
    public Chat chat;
    
    public InputField inputField;
    public DialogBox dialogBox;

    private void Start()
    {
        AutoHideDialog();
    }

    public void SetChatAction()
    {
        if (inputField == null) return;
        if (dialogBox == null) return;
        dialogBox.ShowDialog("酱酱，在思考中。。。。");
        SingleAskStream(inputField.text);
    }

    /// <summary>
    ///  聊天
    /// </summary>
    /// <param name="message"></param>
    private async void Chat(string message)
    {
        chat.messages.Add(new Message
        {
            role = "user",
            content = message
        });
        await ChatGpt.Chat(chat);
        Debug.Log(chat.Now);
    }

    /// <summary>
    /// 单次问答
    /// </summary>
    /// <param name="message"></param>
    private async void SingleAsk(string message)
    {
        var result = await ChatGpt.SingleAsk(message);
        Debug.Log(result);
    }

    /// <summary>
    ///  流式聊天
    /// </summary>
    /// <param name="message"></param>
    private async void ChatStream(string message)
    {
        chat.messages.Add(new Message
        {
            role = "user",
            content = message
        });
        await ChatGpt.ChatStream(chat, Debug.Log);
        Debug.Log($"结束了--------{chat.Now}");
    }

    async void SingleAskStream(string message)
    {
        var result = await ChatGpt.SingleAskStream(message, Debug.Log);
        dialogBox.ShowDialog(result);
        Invoke(nameof(AutoHideDialog),60);
        Debug.Log($"结束了--------{result}");
    }

    void AutoHideDialog()
    {
        if (dialogBox == null) return;
        dialogBox.HideDialog();
    }
}