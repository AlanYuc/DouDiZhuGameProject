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
    /// 记录创建房间时的房间id
    /// </summary>
    private int roomId;

    public override void OnInit()
    {
        skinPath = "RoomListPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        //寻找组件
        playerIdText    = skin.transform.Find("Head/IDValue_Text").GetComponent<Text>();
        beanText        = skin.transform.Find("Head/BeanValue_Text").GetComponent<Text>();
        createButton    = skin.transform.Find("CreateRoom/Create_Button").GetComponent<Button>();
        refreshButton   = skin.transform.Find("CreateRoom/Refresh_Button").GetComponent<Button>();
        content         = skin.transform.Find("RoomList/Scroll View/Viewport/Content");
        roomObj         = skin.transform.Find("Room").gameObject;

        roomObj.SetActive(false);
        playerIdText.text = GameManager.playerId;

        //添加按钮事件监听
        createButton.onClick.AddListener(OnCreateClick);
        refreshButton.onClick.AddListener(OnRefreshClick);

        //添加协议的监听
        NetManager.AddMsgListener("MsgGetPlayer", OnMsgGetPlayer);
        NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);
        NetManager.AddMsgListener("MsgGetRoomList", OnMsgGetRoomList);
        NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);

        //开始向服务端发送消息
        //处理RoomListPanel下，需要的所有消息

        //需要向服务端获取玩家当前的欢乐豆数量
        //MsgGetPlayer中有一个bean参数，不需要客户端赋值，而是等服务端返回
        MsgGetPlayer msgGetPlayer = new MsgGetPlayer();
        NetManager.Send(msgGetPlayer);

        //需要向服务端获取当前的列表内房间的数量
        //同上，服务端返回包含所有房间的数组
        MsgGetRoomList msgGetRoomList = new MsgGetRoomList();
        NetManager.Send(msgGetRoomList);
    }

    public override void OnClose()
    {
        //移除协议的监听
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
        //把欢乐斗给到进入游戏的玩家
    }

    public void OnMsgCreateRoom(MsgBase msgBase)
    {
        MsgCreateRoom msgCreateRoom = msgBase as MsgCreateRoom;
        if (msgCreateRoom.result)
        {
            PanelManager.Open<TipPanel>("创建房间成功");
            //打开房间面板
            PanelManager.Open<RoomPanel>();
            //关闭当前的房间列表面板
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("创建房间失败");
        }
    }

    /// <summary>
    /// 获取所有的房间，用于刷新房间列表
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgGetRoomList(MsgBase msgBase)
    {
        MsgGetRoomList msgGetRoomList = msgBase as MsgGetRoomList;

        /*
         * 倒序销毁所有content下的房间对象，避免下标索引出现问题
         * 比如：第一个下标为0，销毁后，第二个对象的下标会从1变为0，但是i增加为1
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
            //打开所进入的房间面板
            PanelManager.Open<RoomPanel>();
            //关闭房间列表面板
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("加入房间失败");
        }
    }

    public void GenerateRoom(RoomInfo roomInfo)
    {
        //实例化生成Room对象
        GameObject go = Instantiate(roomObj);
        go.transform.SetParent(content);
        //当前的房间对象是通过roomObj克隆生成的，而roomObj已经被设置为隐藏了
        go.SetActive(true);
        go.transform.localScale = Vector3.one;

        //修改Room对象内的数据

        //寻找组件
        Transform roomTrans = go.transform;
        Text roomIdText     = roomTrans.Find("RoomIDValue_Text").GetComponent<Text>();
        Text countText      = roomTrans.Find("CountValue_Text").GetComponent<Text>();
        Text isPrepareText  = roomTrans.Find("StatusValue_Text").GetComponent<Text>();
        Button joinButton   = roomTrans.Find("Join_Button").GetComponent<Button>();

        //修改数据
        roomIdText.text     = roomInfo.roomID.ToString();
        countText.text      = roomInfo.count.ToString();

        //记录当前创建的房间id
        roomId              = roomInfo.roomID;


        if (roomInfo.isPrepare)
        {
            isPrepareText.text = "准备中";
        }
        else
        {
            isPrepareText.text = "已开始";
        }

        //添加加入按钮的事件
        joinButton.onClick.AddListener(OnJoinClick);
    }

    public void OnJoinClick()
    {
        MsgEnterRoom msgEnterRoom = new MsgEnterRoom();
        msgEnterRoom.roomID = roomId;
        NetManager.Send(msgEnterRoom);
    }
}
