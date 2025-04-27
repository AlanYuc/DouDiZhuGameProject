using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    private Button startButton;
    private Button prepareButton;
    private Button exitButton;
    private Transform content;
    private GameObject playerObj;

    public override void OnInit()
    {
        skinPath = "RoomPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        //Ѱ�����
        startButton     = skin.transform.Find("Join/Start_Button").GetComponent<Button>();
        prepareButton   = skin.transform.Find("Join/Prepare_Button").GetComponent<Button>();
        exitButton      = skin.transform.Find("Join/Exit_Button").GetComponent<Button>();
        content         = skin.transform.Find("PlayerList/Scroll View/Viewport/Content");
        playerObj       = skin.transform.Find("Player").gameObject;

        playerObj.SetActive(false);

        //��Ӱ�ť�����¼�
        startButton.onClick.AddListener(OnStartClick);
        prepareButton.onClick.AddListener(OnPrepareClick);
        exitButton.onClick.AddListener(OnExitClick);

        //���Э�����
        NetManager.AddMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.AddMsgListener("MsgPrepare", OnMsgPrepare);

        //���뷿��ʱ�Ȼ�ȡ������Ϣ
        //����˷���PlayerInfo[]�������Ϣ
        MsgGetRoomInfo msgGetRoomInfo = new MsgGetRoomInfo();
        NetManager.Send(msgGetRoomInfo);
    }

    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.RemoveMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.RemoveMsgListener("MsgPrepare", OnMsgPrepare);
    }

    public void OnStartClick()
    {

    }

    public void OnPrepareClick()
    {
        MsgPrepare msgPrepare = new MsgPrepare();
        NetManager.Send(msgPrepare);
    }

    public void OnExitClick()
    {
        MsgLeaveRoom msgLeaveRoom = new MsgLeaveRoom();
        NetManager.Send(msgLeaveRoom);
    }

    /// <summary>
    /// ����շ����ڵ�������ң��ٸ��ݷ��ص�msgGetRoomInfo.players.Length���ɶ�Ӧ���������
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgGetRoomInfo(MsgBase msgBase)
    {
        MsgGetRoomInfo msgGetRoomInfo = msgBase as MsgGetRoomInfo;

        for(int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        if(msgGetRoomInfo.players == null)
        {
            return;
        }

        for(int i =0;i< msgGetRoomInfo.players.Length; i++)
        {
            GeneratePlayer(msgGetRoomInfo.players[i]);
        }
    }

    public void GeneratePlayer(PlayerInfo playerInfo)
    {
        //ʵ������Ҷ���
        GameObject go = Instantiate(playerObj);
        go.transform.SetParent(content);
        go.SetActive(true);
        go.transform.localScale = Vector3.one;

        //Ѱ����Ҷ����ڵ����
        Transform playerTrans   = go.transform;
        Text playerIdText       = playerTrans.Find("IDValue_Text").GetComponent<Text>();
        Text beanText           = playerTrans.Find("BeanValue_Text").GetComponent<Text>();
        Text isPrepareText      = playerTrans.Find("StatusValue_Text").GetComponent<Text>();
        Image hostImg           = playerTrans.Find("Host_Img").GetComponent<Image>();

        //�����������
        playerIdText.text = playerInfo.playerID;
        beanText.text = playerInfo.bean.ToString();

        //�������׼��״̬
        if (playerInfo.isPrepare)
        {
            isPrepareText.text = "��׼��";
        }
        else
        {
            isPrepareText.text = "δ׼��";
        }

        //��������Ƿ��Ƿ�������ʾ��Ӧ����Ϣ
        if (playerInfo.isHost)
        {
            hostImg.gameObject.SetActive(true);
        }
        else
        {
            hostImg.gameObject.SetActive(false);
        }

        //�����ǰ����Ƿ���������ʾ��Ӧ�İ�ť
        if(playerInfo.playerID == GameManager.playerId)
        {
            GameManager.isHost = playerInfo.isHost;
            if (GameManager.isHost)
            {
                startButton.gameObject.SetActive(true);
                prepareButton.gameObject.SetActive(false);
            }
            else
            {
                startButton.gameObject.SetActive(false);
                prepareButton.gameObject.SetActive(true);
            }
        }
    }

    public void OnMsgLeaveRoom(MsgBase msgBase)
    {
        MsgLeaveRoom msgLeaveRoom = msgBase as MsgLeaveRoom;

        //�ж��Ƿ��˳�����ɹ�
        if (msgLeaveRoom.result)
        {
            PanelManager.Open<TipPanel>("�˳�����");
            //�򿪷����б����
            PanelManager.Open<RoomListPanel>();
            //�رյ�ǰ�ķ������
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("�˳�����ʧ��");
        }
    }

    public void OnMsgPrepare(MsgBase msgBase)
    {
        MsgPrepare msgPrepare = msgBase as MsgPrepare;
        if(msgPrepare.isPrepare == false)
        {
            return;
        }

        //�����˷�����Ϣ�����»�ȡ��ǰ�����ڵ��������
        MsgGetRoomInfo msgGetRoomInfo = new MsgGetRoomInfo();
        NetManager.Send(msgGetRoomInfo);


    }
}
