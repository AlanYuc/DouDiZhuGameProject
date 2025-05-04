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
    /// ���ڵ�ǰ����������id
    /// </summary>
    public static string leftPlayerId = "";
    /// <summary>
    /// ���ڵ�ǰ����Ҳ�����id
    /// </summary>
    public static string rightPlayerId = "";
    
    /// <summary>
    /// ��ǰ����Ƿ�Ϊ����
    /// </summary>
    public static bool isHost;
    /// <summary>
    /// ��ǰ����ǲ��ǵ���
    /// </summary>
    public static bool isLandLord = false;
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
    /// <summary>
    /// ������ߵ�������ɵ���Ϸ���壬����е�������������״̬��Ϣ��
    /// </summary>
    public static GameObject leftPlayerInfoObj;
    /// <summary>
    /// �����ұߵ�������ɵ���Ϸ���壬����е�������������״̬��Ϣ��
    /// </summary>
    public static GameObject rightPlayerInfoObj;

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

    /// <summary>
    /// ͬ������������ҵ������Ϣ����Ϸ���壬����˭���ڽе�����˭������������
    /// </summary>
    /// <param name="id">��Ҫ������Ϸ�����id</param>
    /// <param name="name">��Ҫ������Ϸ��������֣�����·����</param>
    public static void SyncGenerate(string id, string name)
    {
        GameObject resource = Resources.Load<GameObject>(name);

        //��ǰ��Ҫ���������ҵ���Ϣ
        if(leftPlayerId == id)
        {
            GameObject go = Instantiate(resource, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(leftPlayerInfoObj.transform, false);
        }

        //��ǰ��Ҫ�����Ҳ���ҵ���Ϣ
        if (rightPlayerId == id)
        {
            GameObject go = Instantiate(resource, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(rightPlayerInfoObj.transform, false);
        }
    }
}
