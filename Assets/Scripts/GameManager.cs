using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

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
    /// �Ϸ������������ŵ���
    /// </summary>
    public static List<Card> threeCards = new List<Card>();
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
    /// <summary>
    /// �Լ�������ɵ���Ϸ���壬����е�������������״̬��Ϣ��
    /// </summary>
    public static GameObject playerObj;
    /// <summary>
    /// �Ϸ��ĵ���
    /// </summary>
    public static GameObject threeCardsObj;
    /// <summary>
    /// ����Ƿ�ס������ѡ��
    /// </summary>
    public static bool isPressing;
    /// <summary>
    /// ѡ�������
    /// </summary>
    public static List<Card> selectCards = new List<Card>();
    /// <summary>
    /// �Ƿ��������ơ�true��ʾ���Բ�������ʾ�����İ�ť��
    /// </summary>
    public static bool canNotPlay;

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

        //��ǰ��Ҫ�����Լ���ҵ���Ϣ
        if (playerId == id)
        {
            GameObject go = Instantiate(resource, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(playerObj.transform, false);
        }
    }

    /// <summary>
    /// ���������Ѿ����ɵ������Ϣ��������Ϣ�ص���
    /// </summary>
    /// <param name="id"></param>
    /// <param name="name"></param>
    public static void SyncDestroy(string id)
    {
        if(id == leftPlayerId)
        {
            for(int i = leftPlayerInfoObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(leftPlayerInfoObj.transform.GetChild(i).gameObject);
            }
        }

        if (id == rightPlayerId)
        {
            for (int i = rightPlayerInfoObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(rightPlayerInfoObj.transform.GetChild(i).gameObject);
            }
        }

        if(id == playerId)
        {
            for (int i = playerObj.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(rightPlayerInfoObj.transform.GetChild(i).gameObject);
            }
        }
    }

    public static void SyncGenerateCard(string id, string name)
    {
        //��ȡCard�ļ����µ�ͼƬ
        name = "Card/" + name;
        Sprite sprite = Resources.Load<Sprite>(name);
        
        if(id == leftPlayerId)
        {
            GameObject go = new GameObject(name);
            Image image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.SetNativeSize();
            go.transform.localScale = new Vector3(0.8f, 0.8f);
            go.transform.SetParent(leftPlayerInfoObj.transform, false);
        }

        if(id == rightPlayerId)
        {
            GameObject go = new GameObject(name);
            Image image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.SetNativeSize();
            go.transform.localScale = new Vector3(0.8f, 0.8f);
            go.transform.SetParent(rightPlayerInfoObj.transform, false);
        }

        if (id == playerId)
        {
            GameObject go = new GameObject(name);
            Image image = go.AddComponent<Image>();
            image.sprite = sprite;
            image.SetNativeSize();
            go.transform.localScale = new Vector3(0.8f, 0.8f);
            go.transform.SetParent(playerObj.transform, false);
        }
    }

    /// <summary>
    /// ͬ������������ҵĿ�������
    /// </summary>
    /// <param name="id">���id</param>
    /// <param name="count">���Ƶ�����</param>
    public static void SyncCardNumber(string id, int count)
    {
        if (id == leftPlayerId)
        {
            Text text = leftPlayerInfoObj.transform.parent.Find("Card_Img/CardNumber_Text").GetComponent<Text>();
            text.text = (int.Parse(text.text) - count).ToString();
        }

        if (id == rightPlayerId)
        {
            Text text = rightPlayerInfoObj.transform.parent.Find("Card_Img/CardNumber_Text").GetComponent<Text>();
            text.text = (int.Parse(text.text) - count).ToString();
        }
    }
}
