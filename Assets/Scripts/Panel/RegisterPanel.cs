using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    public InputField useridInput;
    public InputField passwordInput;
    public InputField repeatPasswordInput;
    public Button registerButton;
    public Button closeButton;

    public override void OnInit()
    {
        skinPath = "RegisterPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        useridInput         = skin.transform.Find("Id_Input").GetComponent<InputField>();
        passwordInput       = skin.transform.Find("Pw_Input").GetComponent<InputField>();
        repeatPasswordInput = skin.transform.Find("RepPw_Input").GetComponent<InputField>();
        registerButton      = skin.transform.Find("Register_Button").GetComponent<Button>();
        closeButton         = skin.transform.Find("Close_Button").GetComponent<Button>();

        registerButton.onClick.AddListener(OnRegisterClick);
        closeButton.onClick.AddListener(OnCloseClick);

        NetManager.AddMsgListener("MsgRegister", OnMsgRegister);
    }

    public override void OnClose()
    {
        
    }

    public void OnRegisterClick()
    {
        if(useridInput.text == "" || passwordInput.text == "" || repeatPasswordInput.text == "")
        {
            Debug.Log("RegisterPanel.OnRegisterClick() : 输入不能为空");
            return;
        }

        if(passwordInput.text != repeatPasswordInput.text)
        {
            Debug.Log("RegisterPanel.OnRegisterClick() : 重复输入的密码错误");
            return;
        }

        //发送注册协议
        MsgRegister msgRegister = new MsgRegister();
        msgRegister.id = useridInput.text;
        msgRegister.pw = passwordInput.text;
        NetManager.Send(msgRegister);
    }

    public void OnCloseClick()
    {
        Close();
    }

    public void OnMsgRegister(MsgBase msgBase)
    {
        MsgRegister msgRegister = msgBase as MsgRegister;
        if (msgRegister.result)
        {
            //注册成功
            Close();
        }
        else
        {
            //注册失败
        }
    }
}
