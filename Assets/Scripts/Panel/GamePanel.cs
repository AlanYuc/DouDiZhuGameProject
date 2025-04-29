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
        playerObj               = skin.transform.Find("Player").gameObject;
        callLandlordButton      = skin.transform.Find("CallLandlord_Btn").GetComponent<Button>();
        notCallLandlordButton   = skin.transform.Find("NotCallLandlord_Btn").GetComponent<Button>();
        robLandlordButton       = skin.transform.Find("RobLandlord_Btn").GetComponent<Button>();
        notRobLandlordButton    = skin.transform.Find("NotRobLandlord_Btn").GetComponent<Button>();

        callLandlordButton.gameObject.SetActive(false);
        notCallLandlordButton.gameObject.SetActive(false);
        robLandlordButton.gameObject.SetActive(false);
        notRobLandlordButton.gameObject.SetActive(false);

        //添加消息监听
        NetManager.AddMsgListener("MsgGetCardList", OnMsgGetCardList);
        NetManager.AddMsgListener("MsgGetStartPlayer", OnMsgGetStartPlayer);

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
    }

    public void OnMsgGetCardList(MsgBase msgBase)
    {
        MsgGetCardList msgGetCardList = msgBase as MsgGetCardList;

        for(int i = 0; i < CardManager.maxHandSize; i++)
        {
            //根据卡牌信息构造卡牌
            Card card = new Card(msgGetCardList.cardInfos[i].suit, msgGetCardList.cardInfos[i].rank);
            GameManager.cards.Add(card);
        }

        //实例化生成卡牌
        GenerateCard(GameManager.cards.ToArray());
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
}
