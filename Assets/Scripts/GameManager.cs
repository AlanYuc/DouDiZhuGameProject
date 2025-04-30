using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��ҵ�ǰ״̬������
/// </summary>
public enum PlayerStatus
{
    /// <summary>
    /// �е���
    /// </summary>
    Call,
    /// <summary>
    /// ������
    /// </summary>
    Rob,
    /// <summary>
    /// ����
    /// </summary>
    Play,
}

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
    /// <summary>
    /// ��ҵ�ǰ��״̬��Ĭ�ϸ�Ϊ�е���״̬
    /// </summary>
    public static PlayerStatus status = PlayerStatus.Call;

    void Start()
    {
        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
        NetManager.AddMsgListener("MsgKick", OnMsgKick);

        PanelManager.Init();
        PanelManager.Open<LoginPanel>();

        root = GameObject.Find("Root").GetComponent<Transform>();

        CardManager.Init();
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
