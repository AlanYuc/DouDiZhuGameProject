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
        //寻找组件
        startButton     = skin.transform.Find("Join/Start_Button").GetComponent<Button>();
        prepareButton   = skin.transform.Find("Join/Prepare_Button").GetComponent<Button>();
        exitButton      = skin.transform.Find("Join/Exit_Button").GetComponent<Button>();
        content         = skin.transform.Find("PlayerList/Scroll View/Viewport/Content");
        playerObj       = skin.transform.Find("Player").gameObject;

        playerObj.SetActive(false);

        //添加按钮监听事件
        startButton.onClick.AddListener(OnStartClick);
        prepareButton.onClick.AddListener(OnPrepareClick);
        exitButton.onClick.AddListener(OnExitClick);

        //添加协议监听
        NetManager.AddMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.AddMsgListener("MsgPrepare", OnMsgPrepare);

        //进入房间时先获取房间信息
        //服务端返回PlayerInfo[]的玩家信息
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
    /// 先清空房间内的所有玩家，再根据返回的msgGetRoomInfo.players.Length生成对应的所有玩家
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
        //实例化玩家对象
        GameObject go = Instantiate(playerObj);
        go.transform.SetParent(content);
        go.SetActive(true);
        go.transform.localScale = Vector3.one;

        //寻找玩家对象内的组件
        Transform playerTrans   = go.transform;
        Text playerIdText       = playerTrans.Find("IDValue_Text").GetComponent<Text>();
        Text beanText           = playerTrans.Find("BeanValue_Text").GetComponent<Text>();
        Text isPrepareText      = playerTrans.Find("StatusValue_Text").GetComponent<Text>();
        Image hostImg           = playerTrans.Find("Host_Img").GetComponent<Image>();

        //更新玩家数据
        playerIdText.text = playerInfo.playerID;
        beanText.text = playerInfo.bean.ToString();

        //更新玩家准备状态
        if (playerInfo.isPrepare)
        {
            isPrepareText.text = "已准备";
        }
        else
        {
            isPrepareText.text = "未准备";
        }

        //根据玩家是否是房主来显示相应的信息
        if (playerInfo.isHost)
        {
            hostImg.gameObject.SetActive(true);
        }
        else
        {
            hostImg.gameObject.SetActive(false);
        }

        //如果当前玩家是房主，则显示对应的按钮
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

        //判断是否退出房间成功
        if (msgLeaveRoom.result)
        {
            PanelManager.Open<TipPanel>("退出房间");
            //打开房间列表面板
            PanelManager.Open<RoomListPanel>();
            //关闭当前的房间面板
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("退出房间失败");
        }
    }

    public void OnMsgPrepare(MsgBase msgBase)
    {
        MsgPrepare msgPrepare = msgBase as MsgPrepare;
        if(msgPrepare.isPrepare == false)
        {
            return;
        }

        //向服务端发送消息，重新获取当前房间内的所有玩家
        MsgGetRoomInfo msgGetRoomInfo = new MsgGetRoomInfo();
        NetManager.Send(msgGetRoomInfo);


    }
}
