using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePanel : BasePanel
{
    public override void OnInit()
    {
        skinPath = "GamePanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
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

        //生成卡牌
    }
}
