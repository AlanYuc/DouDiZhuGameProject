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

        NetManager.AddEventListener(NetManager.NetEvent.ConnectSuccess, OnConnectSuccess);
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFailed, OnConnectFail);
        //����Э����������շ���˷��صĵ�¼��ע���¼��Ľ��
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
        //�ж������id�Ƿ�Ϸ���������������ʽ������6λ���ȵȵ�����
        //�����Ϊֻ�ж��Ƿ�����Ϊ��
        if(useridInput.text == "" || passwordInput.text == "")
        {
            Debug.Log("LoginPanel.OnLoginClcik() : ���벻��Ϊ��");
            PanelManager.Open<TipPanel>("�û��������벻��Ϊ��");
            return;
        }

        //���͵�¼Э��
        MsgLogin msgLogin = new MsgLogin();
        msgLogin.id = useridInput.text;
        msgLogin.pw = passwordInput.text;
        NetManager.Send(msgLogin);
    }

    public void OnRegisterClick()
    {
        //to do...
        //��ע�����
        PanelManager.Open<RegisterPanel>();
    }

    public void OnMsgLogin(MsgBase msgBase)
    {
        MsgLogin msgLogin = msgBase as MsgLogin;
        if (msgLogin.result)
        {
            //��¼�ɹ�
            PanelManager.Open<TipPanel>("��¼�ɹ�");
            PanelManager.Open<RoomListPanel>();
            Close();
        }
        else
        {
            //��¼ʧ��
            PanelManager.Open<TipPanel>("��¼ʧ��");
        }
    }

    public void OnConnectSuccess(string err)
    {
        Debug.Log("LoginPanel.OnConnectSuccess() : ���ӳɹ�");
    }
    public void OnConnectFail(string err)
    {
        Debug.Log("LoginPanel.OnConnectFail() : ����ʧ��");
        PanelManager.Open<TipPanel>("����ʧ�� : " + err);
    }
}
