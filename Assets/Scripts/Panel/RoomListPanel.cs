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

    public override void OnInit()
    {
        skinPath = "RoomListPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        //寻找组件
        playerIdText = skin.transform.Find("Head/IDValue_Text").GetComponent<Text>();
        beanText = skin.transform.Find("Head/BeanValue_Text").GetComponent<Text>();
        createButton = skin.transform.Find("CreateRoom/Create_Button").GetComponent<Button>();
        refreshButton = skin.transform.Find("CreateRoom/Refresh_Button").GetComponent<Button>();
        content = skin.transform.Find("RoomList/Scroll View/Viewport/Content");
        roomObj = skin.transform.Find("Room").gameObject;

        roomObj.SetActive(false);
        playerIdText.text = GameManager.playerId;

        //添加按钮事件监听
        createButton.onClick.AddListener(OnCreateClick);
        refreshButton.onClick.AddListener(OnRefreshClick);
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
}
