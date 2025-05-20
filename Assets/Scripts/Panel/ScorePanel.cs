using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : BasePanel
{
    private Transform farmersWinImgTrans;
    private Transform farmersLoseImgTrans;
    private Transform landlordWinImgTrans;
    private Transform landlordLoseImgTrans;
    private Transform player1Trans;
    private Transform player2Trans;
    private Transform player3Trans;
    private Button exitBtn;
    private Button restartBtn;

    /// <summary>
    /// ����������ÿ����ҵ���Ϣ�����Transform
    /// </summary>
    private Dictionary<string , Transform> playersDic = new Dictionary<string , Transform>();
    /// <summary>
    /// ����������ÿ����ҵĻ��ֶ��仯����
    /// </summary>
    private Dictionary<string , int> playerBeansDelta = new Dictionary<string , int>();


    public override void OnInit()
    {
        skinPath = "ScorePanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        //Ѱ�����
        farmersWinImgTrans      = skin.transform.Find("Title_Img/FarmersWin");
        farmersLoseImgTrans     = skin.transform.Find("Title_Img/FarmersLose");
        landlordWinImgTrans     = skin.transform.Find("Title_Img/LandlordWin");
        landlordLoseImgTrans    = skin.transform.Find("Title_Img/LandlordLose");
        player1Trans            = skin.transform.Find("GameResult/Player1");
        player2Trans            = skin.transform.Find("GameResult/Player2");
        player3Trans            = skin.transform.Find("GameResult/Player3");
        exitBtn                 = skin.transform.Find("Exit_Btn").GetComponent<Button>();
        restartBtn              = skin.transform.Find("Restart_Btn").GetComponent<Button>();

        //����Һͽ�����������Ϣ���ְ�
        playersDic.Clear();
        playersDic.Add(GameManager.playerId, player1Trans);
        playersDic.Add(GameManager.leftPlayerId, player2Trans);
        playersDic.Add(GameManager.rightPlayerId, player3Trans);

        playerBeansDelta.Clear();

        //�����ر�����Ϣ��������ҵ�ʤ��������ʾ������
        farmersWinImgTrans.gameObject.SetActive(false);
        farmersLoseImgTrans.gameObject.SetActive(false);
        landlordWinImgTrans.gameObject.SetActive(false);
        landlordLoseImgTrans.gameObject.SetActive(false);

        //����Э�����
        NetManager.AddMsgListener("MsgGetBeansDelta", OnMsgGetBeansDelta);

        //��ȡ��һ��ֶ���Ϣ
        MsgGetBeansDelta msgGetBeansDelta = new MsgGetBeansDelta();
        NetManager.Send(msgGetBeansDelta);
    }

    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgGetPlayersInfo", OnMsgGetBeansDelta);
    }

    public void ShowPlayersInfo(int multiplier)
    {
        foreach(string id in playersDic.Keys)
        {
            Text idText = playersDic[id].Find("ID_Text").GetComponent<Text>();
            Text multiplierText = playersDic[id].Find("Multiplier_Text").GetComponent<Text>();
            Text beanText = playersDic[id].Find("Bean_Text").GetComponent<Text>();

            idText.text = id;
            multiplierText.text = multiplier.ToString();
            beanText.text = playerBeansDelta[id].ToString();
        }
    }

    public void OnMsgGetBeansDelta(MsgBase msgBase)
    {
        MsgGetBeansDelta msgGetBeansDelta = msgBase as MsgGetBeansDelta;

        //�Ȼ�ȡ���ֶ�����
        for(int i = 0; i < msgGetBeansDelta.playerBeansInfos.Length; i++)
        {
            playerBeansDelta.Add(msgGetBeansDelta.playerBeansInfos[i].playerId, msgGetBeansDelta.playerBeansInfos[i].beansDelta);
        }

        foreach(string id in playerBeansDelta.Keys)
        {
            Debug.Log("id �� ���� ��" + id + " " + playerBeansDelta[id]);
        }

        //���½���������Ϣ
        ShowPlayersInfo(msgGetBeansDelta.playerBeansInfos[0].multiplier);
    }
}
