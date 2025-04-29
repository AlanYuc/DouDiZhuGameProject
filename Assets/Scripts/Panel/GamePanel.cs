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

    public override void OnInit()
    {
        skinPath = "GamePanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        //寻找组件
        playerObj = skin.transform.Find("Player").gameObject;

        //添加消息监听
        NetManager.AddMsgListener("MsgGetCardList", OnMsgGetCardList);

        //发送请求消息，先获取一副扑克牌
        MsgGetCardList msgGetCardList = new MsgGetCardList();
        NetManager.Send(msgGetCardList);
    }

    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgGetCardList", OnMsgGetCardList);
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
}
