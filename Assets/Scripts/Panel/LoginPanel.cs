using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    //LoginPanel���������
    public InputField useridInput;
    public InputField passwordInput;
    public Button loginButton;
    public Button registerButton;

    public override void OnInit()
    {
        //skinPath��·��
        //������Դ����Resources�ļ����£���Ϊ���·��
        skinPath = "LoginPanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        //��Ѱ�����
        useridInput     = skin.transform.Find("Id_Input").GetComponent<InputField>();
        passwordInput   = skin.transform.Find("Pw_Input").GetComponent<InputField>();
        loginButton     = skin.transform.Find("Login_Button").GetComponent<Button>();
        registerButton  = skin.transform.Find("Register_Button").GetComponent<Button>();

        //��̬�󶨣���Ӱ�ť�¼�����
        loginButton.onClick.AddListener(OnLoginClcik);
        registerButton.onClick.AddListener(OnRegisterClick);

        //����Э����������շ���˷��صĵ�¼��ע���¼��Ľ��
        NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
    }

    public override void OnClose()
    {
        
    }

    public void OnLoginClcik()
    {
        //�ж������id�Ƿ�Ϸ���������������ʽ������6λ���ȵȵ�����
        //�����Ϊֻ�ж��Ƿ�����Ϊ��
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
        //��ע�����
    }

    public void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msgLogin = msgBase as MsgLogin;
        if (msgLogin.result)
        {
            //��¼�ɹ�
        }
        else
        {
            //��¼ʧ��
        }
    }
}
