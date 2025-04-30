using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    /// <summary>
    /// 玩家游戏物体
    /// </summary>
    public GameObject playerObj;
    /// <summary>
    /// "叫地主"的按钮
    /// </summary>
    public Button callLandlordButton;
    /// <summary>
    /// "不叫"的按钮
    /// </summary>
    public Button notCallLandlordButton;
    /// <summary>
    /// "抢地主"的按钮
    /// </summary>
    public Button robLandlordButton;
    /// <summary>
    /// "不抢"的按钮
    /// </summary>
    public Button notRobLandlordButton;


    public override void OnInit()
    {
        skinPath = "GamePanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        //寻找组件
        playerObj = skin.transform.Find("Player").gameObject;
        callLandlordButton = skin.transform.Find("CallLandlord_Btn").GetComponent<Button>();
        notCallLandlordButton = skin.transform.Find("NotCallLandlord_Btn").GetComponent<Button>();
        robLandlordButton = skin.transform.Find("RobLandlord_Btn").GetComponent<Button>();
        notRobLandlordButton = skin.transform.Find("NotRobLandlord_Btn").GetComponent<Button>();

        callLandlordButton.gameObject.SetActive(false);
        notCallLandlordButton.gameObject.SetActive(false);
        robLandlordButton.gameObject.SetActive(false);
        notRobLandlordButton.gameObject.SetActive(false);

        //添加消息监听
        NetManager.AddMsgListener("MsgGetCardList", OnMsgGetCardList);
        NetManager.AddMsgListener("MsgGetStartPlayer", OnMsgGetStartPlayer);
        NetManager.AddMsgListener("MsgSwitchTurn", OnMsgSwitchTurn);
        NetManager.AddMsgListener("MsgGetOtherPlayers", OnMsgGetOtherPlayers);

        //添加按钮事件
        callLandlordButton.onClick.AddListener(OnCallLandlordButtonClick);
        notCallLandlordButton.onClick.AddListener(OnNotCallLandlordButtonClick);
        robLandlordButton.onClick.AddListener(OnRobLandlordButtonClick);
        notRobLandlordButton.onClick.AddListener(OnNotRobLandlordButtonClick);

        //发送请求消息，获取同桌的左右玩家的id
        MsgGetOtherPlayers msgGetOtherPlayers = new MsgGetOtherPlayers();
        NetManager.Send(msgGetOtherPlayers);

        //发送请求消息，先获取一副扑克牌
        MsgGetCardList msgGetCardList = new MsgGetCardList();
        NetManager.Send(msgGetCardList);

        //发送请求消息,知道从哪个玩家开始叫地主
        MsgGetStartPlayer msgGetStartPlayer = new MsgGetStartPlayer();
        NetManager.Send(msgGetStartPlayer);
    }

    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgGetCardList", OnMsgGetCardList);
        NetManager.RemoveMsgListener("MsgGetStartPlayer", OnMsgGetStartPlayer);
        NetManager.RemoveMsgListener("MsgSwitchTurn", OnMsgSwitchTurn);
        NetManager.RemoveMsgListener("MsgGetOtherPlayers", OnMsgGetOtherPlayers);
    }

    public void OnMsgGetCardList(MsgBase msgBase)
    {
        MsgGetCardList msgGetCardList = msgBase as MsgGetCardList;

        for (int i = 0; i < CardManager.maxHandSize; i++)
        {
            //根据卡牌信息构造卡牌
            Card card = new Card(msgGetCardList.cardInfos[i].suit, msgGetCardList.cardInfos[i].rank);
            GameManager.cards.Add(card);
        }

        //手牌排序
        Card[] cards = CardManager.CardSort(GameManager.cards.ToArray());

        //实例化生成卡牌
        GenerateCard(cards);
    }

    /// <summary>
    /// 手牌排序
    /// </summary>
    public void CardSort()
    {

    }

    /// <summary>
    /// 实例化生成卡牌
    /// </summary>
    /// <param name="cards"></param>
    public void GenerateCard(Card[] cards)
    {
        for(int i = 0; i < cards.Length; i++)
        {
            string name = CardManager.GetName(cards[i]);
            GameObject cardObj = new GameObject(name);
            Image image = cardObj.AddComponent<Image>();
            Sprite sprite = Resources.Load<Sprite>("Card/" + name);
            image.sprite = sprite;
            image.rectTransform.sizeDelta = new Vector2(80, 100);
            cardObj.transform.SetParent(playerObj.transform.Find("Cards"), false);
            cardObj.layer = LayerMask.NameToLayer("UI");
        }
    }

    /// <summary>
    /// 当前玩家是否需要叫地主
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgGetStartPlayer(MsgBase msgBase)
    {
        MsgGetStartPlayer msgGetStartPlayer = msgBase as MsgGetStartPlayer;

        if(GameManager.playerId == msgGetStartPlayer.playerId)
        {
            callLandlordButton.gameObject.SetActive(true);
            notCallLandlordButton.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 玩家叫/抢完地主后，切换到下一个玩家
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgSwitchTurn(MsgBase msgBase)
    {
        MsgSwitchTurn msgSwitchTurn = msgBase as MsgSwitchTurn;
        switch (GameManager.status)
        {
            case PlayerStatus.Call:
                //下一个叫地主的玩家就是当前客户端的玩家，就显示叫地主的按钮
                if(msgSwitchTurn.nextPlayerId == GameManager.playerId)
                {
                    callLandlordButton.gameObject.SetActive(true);
                    notCallLandlordButton.gameObject.SetActive(true);
                }
                else
                {
                    callLandlordButton.gameObject.SetActive(false);
                    notCallLandlordButton.gameObject.SetActive(false);
                }
                break;
            case PlayerStatus.Rob:
                break;
            case PlayerStatus.Play:
                break;
            default:
                break;
        }

    }

    /// <summary>
    /// 获得同桌的其他玩家
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgGetOtherPlayers(MsgBase msgBase)
    {
        MsgGetOtherPlayers msgGetOtherPlayers = msgBase as MsgGetOtherPlayers;

        GameManager.leftPlayerId = msgGetOtherPlayers.leftPlayerId;
        GameManager.rightPlayerId = msgGetOtherPlayers.rightPlayerId;
    }

    /// <summary>
    /// 玩家点击"叫地主"按钮
    /// </summary>
    public void OnCallLandlordButtonClick()
    {
        MsgSwitchTurn msgSwitchTurn = new MsgSwitchTurn();
        NetManager.Send(msgSwitchTurn);
    }

    /// <summary>
    /// 玩家点击"不叫"按钮
    /// </summary>
    public void OnNotCallLandlordButtonClick()
    {
        MsgSwitchTurn msgSwitchTurn = new MsgSwitchTurn();
        NetManager.Send(msgSwitchTurn);
    }

    /// <summary>
    /// 玩家点击"抢地主"按钮
    /// </summary>
    public void OnRobLandlordButtonClick()
    {

    }

    /// <summary>
    /// 玩家点击"不抢"按钮
    /// </summary>
    public void OnNotRobLandlordButtonClick()
    {

    }
}
