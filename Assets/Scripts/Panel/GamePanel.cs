using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel
{
    /// <summary>
    /// �����Ϸ����
    /// </summary>
    public GameObject playerObj;
    /// <summary>
    /// "�е���"�İ�ť
    /// </summary>
    public Button callLandlordButton;
    /// <summary>
    /// "����"�İ�ť
    /// </summary>
    public Button notCallLandlordButton;
    /// <summary>
    /// "������"�İ�ť
    /// </summary>
    public Button robLandlordButton;
    /// <summary>
    /// "����"�İ�ť
    /// </summary>
    public Button notRobLandlordButton;
    /// <summary>
    /// "����"��ť
    /// </summary>
    public Button playCardButton;
    /// <summary>
    /// "����"��ť
    /// </summary>
    public Button notPlayCardButton;

    public override void OnInit()
    {
        skinPath = "GamePanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        //Ѱ�����
        playerObj                       = skin.transform.Find("Player").gameObject;
        callLandlordButton              = skin.transform.Find("CallLandlord_Btn").GetComponent<Button>();
        notCallLandlordButton           = skin.transform.Find("NotCallLandlord_Btn").GetComponent<Button>();
        robLandlordButton               = skin.transform.Find("RobLandlord_Btn").GetComponent<Button>();
        notRobLandlordButton            = skin.transform.Find("NotRobLandlord_Btn").GetComponent<Button>();
        playCardButton                  = skin.transform.Find("PlayCard_Btn").GetComponent<Button>();
        notPlayCardButton               = skin.transform.Find("NotPlayCard_Btn").GetComponent<Button>();

        //��ʼ�����
        GameManager.leftPlayerInfoObj   = skin.transform.Find("Player_Left/InfoObj").gameObject;
        GameManager.rightPlayerInfoObj  = skin.transform.Find("Player_Right/InfoObj").gameObject;
        GameManager.threeCardsObj       = skin.transform.Find("ThreeCards").gameObject;

        callLandlordButton.gameObject.SetActive(false);
        notCallLandlordButton.gameObject.SetActive(false);
        robLandlordButton.gameObject.SetActive(false);
        notRobLandlordButton.gameObject.SetActive(false);
        playCardButton.gameObject.SetActive(false);
        notPlayCardButton.gameObject.SetActive(false);

        //�����Ϣ����
        NetManager.AddMsgListener("MsgGetCardList", OnMsgGetCardList);
        NetManager.AddMsgListener("MsgGetStartPlayer", OnMsgGetStartPlayer);
        NetManager.AddMsgListener("MsgSwitchTurn", OnMsgSwitchTurn);
        NetManager.AddMsgListener("MsgGetOtherPlayers", OnMsgGetOtherPlayers);
        NetManager.AddMsgListener("MsgCallLandlord", OnMsgCallLandlord);
        NetManager.AddMsgListener("MsgReStart", OnMsgReStart);
        NetManager.AddMsgListener("MsgStartRobLandlord", OnMsgStartRobLandlord);
        NetManager.AddMsgListener("MsgRobLandlord", OnMsgRobLandlord);
        NetManager.AddMsgListener("MsgPlayCards", OnMsgPlayCards);

        //��Ӱ�ť�¼�
        callLandlordButton.onClick.AddListener(OnCallLandlordButtonClick);
        notCallLandlordButton.onClick.AddListener(OnNotCallLandlordButtonClick);
        robLandlordButton.onClick.AddListener(OnRobLandlordButtonClick);
        notRobLandlordButton.onClick.AddListener(OnNotRobLandlordButtonClick);
        playCardButton.onClick.AddListener(OnPlayCardButtonClick);
        notPlayCardButton.onClick.AddListener(OnNotPlayCardButtonClick);

        //����������Ϣ����ȡͬ����������ҵ�id
        MsgGetOtherPlayers msgGetOtherPlayers = new MsgGetOtherPlayers();
        NetManager.Send(msgGetOtherPlayers);

        //����������Ϣ���Ȼ�ȡһ���˿���
        MsgGetCardList msgGetCardList = new MsgGetCardList();
        NetManager.Send(msgGetCardList);

        //����������Ϣ,֪�����ĸ���ҿ�ʼ�е���
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
            //���ݿ�����Ϣ���쿨��
            Card card = new Card(msgGetCardList.cardInfos[i].suit, msgGetCardList.cardInfos[i].rank);
            GameManager.cards.Add(card);
        }

        //��������
        Card[] cards = CardManager.CardSort(GameManager.cards.ToArray());

        //ʵ�������ɿ���
        GenerateCard(cards);

        //��ȡ���ŵ���
        for(int i = 0; i < 3; i++)
        {
            Card card = new Card(msgGetCardList.threeCards[i].suit, msgGetCardList.threeCards[i].rank);
            GameManager.threeCards.Add(card);
        }
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void CardSort()
    {

    }

    /// <summary>
    /// ʵ�������ɿ���
    /// </summary>
    /// <param name="cards"></param>
    public void GenerateCard(Card[] cards)
    {
        //ÿ������ǰ����գ������ظ�
        Transform cardsTrans = playerObj.transform.Find("Cards");
        for(int i = cardsTrans.childCount - 1; i >= 0; i--)
        {
            Destroy(cardsTrans.GetChild(i).gameObject);
        }

        for (int i = 0; i < cards.Length; i++)
        {
            //�����Ƶ����������ƵĶ��󣬲���ȡ�޸�ͼƬ��Դ
            string name = CardManager.GetName(cards[i]);
            GameObject cardObj = new GameObject(name);
            Image image = cardObj.AddComponent<Image>();
            Sprite sprite = Resources.Load<Sprite>("Card/" + name);
            image.sprite = sprite;
            image.rectTransform.sizeDelta = new Vector2(80, 100);
            cardObj.transform.SetParent(cardsTrans, false);
            cardObj.layer = LayerMask.NameToLayer("UI");

            //�ѽű�����ȥ
            cardObj.AddComponent<CardUI>();
        }
    }

    /// <summary>
    /// ��ǰ����Ƿ���Ҫ�е���
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
    /// ��ҽ�/����������л�����һ�����
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgSwitchTurn(MsgBase msgBase)
    {
        MsgSwitchTurn msgSwitchTurn = msgBase as MsgSwitchTurn;
        switch (GameManager.status)
        {
            case PlayerStatus.Call:
                //��һ���е�������Ҿ��ǵ�ǰ�ͻ��˵���ң�����ʾ�е����İ�ť
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
                //��һ������������Ҿ��ǵ�ǰ�ͻ��˵���ң�����ʾ�������İ�ť
                if(msgSwitchTurn.nextPlayerId == GameManager.playerId)
                {
                    robLandlordButton.gameObject.SetActive(true);
                    notRobLandlordButton.gameObject.SetActive(true);

                    //�������׶Σ��е����İ�ť����
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
                //�Ȱѽе����������İ�ťȫ������
                robLandlordButton.gameObject.SetActive(false);
                notRobLandlordButton.gameObject.SetActive(false);
                callLandlordButton.gameObject.SetActive(false);
                notCallLandlordButton.gameObject.SetActive(false);

                if (msgSwitchTurn.nextPlayerId == GameManager.playerId)
                {
                    playCardButton.gameObject.SetActive(true);
                    notPlayCardButton.gameObject.SetActive(true);

                    if (GameManager.canNotPlay)//��ǰ״̬������ ������
                    {
                        //��ɫ����
                        notPlayCardButton.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                        //���Խ���
                        notPlayCardButton.enabled = true;
                    }
                    else//��ǰ״̬�²����� ������
                    {
                        //��ɫ�䰵
                        notPlayCardButton.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.6f);
                        //�����Խ���
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
    /// ���ͬ�����������
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgGetOtherPlayers(MsgBase msgBase)
    {
        MsgGetOtherPlayers msgGetOtherPlayers = msgBase as MsgGetOtherPlayers;

        GameManager.leftPlayerId = msgGetOtherPlayers.leftPlayerId;
        GameManager.rightPlayerId = msgGetOtherPlayers.rightPlayerId;
    }

    /// <summary>
    /// ����е������߼�
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgCallLandlord(MsgBase msgBase)
    {
        MsgCallLandlord msgCallLandlord = msgBase as MsgCallLandlord;
        MsgSwitchTurn msgSwitchTurn = new MsgSwitchTurn();

        //�������ߵ���Ҹ����Ƿ�е�����ʾ�����Ϣ
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

        //��������ҳ�Ϊ����������ͼƬ,����ʾ���ŵ���
        if(msgCallLandlord.result == 3)
        {
            //���ĵ���ͼƬ
            SyncLandLord(msgCallLandlord.id);
            //��ʾ�Ϸ����ŵ���
            RevealThreeCards(GameManager.threeCards.ToArray());
            //�л����״̬Ϊ���ƽ׶�
            GameManager.status = PlayerStatus.Play;
        }

        //���ǵ�ǰ��ң�����������߼�����Ҫִ��
        if(msgCallLandlord.id != GameManager.playerId)
        {
            return;
        }

        //��ʼ����������߼�
        switch (msgCallLandlord.result)
        {
            case 0:
                break;
            case 1://������
                MsgStartRobLandlord msgStartRobLandlord = new MsgStartRobLandlord();
                NetManager.Send(msgStartRobLandlord);
                break;
            case 2://����ϴ��
                MsgReStart msgReStart = new MsgReStart();
                NetManager.Send(msgReStart);
                break;
            case 3://�Լ��ǵ���
                TurnLandLord();
                //�Լ��е����ɹ��Ļغϲ���Ҫ�л��غ�
                msgSwitchTurn.round = 0;
                break;
            default:
                break;
        }

        //������Ϣ�л��غ�
        NetManager.Send(msgSwitchTurn);
    }

    /// <summary>
    /// �����е��������¿�ʼ��ϴ��
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgReStart(MsgBase msgBase)
    {
        MsgReStart msgReStart = msgBase as MsgReStart;

        //�Ȱ�ԭ��Cards������������п�������
        Transform cardTrans = playerObj.transform.Find("Cards");
        for (int i = cardTrans.childCount - 1; i >= 0; i--) 
        {
            Destroy(cardTrans.GetChild(i).gameObject);
        }
        //����������еĿ������
        GameManager.cards.Clear();
        GameManager.threeCards.Clear();

        //Ȼ�����»�ȡһ�ο��ƣ�������MsgReStart����Ϣ���͸�����˺󣬷�������������ϴ��
        MsgGetCardList msgGetCardList = new MsgGetCardList();
        NetManager.Send(msgGetCardList);
    }

    /// <summary>
    /// ��ʼ���������л���ص�״̬
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgStartRobLandlord(MsgBase msgBase)
    {
        MsgStartRobLandlord msgStartRobLandlord = msgBase as MsgStartRobLandlord;

        //���ĵ�ǰ�׶�Ϊ�������׶�
        GameManager.status = PlayerStatus.Rob;
    }

    /// <summary>
    /// ������Ƶ���Ϣ
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgPlayCards(MsgBase msgBase)
    {
        MsgPlayCards msgPlayCards = msgBase as MsgPlayCards;

        //���Գ������͵��ж�
        Debug.Log("GamePanel.OnMsgPlayCards. ���Ƶ�����Ϊ��" + (CardManager.CardType)msgPlayCards.cardType);

        //���� ������Ƿ���� ������ ���߼�
        //����Ϣ�ᷢ�͸�������ң�
        //��Ȼ����������ʱ������ҵ�GameManager.canNotPlay�����޸ģ���ֻ�����ڲ�������һ�����Ҫ��ֵ��
        //�´γ��ƵȲ����󣬸�ֵ�ᱻ���¸�ֵ
        GameManager.canNotPlay = msgPlayCards.canNotPlay;

        //����������ҵĳ����߼������ƾ���ʾ������ƣ���������ʾ����
        if (msgPlayCards.result)
        {
            if (msgPlayCards.isPlay)
            {
                Card[] cards = CardManager.GetCards(msgPlayCards.cardInfos);
                Array.Sort(cards, (Card card1, Card card2) => (int)card1.rank - (int)card2.rank);

                //�������Ҫ��ʾ������
                GameManager.SyncDestroy(msgPlayCards.id);

                //����ͬ���Ŀ���
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

        //��������ǰ�ͻ�����ҵĳ����߼�
        if(msgPlayCards.id != GameManager.playerId)
        {
            return;
        }

        //���ؽ��Ϊtrue��˵�������ɹ�
        if (msgPlayCards.result)
        {
            //��һ�֣����Ƴɹ�
            if (msgPlayCards.isPlay)
            {
                Card[] cards = CardManager.GetCards(msgPlayCards.cardInfos);
                Array.Sort(cards, (Card card1, Card card2) => (int)card1.rank - (int)card2.rank);

                //���ƺ󣬰ѿͻ��˴���������е���ؿ���ɾ��
                for (int i = 0; i < cards.Length; i++)
                {
                    //ɾ������
                    for(int j = GameManager.cards.Count - 1; j >= 0; j--)
                    {
                        if (GameManager.cards[j].suit == cards[i].suit && GameManager.cards[j].rank == cards[i].rank)
                        {
                            GameManager.cards.RemoveAt(j);
                        }
                    }
                    //ɾ��ѡ�е���
                    for(int j =GameManager.selectCards.Count - 1; j >= 0; j--)
                    {
                        if (GameManager.selectCards[j].suit == cards[i].suit && GameManager.selectCards[j].rank == cards[i].rank)
                        {
                            GameManager.selectCards.RemoveAt(j);
                        }
                    }
                }

                //��������������һ��
                Card[] remainCards = CardManager.CardSort(GameManager.cards.ToArray());
                GenerateCard(remainCards);
            }

            MsgSwitchTurn msgSwitchTurn = new MsgSwitchTurn();
            NetManager.Send(msgSwitchTurn);
        }
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgRobLandlord(MsgBase msgBase)
    {
        MsgRobLandlord msgRobLandlord = msgBase as MsgRobLandlord;

        //������Ϣ�л��غ�
        MsgSwitchTurn msgSwitchTurn = new MsgSwitchTurn();

        //�������ߵ���Ҹ����Ƿ���������ʾ�����Ϣ
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

        //��û�г�Ϊ�����Ļ���msgRobLandlord.landLordIdΪ�գ��������ͼƬ
        SyncLandLord(msgRobLandlord.landLordId);

        //�Լ��ǵ������л�ͼƬ�ز�
        if(msgRobLandlord.landLordId == GameManager.playerId)
        {
            TurnLandLord();
        }

        if(msgRobLandlord.landLordId != "")
        {
            //����������,�ͽ�ʾ���ŵ���
            RevealThreeCards(GameManager.threeCards.ToArray());
            //�л����״̬Ϊ���ƽ׶�
            GameManager.status = PlayerStatus.Play;
        }

        //�������Ĳ����Լ��������߼�����Ҫִ��
        if(msgRobLandlord.id != GameManager.playerId)
        {
            return;
        }

        if (!msgRobLandlord.isNeedRob)
        {
            //��һ�Ҳ���Ҫ����������ôֱ������
            msgSwitchTurn.round = 2;
            NetManager.Send(msgSwitchTurn);
            return;
        }
        else
        {
            //��һ����Ҫ������  
            msgSwitchTurn.round = 1;
            NetManager.Send(msgSwitchTurn);
            return;
        }
    }

    //��Ϊ��������������ͼƬ�زģ��������ŵ��Ʒ��䵽�������
    public void TurnLandLord()
    {
        //����ͼƬ
        GameManager.isLandLord = true;
        GameObject go = Resources.Load<GameObject>("LandLord");
        Sprite sprite = go.GetComponent<SpriteRenderer>().sprite;
        playerObj.transform.Find("Image").GetComponent<Image>().sprite = sprite;

        //��������ӵ���������,���������Ʊ��20��
        Card[] cards = new Card[20];

        //�Ƚ�ԭ�ȵ����ƿ������ٿ������ŵ���
        //GameManager.cards.ToArray().CopyTo(cards, 0);
        Array.Copy(GameManager.cards.ToArray(), 0, cards, 0, 17);
        Array.Copy(GameManager.threeCards.ToArray(), 0, cards, 17, 3);

        cards = CardManager.CardSort(cards);
        GenerateCard(cards);

        //����������Ƶ�����
        for(int i = 0; i < 3; i++)
        {
            GameManager.cards.Add(GameManager.threeCards[i]);
        }
    }

    /// <summary>
    /// ������������ߵ���ҳ�Ϊ�������������ĵ���ͼƬ
    /// </summary>
    /// <param name="id"></param>
    public void SyncLandLord(string id)
    {
        GameObject go = Resources.Load<GameObject>("LandLord");
        Sprite sprite = go.GetComponent<SpriteRenderer>().sprite;
        
        if(id == GameManager.leftPlayerId)
        {
            //��ߵ�����ǵ���
            GameManager.leftPlayerInfoObj.transform.parent.Find("Image").GetComponent<Image>().sprite = sprite;
        }
        if (id == GameManager.rightPlayerId)
        {
            //�ұߵ�����ǵ���
            GameManager.rightPlayerInfoObj.transform.parent.Find("Image").GetComponent<Image>().sprite = sprite;
        }
    }

    /// <summary>
    /// �е����󣬽�ʾ�Ϸ������ŵ���
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
    /// ��ҵ��"�е���"��ť
    /// </summary>
    public void OnCallLandlordButtonClick()
    {
        MsgCallLandlord msgCallLandlord = new MsgCallLandlord();
        msgCallLandlord.isCall = true;
        NetManager.Send(msgCallLandlord);
    }

    /// <summary>
    /// ��ҵ��"����"��ť
    /// </summary>
    public void OnNotCallLandlordButtonClick()
    {
        MsgCallLandlord msgCallLandlord = new MsgCallLandlord();
        msgCallLandlord.isCall = false;
        NetManager.Send(msgCallLandlord);
    }

    /// <summary>
    /// ��ҵ��"������"��ť
    /// </summary>
    public void OnRobLandlordButtonClick()
    {
        MsgRobLandlord msgRobLandlord = new MsgRobLandlord();
        msgRobLandlord.isRob = true;
        NetManager.Send(msgRobLandlord);
    }
    
    /// <summary>
    /// ��ҵ��"����"��ť
    /// </summary>
    public void OnNotRobLandlordButtonClick()
    {
        MsgRobLandlord msgRobLandlord = new MsgRobLandlord();
        msgRobLandlord.isRob = false;
        NetManager.Send(msgRobLandlord);
    }

    /// <summary>
    /// ��ҵ��"����"��ť
    /// </summary>
    public void OnPlayCardButtonClick()
    {
        MsgPlayCards msgPlayCards = new MsgPlayCards();
        msgPlayCards.isPlay = true;
        msgPlayCards.cardInfos = CardManager.GetCardInfos(GameManager.selectCards.ToArray());
        NetManager.Send(msgPlayCards);
    }

    /// <summary>
    /// ��ҵ��"����"��ť
    /// </summary>
    public void OnNotPlayCardButtonClick()
    {

    }
}
