using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 玩家id
    /// </summary>
    public static string playerId = "";
    /// <summary>
    /// 当前玩家是否为房主
    /// </summary>
    public static bool isHost;
    /// <summary>
    /// 通过Root来找当前场景激活的Panel
    /// </summary>
    private Transform root;
    /// <summary>
    /// 玩家手牌
    /// </summary>
    public static List<Card> cards = new List<Card>();

    void Start()
    {
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgKick", OnMsgKick);

        PanelManager.Init();
        PanelManager.Open<LoginPanel>();

        root = GameObject.Find("Root").GetComponent<Transform>();
    }

    private void Update()
    {
        NetManager.Update();
    }

    public void OnMsgKick(MsgBase msgBase)
    {
        PanelManager.Open<TipPanel>("被踢下线");

        //被踢下线后，先关闭当前面板，通过Root来找
        root.GetComponent<BasePanel>().Close();

        //然后需要返回登录界面
        PanelManager.Open<LoginPanel>();
        
    }

    public void OnConnectClose(string err)
    {
        PanelManager.Open<TipPanel>("断开连接");
    }
}
