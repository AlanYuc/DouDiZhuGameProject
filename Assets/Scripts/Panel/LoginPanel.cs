using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    //LoginPanel包含的组件
    public InputField useridInput;
    public InputField passwordInput;
    public Button loginButton;
    public Button registerButton;

    public override void OnInit()
    {
        //skinPath的路径
        //所有资源都在Resources文件夹下，且为相对路径
        skinPath = "LoginPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        //先寻找组件
        useridInput     = skin.transform.Find("Id_Input").GetComponent<InputField>();
        passwordInput   = skin.transform.Find("Pw_Input").GetComponent<InputField>();
        loginButton     = skin.transform.Find("Login_Button").GetComponent<Button>();
        registerButton  = skin.transform.Find("Register_Button").GetComponent<Button>();

        //动态绑定，添加按钮事件监听
        loginButton.onClick.AddListener(OnLoginClcik);
        registerButton.onClick.AddListener(OnRegisterClick);

        NetManager.AddEventListener(NetManager.NetEvent.ConnectSuccess, OnConnectSuccess);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFailed, OnConnectFail);
        //网络协议监听，接收服务端发回的登录、注册事件的结果
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        NetManager.Connect("127.0.0.1", 8888);
    }

    public override void OnClose()
    {
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSuccess, OnConnectSuccess);
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectFailed, OnConnectFail);
        NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);
    }

    public void OnLoginClcik()
    {
        //判断输入的id是否合法，可以用正则表达式，加上6位数等等的限制
        //这里简化为只判断是否输入为空
        if(useridInput.text == "" || passwordInput.text == "")
        {
            Debug.Log("LoginPanel.OnLoginClcik() : 输入不能为空");
            PanelManager.Open<TipPanel>("用户名和密码不能为空");
            return;
        }

        //发送登录协议
        MsgLogin msgLogin = new MsgLogin();
        msgLogin.id = useridInput.text;
        msgLogin.pw = passwordInput.text;
        NetManager.Send(msgLogin);
    }

    public void OnRegisterClick()
    {
        //to do...
        //打开注册面板
        PanelManager.Open<RegisterPanel>();
    }

    public void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msgLogin = msgBase as MsgLogin;
        if (msgLogin.result)
        {
            //登录成功
            PanelManager.Open<TipPanel>("登录成功");
            PanelManager.Open<RoomListPanel>();
            Close();
        }
        else
        {
            //登录失败
            PanelManager.Open<TipPanel>("登录失败");
        }
    }

    public void OnConnectSuccess(string err)
    {
        Debug.Log("LoginPanel.OnConnectSuccess() : 连接成功");
    }
    public void OnConnectFail(string err)
    {
        Debug.Log("LoginPanel.OnConnectFail() : 连接失败");
        PanelManager.Open<TipPanel>("连接失败 : " + err);
    }
}
