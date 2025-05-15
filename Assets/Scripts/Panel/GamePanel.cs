using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    /// <summary>
    /// "出牌"按钮
    /// </summary>
    public Button playCardButton;
    /// <summary>
    /// "不出"按钮
    /// </summary>
    public Button notPlayCardButton;

    public override void OnInit()
    {
        skinPath = "GamePanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        //寻找组件
        playerObj                       = skin.transform.Find("Player").gameObject;
        callLandlordButton              = skin.transform.Find("CallLandlord_Btn").GetComponent<Button>();
        notCallLandlordButton           = skin.transform.Find("NotCallLandlord_Btn").GetComponent<Button>();
        robLandlordButton               = skin.transform.Find("RobLandlord_Btn").GetComponent<Button>();
        notRobLandlordButton            = skin.transform.Find("NotRobLandlord_Btn").GetComponent<Button>();
        playCardButton                  = skin.transform.Find("PlayCard_Btn").GetComponent<Button>();
        notPlayCardButton               = skin.transform.Find("NotPlayCard_Btn").GetComponent<Button>();

        //初始化组件
        GameManager.leftPlayerInfoObj   = skin.transform.Find("Player_Left/InfoObj").gameObject;
        GameManager.rightPlayerInfoObj  = skin.transform.Find("Player_Right/InfoObj").gameObject;
        GameManager.threeCardsObj       = skin.transform.Find("ThreeCards").gameObject;

        callLandlordButton.gameObject.SetActive(false);
        notCallLandlordButton.gameObject.SetActive(false);
        robLandlordButton.gameObject.SetActive(false);
        notRobLandlordButton.gameObject.SetActive(false);
        playCardButton.gameObject.SetActive(false);
        notPlayCardButton.gameObject.SetActive(false);

        //添加消息监听
        NetManager.AddMsgListener("MsgGetCardList", OnMsgGetCardList);
        NetManager.AddMsgListener("MsgGetStartPlayer", OnMsgGetStartPlayer);
        NetManager.AddMsgListener("MsgSwitchTurn", OnMsgSwitchTurn);
        NetManager.AddMsgListener("MsgGetOtherPlayers", OnMsgGetOtherPlayers);
        NetManager.AddMsgListener("MsgCallLandlord", OnMsgCallLandlord);
        NetManager.AddMsgListener("MsgReStart", OnMsgReStart);
        NetManager.AddMsgListener("MsgStartRobLandlord", OnMsgStartRobLandlord);
        NetManager.AddMsgListener("MsgRobLandlord", OnMsgRobLandlord);
        NetManager.AddMsgListener("MsgPlayCards", OnMsgPlayCards);

        //添加按钮事件
        callLandlordButton.onClick.AddListener(OnCallLandlordButtonClick);
        notCallLandlordButton.onClick.AddListener(OnNotCallLandlordButtonClick);
        robLandlordButton.onClick.AddListener(OnRobLandlordButtonClick);
        notRobLandlordButton.onClick.AddListener(OnNotRobLandlordButtonClick);
        playCardButton.onClick.AddListener(OnPlayCardButtonClick);
        notPlayCardButton.onClick.AddListener(OnNotPlayCardButtonClick);

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
        NetManager.RemoveMsgListener("MsgCallLandlord", OnMsgCallLandlord);
        NetManager.RemoveMsgListener("MsgReStart", OnMsgReStart);
        NetManager.RemoveMsgListener("MsgStartRobLandlord", OnMsgStartRobLandlord);
        NetManager.RemoveMsgListener("MsgRobLandlord", OnMsgRobLandlord);
        NetManager.RemoveMsgListener("MsgPlayCards", OnMsgPlayCards);
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

        //获取三张底牌
        for(int i = 0; i < 3; i++)
        {
            Card card = new Card(msgGetCardList.threeCards[i].suit, msgGetCardList.threeCards[i].rank);
            GameManager.threeCards.Add(card);
        }
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
        //每次生成前先清空，避免重复
        Transform cardsTrans = playerObj.transform.Find("Cards");
        for(int i = cardsTrans.childCount - 1; i >= 0; i--)
        {
            Destroy(cardsTrans.GetChild(i).gameObject);
        }

        for (int i = 0; i < cards.Length; i++)
        {
            //根据牌的名字生成牌的对象，并获取修改图片资源
            string name = CardManager.GetName(cards[i]);
            GameObject cardObj = new GameObject(name);
            Image image = cardObj.AddComponent<Image>();
            Sprite sprite = Resources.Load<Sprite>("Card/" + name);
            image.sprite = sprite;
            image.rectTransform.sizeDelta = new Vector2(80, 100);
            cardObj.transform.SetParent(cardsTrans, false);
            cardObj.layer = LayerMask.NameToLayer("UI");

            //把脚本挂上去
            cardObj.AddComponent<CardUI>();
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
                //下一个抢地主的玩家就是当前客户端的玩家，就显示抢地主的按钮
                if(msgSwitchTurn.nextPlayerId == GameManager.playerId)
                {
                    robLandlordButton.gameObject.SetActive(true);
                    notRobLandlordButton.gameObject.SetActive(true);

                    //抢地主阶段，叫地主的按钮隐藏
                    callLandlordButton.gameObject.SetActive(false);
                    notCallLandlordButton.gameObject.SetActive(false);
                }
                else
                {
                    robLandlordButton.gameObject.SetActive(false);
                    notRobLandlordButton.gameObject.SetActive(false);
                    callLandlordButton.gameObject.SetActive(false);
                    notCallLandlordButton.gameObject.SetActive(false);
                }
                    break;
            case PlayerStatus.Play:
                //先把叫地主抢地主的按钮全部隐藏
                robLandlordButton.gameObject.SetActive(false);
                notRobLandlordButton.gameObject.SetActive(false);
                callLandlordButton.gameObject.SetActive(false);
                notCallLandlordButton.gameObject.SetActive(false);

                if (msgSwitchTurn.nextPlayerId == GameManager.playerId)
                {
                    playCardButton.gameObject.SetActive(true);
                    notPlayCardButton.gameObject.SetActive(true);

                    if (GameManager.canNotPlay)//当前状态下允许 不出牌
                    {
                        //颜色正常
                        notPlayCardButton.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        //可以交互
                        notPlayCardButton.enabled = true;
                    }
                    else//当前状态下不允许 不出牌
                    {
                        //颜色变暗
                        notPlayCardButton.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.6f);
                        //不可以交互
                        notPlayCardButton.enabled = false;
                    }
                }
                else
                {
                    playCardButton.gameObject.SetActive(false);
                    notPlayCardButton.gameObject.SetActive(false);
                }
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
    /// 处理叫地主的逻辑
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgCallLandlord(MsgBase msgBase)
    {
        MsgCallLandlord msgCallLandlord = msgBase as MsgCallLandlord;
        MsgSwitchTurn msgSwitchTurn = new MsgSwitchTurn();

        //左右两边的玩家根据是否叫地主显示相关信息
        if (msgCallLandlord.isCall)
        {
            GameManager.SyncDestroy(msgCallLandlord.id);
            GameManager.SyncGenerate(msgCallLandlord.id, "Word/CallLandlord");
        }
        else
        {
            GameManager.SyncDestroy(msgCallLandlord.id);
            GameManager.SyncGenerate(msgCallLandlord.id, "Word/NotCallLandlord");
        }

        //有其他玩家成为地主，更改图片,并显示三张底牌
        if(msgCallLandlord.result == 3)
        {
            //更改地主图片
            SyncLandLord(msgCallLandlord.id);
            //揭示上方三张底牌
            RevealThreeCards(GameManager.threeCards.ToArray());
            //切换玩家状态为出牌阶段
            GameManager.status = PlayerStatus.Play;
        }

        //不是当前玩家，后续自身的逻辑不需要执行
        if(msgCallLandlord.id != GameManager.playerId)
        {
            return;
        }

        //开始处理自身的逻辑
        switch (msgCallLandlord.result)
        {
            case 0:
                break;
            case 1://抢地主
                MsgStartRobLandlord msgStartRobLandlord = new MsgStartRobLandlord();
                NetManager.Send(msgStartRobLandlord);
                break;
            case 2://重新洗牌
                MsgReStart msgReStart = new MsgReStart();
                NetManager.Send(msgReStart);
                break;
            case 3://自己是地主
                TurnLandLord();
                //自己叫地主成功的回合不需要切换回合
                msgSwitchTurn.round = 0;
                break;
            default:
                break;
        }

        //发送消息切换回合
        NetManager.Send(msgSwitchTurn);
    }

    /// <summary>
    /// 都不叫地主后，重新开始并洗牌
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgReStart(MsgBase msgBase)
    {
        MsgReStart msgReStart = msgBase as MsgReStart;

        //先把原先Cards对象下面的所有卡牌销毁
        Transform cardTrans = playerObj.transform.Find("Cards");
        for (int i = cardTrans.childCount - 1; i >= 0; i--) 
        {
            Destroy(cardTrans.GetChild(i).gameObject);
        }
        //把玩家数据中的卡牌清空
        GameManager.cards.Clear();
        GameManager.threeCards.Clear();

        //然后重新获取一次卡牌，卡牌在MsgReStart的消息发送给服务端后，服务端已完成重新洗牌
        MsgGetCardList msgGetCardList = new MsgGetCardList();
        NetManager.Send(msgGetCardList);
    }

    /// <summary>
    /// 开始抢地主，切换相关的状态
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgStartRobLandlord(MsgBase msgBase)
    {
        MsgStartRobLandlord msgStartRobLandlord = msgBase as MsgStartRobLandlord;

        //更改当前阶段为抢地主阶段
        GameManager.status = PlayerStatus.Rob;
    }

    /// <summary>
    /// 处理出牌的消息
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgPlayCards(MsgBase msgBase)
    {
        MsgPlayCards msgPlayCards = msgBase as MsgPlayCards;

        //测试出牌类型的判断
        Debug.Log("GamePanel.OnMsgPlayCards. 出牌的类型为：" + (CardManager.CardType)msgPlayCards.cardType);

        //更新 该玩家是否可以 不出牌 的逻辑
        //该消息会发送给所有玩家，
        //虽然不允许不出牌时所有玩家的GameManager.canNotPlay都会修改，但只有正在操作的玩家会有需要该值，
        //下次出牌等操作后，该值会被重新赋值
        GameManager.canNotPlay = msgPlayCards.canNotPlay;

        //处理左右玩家的出牌逻辑，出牌就显示具体的牌，不出就显示不出
        if (msgPlayCards.result)
        {
            if (msgPlayCards.isPlay)
            {
                Card[] cards = CardManager.GetCards(msgPlayCards.cardInfos);
                Array.Sort(cards, (Card card1, Card card2) => (int)card1.rank - (int)card2.rank);

                //先清空需要显示的区域
                GameManager.SyncDestroy(msgPlayCards.id);

                //生成同步的卡牌
                for(int i = 0; i < cards.Length; i++)
                {
                    GameManager.SyncGenerateCard(msgPlayCards.id, CardManager.GetName(cards[i]));
                }
            }
            else
            {
                GameManager.SyncDestroy(msgPlayCards.id);
                GameManager.SyncGenerate(msgPlayCards.id, "Word/NotPlayCard");
            }
        }

        //后续处理当前客户端玩家的出牌逻辑
        if(msgPlayCards.id != GameManager.playerId)
        {
            return;
        }

        //返回结果为true，说明操作成功
        if (msgPlayCards.result)
        {
            //第一种，出牌成功
            if (msgPlayCards.isPlay)
            {
                Card[] cards = CardManager.GetCards(msgPlayCards.cardInfos);
                Array.Sort(cards, (Card card1, Card card2) => (int)card1.rank - (int)card2.rank);

                //出牌后，把客户端储存的数据中的相关卡牌删除
                for (int i = 0; i < cards.Length; i++)
                {
                    //删除手牌
                    for(int j = GameManager.cards.Count - 1; j >= 0; j--)
                    {
                        if (GameManager.cards[j].suit == cards[i].suit && GameManager.cards[j].rank == cards[i].rank)
                        {
                            GameManager.cards.RemoveAt(j);
                        }
                    }
                    //删除选中的牌
                    for(int j =GameManager.selectCards.Count - 1; j >= 0; j--)
                    {
                        if (GameManager.selectCards[j].suit == cards[i].suit && GameManager.selectCards[j].rank == cards[i].rank)
                        {
                            GameManager.selectCards.RemoveAt(j);
                        }
                    }
                }

                //把手牌重新生成一遍
                Card[] remainCards = CardManager.CardSort(GameManager.cards.ToArray());
                GenerateCard(remainCards);
            }

            MsgSwitchTurn msgSwitchTurn = new MsgSwitchTurn();
            NetManager.Send(msgSwitchTurn);
        }
    }

    /// <summary>
    /// 抢地主
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgRobLandlord(MsgBase msgBase)
    {
        MsgRobLandlord msgRobLandlord = msgBase as MsgRobLandlord;

        //发送消息切换回合
        MsgSwitchTurn msgSwitchTurn = new MsgSwitchTurn();

        //左右两边的玩家根据是否抢地主显示相关信息
        if (msgRobLandlord.isRob)
        {
            GameManager.SyncDestroy(msgRobLandlord.id);
            GameManager.SyncGenerate(msgRobLandlord.id, "Word/RobLandlord");
        }
        else
        {
            GameManager.SyncDestroy(msgRobLandlord.id);
            GameManager.SyncGenerate(msgRobLandlord.id, "Word/NotRobLandlord");
        }

        //还没有成为地主的话，msgRobLandlord.landLordId为空，不会更改图片
        SyncLandLord(msgRobLandlord.landLordId);

        //自己是地主，切换图片素材
        if(msgRobLandlord.landLordId == GameManager.playerId)
        {
            TurnLandLord();
        }

        if(msgRobLandlord.landLordId != "")
        {
            //产生地主了,就揭示三张底牌
            RevealThreeCards(GameManager.threeCards.ToArray());
            //切换玩家状态为出牌阶段
            GameManager.status = PlayerStatus.Play;
        }

        //抢地主的不是自己，后续逻辑不需要执行
        if(msgRobLandlord.id != GameManager.playerId)
        {
            return;
        }

        if (!msgRobLandlord.isNeedRob)
        {
            //下一家不需要抢地主，那么直接跳过
            msgSwitchTurn.round = 2;
            NetManager.Send(msgSwitchTurn);
            return;
        }
        else
        {
            //下一家需要抢地主  
            msgSwitchTurn.round = 1;
            NetManager.Send(msgSwitchTurn);
            return;
        }
    }

    //成为地主，跟换地主图片素材，并把三张底牌分配到玩家手中
    public void TurnLandLord()
    {
        //更换图片
        GameManager.isLandLord = true;
        GameObject go = Resources.Load<GameObject>("LandLord");
        Sprite sprite = go.GetComponent<SpriteRenderer>().sprite;
        playerObj.transform.Find("Image").GetComponent<Image>().sprite = sprite;

        //将底牌添加到地主手中,地主的手牌变成20张
        Card[] cards = new Card[20];

        //先将原先的手牌拷贝，再拷贝三张底牌
        //GameManager.cards.ToArray().CopyTo(cards, 0);
        Array.Copy(GameManager.cards.ToArray(), 0, cards, 0, 17);
        Array.Copy(GameManager.threeCards.ToArray(), 0, cards, 17, 3);

        cards = CardManager.CardSort(cards);
        GenerateCard(cards);

        //更新玩家手牌的数据
        for(int i = 0; i < 3; i++)
        {
            GameManager.cards.Add(GameManager.threeCards[i]);
        }
    }

    /// <summary>
    /// 如果是左右两边的玩家成为地主，更改他的地主图片
    /// </summary>
    /// <param name="id"></param>
    public void SyncLandLord(string id)
    {
        GameObject go = Resources.Load<GameObject>("LandLord");
        Sprite sprite = go.GetComponent<SpriteRenderer>().sprite;
        
        if(id == GameManager.leftPlayerId)
        {
            //左边的玩家是地主
            GameManager.leftPlayerInfoObj.transform.parent.Find("Image").GetComponent<Image>().sprite = sprite;
        }
        if (id == GameManager.rightPlayerId)
        {
            //右边的玩家是地主
            GameManager.rightPlayerInfoObj.transform.parent.Find("Image").GetComponent<Image>().sprite = sprite;
        }
    }

    /// <summary>
    /// 叫地主后，揭示上方的三张底牌
    /// </summary>
    /// <param name="cards"></param>
    public void RevealThreeCards(Card[] cards)
    {
        for(int i = 0; i < 3; i++)
        {
            string name = CardManager.GetName(cards[i]);
            Sprite sprite = Resources.Load<Sprite>("Card/" + name);
            GameManager.threeCardsObj.transform.GetChild(i).GetComponent<Image>().sprite = sprite;
        }
    }

    /// <summary>
    /// 玩家点击"叫地主"按钮
    /// </summary>
    public void OnCallLandlordButtonClick()
    {
        MsgCallLandlord msgCallLandlord = new MsgCallLandlord();
        msgCallLandlord.isCall = true;
        NetManager.Send(msgCallLandlord);
    }

    /// <summary>
    /// 玩家点击"不叫"按钮
    /// </summary>
    public void OnNotCallLandlordButtonClick()
    {
        MsgCallLandlord msgCallLandlord = new MsgCallLandlord();
        msgCallLandlord.isCall = false;
        NetManager.Send(msgCallLandlord);
    }

    /// <summary>
    /// 玩家点击"抢地主"按钮
    /// </summary>
    public void OnRobLandlordButtonClick()
    {
        MsgRobLandlord msgRobLandlord = new MsgRobLandlord();
        msgRobLandlord.isRob = true;
        NetManager.Send(msgRobLandlord);
    }
    
    /// <summary>
    /// 玩家点击"不抢"按钮
    /// </summary>
    public void OnNotRobLandlordButtonClick()
    {
        MsgRobLandlord msgRobLandlord = new MsgRobLandlord();
        msgRobLandlord.isRob = false;
        NetManager.Send(msgRobLandlord);
    }

    /// <summary>
    /// 玩家点击"出牌"按钮
    /// </summary>
    public void OnPlayCardButtonClick()
    {
        MsgPlayCards msgPlayCards = new MsgPlayCards();
        msgPlayCards.isPlay = true;
        msgPlayCards.cardInfos = CardManager.GetCardInfos(GameManager.selectCards.ToArray());
        NetManager.Send(msgPlayCards);
    }

    /// <summary>
    /// 玩家点击"不出"按钮
    /// </summary>
    public void OnNotPlayCardButtonClick()
    {

    }
}
