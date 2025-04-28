using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// ���id
    /// </summary>
    public static string playerId = "";
    /// <summary>
    /// ��ǰ����Ƿ�Ϊ����
    /// </summary>
    public static bool isHost;
    /// <summary>
    /// ͨ��Root���ҵ�ǰ���������Panel
    /// </summary>
    private Transform root;
    /// <summary>
    /// �������
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
        PanelManager.Open<TipPanel>("��������");

        //�������ߺ��ȹرյ�ǰ��壬ͨ��Root����
        root.GetComponent<BasePanel>().Close();

        //Ȼ����Ҫ���ص�¼����
        PanelManager.Open<LoginPanel>();
        
    }

    public void OnConnectClose(string err)
    {
        PanelManager.Open<TipPanel>("�Ͽ�����");
    }
}
