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

        GameManager.leftPlayerInfoObj   = skin.transform.Find("Player_Left/InfoObj").gameObject;
        GameManager.rightPlayerInfoObj  = skin.transform.Find("Player_Right/InfoObj").gameObject;

        callLandlordButton.gameObject.SetActive(false);
        notCallLandlordButton.gameObject.SetActive(false);
        robLandlordButton.gameObject.SetActive(false);
        notRobLandlordButton.gameObject.SetActive(false);

        //�����Ϣ����
        NetManager.AddMsgListener("MsgGetCardList", OnMsgGetCardList);
        NetManager.AddMsgListener("MsgGetStartPlayer", OnMsgGetStartPlayer);
        NetManager.AddMsgListener("MsgSwitchTurn", OnMsgSwitchTurn);
        NetManager.AddMsgListener("MsgGetOtherPlayers", OnMsgGetOtherPlayers);
        NetManager.AddMsgListener("MsgCallLandlord", OnMsgCallLandlord);
        NetManager.AddMsgListener("MsgReStart", OnMsgReStart);
        NetManager.AddMsgListener("MsgStartRobLandlord", OnMsgStartRobLandlord);

        //��Ӱ�ť�¼�
        callLandlordButton.onClick.AddListener(OnCallLandlordButtonClick);
        notCallLandlordButton.onClick.AddListener(OnNotCallLandlordButtonClick);
        robLandlordButton.onClick.AddListener(OnRobLandlordButtonClick);
        notRobLandlordButton.onClick.AddListener(OnNotRobLandlordButtonClick);

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

        if (msgCallLandlord.isCall)
        {
            GameManager.SyncGenerate(msgCallLandlord.id, "Word/CallLandlord");
        }
        else
        {
            GameManager.SyncGenerate(msgCallLandlord.id, "Word/NotCallLandlord");
        }

        if(msgCallLandlord.id != GameManager.playerId)
        {
            return;
        }

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
                break;
            default:
                break;
        }

        MsgSwitchTurn msgSwitchTurn = new MsgSwitchTurn();
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

        //Ȼ�����»�ȡһ�ο��ƣ�������MsgReStart����Ϣ���͸�����˺󣬷�������������ϴ��
        MsgGetCardList msgGetCardList = new MsgGetCardList();
        NetManager.Send(msgGetCardList);
    }

    public void OnMsgStartRobLandlord(MsgBase msgBase)
    {
        MsgStartRobLandlord msgStartRobLandlord = msgBase as MsgStartRobLandlord;

        //���ĵ�ǰ�׶�Ϊ�������׶�
        GameManager.status = PlayerStatus.Rob;
    }

    public void TurnLandLord()
    {
        GameManager.isLandLord = true;
        GameObject go = Resources.Load<GameObject>("LandLord");
        Sprite sprite = go.GetComponent<SpriteRenderer>().sprite;
        playerObj.transform.Find("Image").GetComponent<Image>().sprite = sprite;
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

    }

    /// <summary>
    /// ��ҵ��"����"��ť
    /// </summary>
    public void OnNotRobLandlordButtonClick()
    {

    }
}
