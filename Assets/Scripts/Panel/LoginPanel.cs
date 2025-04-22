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

        //网络协议监听，接收服务端发回的登录、注册事件的结果
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
    }

    public override void OnClose()
    {
        
    }

    public void OnLoginClcik()
    {
        //判断输入的id是否合法，可以用正则表达式，加上6位数等等的限制
        //这里简化为只判断是否输入为空
        if(useridInput.text == "" || passwordInput.text == "")
        {
            return;
        }

        MsgLogin msgLogin = new MsgLogin();
        msgLogin.userid = useridInput.text;
        msgLogin.password = passwordInput.text;
        NetManager.Send(msgLogin);
    }

    public void OnRegisterClick()
    {
        //to do...
        //打开注册面板
    }

    public void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msgLogin = msgBase as MsgLogin;
        if (msgLogin.result)
        {
            //登录成功
        }
        else
        {
            //登录失败
        }
    }
}
