using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

/// <summary>
/// 玩家当前状态的种类
/// </summary>
public enum PlayerStatus
{
    /// <summary>
    /// 叫地主
    /// </summary>
    Call,
    /// <summary>
    /// 抢地主
    /// </summary>
    Rob,
    /// <summary>
    /// 出牌
    /// </summary>
    Play,
}

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 玩家id
    /// </summary>
    public static string playerId = "";
    /// <summary>
    /// 坐在当前玩家左侧的玩家id
    /// </summary>
    public static string leftPlayerId = "";
    /// <summary>
    /// 坐在当前玩家右侧的玩家id
    /// </summary>
    public static string rightPlayerId = "";
    
    /// <summary>
    /// 当前玩家是否为房主
    /// </summary>
    public static bool isHost;
    /// <summary>
    /// 当前玩家是不是地主
    /// </summary>
    public static bool isLandLord = false;
    /// <summary>
    /// 通过Root来找当前场景激活的Panel
    /// </summary>
    private Transform root;
    /// <summary>
    /// 玩家手牌
    /// </summary>
    public static List<Card> cards = new List<Card>();
    /// <summary>
    /// 上方给地主的三张底牌
    /// </summary>
    public static List<Card> threeCards = new List<Card>();
    /// <summary>
    /// 玩家当前的状态，默认给为叫地主状态
    /// </summary>
    public static PlayerStatus status = PlayerStatus.Call;
    /// <summary>
    /// 坐在左边的玩家生成的游戏物体，比如叫地主、抢地主的状态信息等
    /// </summary>
    public static GameObject leftPlayerInfoObj;
    /// <summary>
    /// 坐在右边的玩家生成的游戏物体，比如叫地主、抢地主的状态信息等
    /// </summary>
    public static GameObject rightPlayerInfoObj;
    /// <summary>
    /// 自己玩家生成的游戏物体，比如叫地主、抢地主的状态信息等
    /// </summary>
    public static GameObject playerObj;
    /// <summary>
    /// 上方的底牌
    /// </summary>
    public static GameObject threeCardsObj;
    /// <summary>
    /// 鼠标是否按住，进行选牌
    /// </summary>
    public static bool isPressing;
    /// <summary>
    /// 选择的手牌
    /// </summary>
    public static List<Card> selectCards = new List<Card>();
    /// <summary>
    /// 是否允许不出牌。true表示可以不出，显示不出的按钮。
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

    /// <summary>
    /// 同步生成其他玩家的相关信息的游戏物体，比如谁正在叫地主，谁正在抢地主等
    /// </summary>
    /// <param name="id">需要加载游戏物体的id</param>
    /// <param name="name">需要加载游戏物体的名字（包含路径）</param>
    public static void SyncGenerate(string id, string name)
    {
        GameObject resource = Resources.Load<GameObject>(name);

        //当前需要生成左侧玩家的信息
        if(leftPlayerId == id)
        {
            GameObject go = Instantiate(resource, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(leftPlayerInfoObj.transform, false);
        }

        //当前需要生成右侧玩家的信息
        if (rightPlayerId == id)
        {
            GameObject go = Instantiate(resource, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(rightPlayerInfoObj.transform, false);
        }

        //当前需要生成自己玩家的信息
        if (playerId == id)
        {
            GameObject go = Instantiate(resource, Vector3.zero, Quaternion.identity);
            go.transform.SetParent(playerObj.transform, false);
        }
    }

    /// <summary>
    /// 用于清理已经生成的相关信息，避免信息重叠。
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
        //读取Card文件夹下的图片
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
    /// 同步更新左右玩家的卡牌数量
    /// </summary>
    /// <param name="id">玩家id</param>
    /// <param name="count">出牌的数量</param>
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
