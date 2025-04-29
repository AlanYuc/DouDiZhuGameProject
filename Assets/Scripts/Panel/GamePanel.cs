using System.Collections;
using System.Collections.Generic;
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
        playerObj               = skin.transform.Find("Player").gameObject;
        callLandlordButton      = skin.transform.Find("CallLandlord_Btn").GetComponent<Button>();
        notCallLandlordButton   = skin.transform.Find("NotCallLandlord_Btn").GetComponent<Button>();
        robLandlordButton       = skin.transform.Find("RobLandlord_Btn").GetComponent<Button>();
        notRobLandlordButton    = skin.transform.Find("NotRobLandlord_Btn").GetComponent<Button>();

        callLandlordButton.gameObject.SetActive(false);
        notCallLandlordButton.gameObject.SetActive(false);
        robLandlordButton.gameObject.SetActive(false);
        notRobLandlordButton.gameObject.SetActive(false);

        //�����Ϣ����
        NetManager.AddMsgListener("MsgGetCardList", OnMsgGetCardList);
        NetManager.AddMsgListener("MsgGetStartPlayer", OnMsgGetStartPlayer);

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
    }

    public void OnMsgGetCardList(MsgBase msgBase)
    {
        MsgGetCardList msgGetCardList = msgBase as MsgGetCardList;

        for(int i = 0; i < CardManager.maxHandSize; i++)
        {
            //���ݿ�����Ϣ���쿨��
            Card card = new Card(msgGetCardList.cardInfos[i].suit, msgGetCardList.cardInfos[i].rank);
            GameManager.cards.Add(card);
        }

        //ʵ�������ɿ���
        GenerateCard(GameManager.cards.ToArray());
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
}
