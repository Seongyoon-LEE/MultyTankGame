using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Chat;
using Photon.Realtime;
using ExitGames.Client.Photon;
using AuthenticationValues = Photon.Chat.AuthenticationValues;
using System;

public class PhotonChat : MonoBehaviourPun, IChatClientListener
{
    private ChatClient chatClient;
    private string userName;
    private string currentChannelName;

    public Text outPutText;
    public InputField inputField;
    void Start()
    {
        Application.runInBackground = true;

        userName = DateTime.Now.ToShortTimeString();

        currentChannelName = "Channel 001";
        chatClient = new ChatClient(this);

        chatClient.UseBackgroundWorkerForSending = true;
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new AuthenticationValues(userName));
        AddLine(string.Format("연결시도", userName));

    }

    public void AddLine(string message)
    {
        outPutText.text += message + "\r\n";
    }

    public void OnApplicationQuit()
    {
        if (chatClient != null)
            chatClient.Disconnect();
    }
    public void DebugReturn(DebugLevel level, string message)
    {
        if (level == DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }
    public void OnConnected()
    {
        AddLine("서버 연결 성공");
        chatClient.Subscribe(new string[] { currentChannelName }, 10);
    }
    public void OnDisconnected()
    {
        AddLine("서버에 연결이 끊어 졌습니다.");
    }
    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange = " + state);
    }
    public void OnSubscribed(string[] channels, bool[] results)
    {
        AddLine(string.Format("채널입장 ({0})", string.Join(",", channels)));
    }

    public void OnUnsubscribed(string[] channels)
    {
        AddLine(string.Format("채널퇴장 ({0})", string.Join(",", channels)));
    }
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        if (channelName.Equals(currentChannelName))
            this.ShowChannel(currentChannelName);
    }

    public void ShowChannel(string channelName)
    {
        if (string.IsNullOrEmpty(channelName)) return;
        ChatChannel channel = null;
        bool found = this.chatClient.TryGetChannel(channelName, out channel);
        if (!found)
        {
            Debug.Log("Channel Failed");
            return;
        }
        this.currentChannelName = channelName;
        this.outPutText.text = channel.ToStringMessages();
        Debug.Log("ShowChannel: " + currentChannelName);
    }
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        Debug.Log("OnPrivateMessage: " + message);
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log("status: " + string.Format("{0} is {1}, Msg:{2} ", user, status, message));
    }

    void Update()
    {
        chatClient.Service();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnEnterSend()
    {
        if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter))
        {
            this.SendChatMessage(this.inputField.text);
            this.inputField.text = "";

        }
    }

    void SendChatMessage(string inputLine)
    {
        if (string.IsNullOrEmpty(inputLine)) return;

        Debug.Log(inputLine);
        this.chatClient.PublishMessage(currentChannelName, inputLine);

    }

}
