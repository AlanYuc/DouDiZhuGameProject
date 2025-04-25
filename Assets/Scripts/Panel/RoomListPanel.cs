using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel
{
    private Text playerIdText;
    private Text beanText;
    private Button createButton;
    private Button refreshButton;
    private Transform content;
    private GameObject roomObj;

    /// <summary>
    /// ��¼��������ʱ�ķ���id
    /// </summary>
    private int roomId;

    public override void OnInit()
    {
        skinPath = "RoomListPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        //Ѱ�����
        playerIdText    = skin.transform.Find("Head/IDValue_Text").GetComponent<Text>();
        beanText        = skin.transform.Find("Head/BeanValue_Text").GetComponent<Text>();
        createButton    = skin.transform.Find("CreateRoom/Create_Button").GetComponent<Button>();
        refreshButton   = skin.transform.Find("CreateRoom/Refresh_Button").GetComponent<Button>();
        content         = skin.transform.Find("RoomList/Scroll View/Viewport/Content");
        roomObj         = skin.transform.Find("Room").gameObject;

        roomObj.SetActive(false);
        playerIdText.text = GameManager.playerId;

        //��Ӱ�ť�¼�����
        createButton.onClick.AddListener(OnCreateClick);
        refreshButton.onClick.AddListener(OnRefreshClick);

        //���Э��ļ���
        NetManager.AddMsgListener("MsgGetPlayer", OnMsgGetPlayer);
        NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.AddMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);

        //��ʼ�����˷�����Ϣ
        //����RoomListPanel�£���Ҫ��������Ϣ

        //��Ҫ�����˻�ȡ��ҵ�ǰ�Ļ��ֶ�����
        //MsgGetPlayer����һ��bean����������Ҫ�ͻ��˸�ֵ�����ǵȷ���˷���
        MsgGetPlayer msgGetPlayer = new MsgGetPlayer();
        NetManager.Send(msgGetPlayer);

        //��Ҫ�����˻�ȡ��ǰ���б��ڷ��������
        //ͬ�ϣ�����˷��ذ������з��������
        MsgGetRoomList msgGetRoomList = new MsgGetRoomList();
        NetManager.Send(msgGetRoomList);
    }

    public override void OnClose()
    {
        //�Ƴ�Э��ļ���
        NetManager.RemoveMsgListener("MsgGetPlayer", OnMsgGetPlayer);
        NetManager.RemoveMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.RemoveMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.RemoveMsgListener("MsgEnterRoom", OnMsgEnterRoom);
    }

    public void OnCreateClick()
    {
        MsgCreateRoom msgCreateRoom = new MsgCreateRoom();
        NetManager.Send(msgCreateRoom);
    }
    public void OnRefreshClick()
    {
        MsgGetRoomList msgGetRoomList = new MsgGetRoomList();
        NetManager.Send(msgGetRoomList);
    }

    public void OnMsgGetPlayer(MsgBase msgBase)
    {
        MsgGetPlayer msgGetPlayer = msgBase as MsgGetPlayer;
        beanText.text = msgGetPlayer.bean.ToString();

        //to do
        //�ѻ��ֶ�����������Ϸ�����
    }

    public void OnMsgCreateRoom(MsgBase msgBase)
    {
        MsgCreateRoom msgCreateRoom = msgBase as MsgCreateRoom;
        if (msgCreateRoom.result)
        {
            PanelManager.Open<TipPanel>("��������ɹ�");
            //�򿪷������
            PanelManager.Open<RoomPanel>();
            //�رյ�ǰ�ķ����б����
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("��������ʧ��");
        }
    }

    /// <summary>
    /// ��ȡ���еķ��䣬����ˢ�·����б�
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgGetRoomList(MsgBase msgBase)
    {
        MsgGetRoomList msgGetRoomList = msgBase as MsgGetRoomList;

        /*
         * ������������content�µķ�����󣬱����±�������������
         * ���磺��һ���±�Ϊ0�����ٺ󣬵ڶ���������±���1��Ϊ0������i����Ϊ1
         */
        for (int i = content.childCount - 1; i >= 0; i--)
        {
            Destroy(content.GetChild(i).gameObject);
        }

        if(msgGetRoomList.rooms == null)
        {
            return;
        }

        for(int i = 0; i < msgGetRoomList.rooms.Length; i++)
        {
            GenerateRoom(msgGetRoomList.rooms[i]);
        }
    }

    public void OnMsgEnterRoom(MsgBase msgBase)
    {
        MsgEnterRoom msgEnterRoom = msgBase as MsgEnterRoom;

        if (msgEnterRoom.result)
        {
            //��������ķ������
            PanelManager.Open<RoomPanel>();
            //�رշ����б����
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("���뷿��ʧ��");
        }
    }

    public void GenerateRoom(RoomInfo roomInfo)
    {
        //ʵ��������Room����
        GameObject go = Instantiate(roomObj);
        go.transform.SetParent(content);
        //��ǰ�ķ��������ͨ��roomObj��¡���ɵģ���roomObj�Ѿ�������Ϊ������
        go.SetActive(true);
        go.transform.localScale = Vector3.one;

        //�޸�Room�����ڵ�����

        //Ѱ�����
        Transform roomTrans = go.transform;
        Text roomIdText     = roomTrans.Find("RoomIDValue_Text").GetComponent<Text>();
        Text countText      = roomTrans.Find("CountValue_Text").GetComponent<Text>();
        Text isPrepareText  = roomTrans.Find("StatusValue_Text").GetComponent<Text>();
        Button joinButton   = roomTrans.Find("Join_Button").GetComponent<Button>();

        //�޸�����
        roomIdText.text     = roomInfo.roomID.ToString();
        countText.text      = roomInfo.count.ToString();

        //��¼��ǰ�����ķ���id
        roomId              = roomInfo.roomID;


        if (roomInfo.isPrepare)
        {
            isPrepareText.text = "׼����";
        }
        else
        {
            isPrepareText.text = "�ѿ�ʼ";
        }

        //��Ӽ��밴ť���¼�
        joinButton.onClick.AddListener(OnJoinClick);
    }

    public void OnJoinClick()
    {
        MsgEnterRoom msgEnterRoom = new MsgEnterRoom();
        msgEnterRoom.roomID = roomId;
        NetManager.Send(msgEnterRoom);
    }
}
