using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : BasePanel
{
    private Button exitBtn;
    private Button restartBtn;
    /// <summary>
    /// �Ծֽ����1��ʾũ���ʤ��2��ʾ������ʤ
    /// </summary>
    private int winResult;

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
        GameManager.farmersWinImgObj    = skin.transform.Find("Title_Img/FarmersWin").gameObject;
        GameManager.farmersLoseImgObj   = skin.transform.Find("Title_Img/FarmersLose").gameObject;
        GameManager.landlordWinImgObj   = skin.transform.Find("Title_Img/LandlordWin").gameObject;
        GameManager.landlordLoseImgObj  = skin.transform.Find("Title_Img/LandlordLose").gameObject;
        GameManager.player1ScoreInfo    = skin.transform.Find("GameResult/Player1").gameObject;
        GameManager.player2ScoreInfo    = skin.transform.Find("GameResult/Player2").gameObject;
        GameManager.player3ScoreInfo    = skin.transform.Find("GameResult/Player3").gameObject;
        exitBtn                         = skin.transform.Find("Exit_Btn").GetComponent<Button>();
        restartBtn                      = skin.transform.Find("Restart_Btn").GetComponent<Button>();
        GameManager.audioSource         = skin.transform.Find("Audio Source").GetComponent<AudioSource>();


        /*
         * ������˵����⣬��һ��ʹ��û���⣬���ǵڶ��δ򿪺�
         * playersDic��Ӻ�����Ϣ�ص������ڣ����е�transform�����null��
         * ���ֵ������ŵ�GameManager�²Ž��������⣬������farmersWinImgTrans����ͬһ������
         */

        //playersDic.Clear();
        //playersDic.Add(GameManager.playerId, GameManager.player1ScoreInfo.transform);
        //playersDic.Add(GameManager.leftPlayerId, GameManager.player2ScoreInfo.transform);
        //playersDic.Add(GameManager.rightPlayerId, GameManager.player3ScoreInfo.transform);

        //����Һͽ�����������Ϣ���ְ�
        GameManager.playersDic.Clear();
        GameManager.playersDic.Add(GameManager.playerId, GameManager.player1ScoreInfo.transform);
        GameManager.playersDic.Add(GameManager.leftPlayerId, GameManager.player2ScoreInfo.transform);
        GameManager.playersDic.Add(GameManager.rightPlayerId, GameManager.player3ScoreInfo.transform);

        playerBeansDelta.Clear();

        if (para.Length >= 1)
        {
            winResult = (int)para[0];
        }

        //��Ӱ�ť�¼�
        exitBtn.onClick.AddListener(OnExitButtonClick);
        restartBtn.onClick.AddListener(OnRestartButtonClick);

        //TO DO... �ȹرգ�������˵
        restartBtn.enabled = false;

        //�����ر�����Ϣ��������ҵ�ʤ��������ʾ������
        GameManager.farmersWinImgObj.SetActive(false);
        GameManager.farmersLoseImgObj.SetActive(false);
        GameManager.landlordWinImgObj.SetActive(false);
        GameManager.landlordLoseImgObj.SetActive(false);

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
        //��ʾ�����������
        foreach (string id in GameManager.playersDic.Keys)
        {
            Text idText = GameManager.playersDic[id].Find("ID_Text").GetComponent<Text>();
            Text multiplierText = GameManager.playersDic[id].Find("Multiplier_Text").GetComponent<Text>();
            Text beanText = GameManager.playersDic[id].Find("Bean_Text").GetComponent<Text>();

            idText.text = id;
            multiplierText.text = multiplier.ToString();
            beanText.text = playerBeansDelta[id].ToString();
        }

        string audioPath = "Sounds/";

        //��ʾ�Ϸ��Ļ�ʤ��ͼƬ
        if (winResult == 2)
        {
            //������ʤ
            if (GameManager.isLandLord)
            {
                GameManager.farmersWinImgObj.gameObject.SetActive(false);
                GameManager.farmersLoseImgObj.gameObject.SetActive(false);
                GameManager.landlordWinImgObj.gameObject.SetActive(true);
                GameManager.landlordLoseImgObj.gameObject.SetActive(false);

                audioPath = audioPath + "MusicEx_Win";
            }
            else
            {
                GameManager.farmersWinImgObj.gameObject.SetActive(false);
                GameManager.farmersLoseImgObj.gameObject.SetActive(true);
                GameManager.landlordWinImgObj.gameObject.SetActive(false);
                GameManager.landlordLoseImgObj.gameObject.SetActive(false);

                audioPath = audioPath + "MusicEx_Lose";
            }
        }
        else
        {
            //winResult == 1 ũ���ʤ
            if (GameManager.isLandLord)
            {
                GameManager.farmersWinImgObj.gameObject.SetActive(false);
                GameManager.farmersLoseImgObj.gameObject.SetActive(false);
                GameManager.landlordWinImgObj.gameObject.SetActive(false);
                GameManager.landlordLoseImgObj.gameObject.SetActive(true);

                audioPath = audioPath + "MusicEx_Lose";
            }
            else
            {
                GameManager.farmersWinImgObj.gameObject.SetActive(true);
                GameManager.farmersLoseImgObj.gameObject.SetActive(false);
                GameManager.landlordWinImgObj.gameObject.SetActive(false);
                GameManager.landlordLoseImgObj.gameObject.SetActive(false);

                audioPath = audioPath + "MusicEx_Win";
            }
        }

        GameManager.audioSource.clip = Resources.Load<AudioClip>(audioPath);
        GameManager.audioSource.Play();
    }

    public void OnMsgGetBeansDelta(MsgBase msgBase)
    {
        MsgGetBeansDelta msgGetBeansDelta = msgBase as MsgGetBeansDelta;

        //�Ȼ�ȡ���ֶ�����
        for(int i = 0; i < msgGetBeansDelta.playerBeansInfos.Length; i++)
        {
            if (!playerBeansDelta.ContainsKey(msgGetBeansDelta.playerBeansInfos[i].playerId))
            {
                playerBeansDelta.Add(msgGetBeansDelta.playerBeansInfos[i].playerId, msgGetBeansDelta.playerBeansInfos[i].beansDelta);
            }
            else
            {
                playerBeansDelta[msgGetBeansDelta.playerBeansInfos[i].playerId] = msgGetBeansDelta.playerBeansInfos[i].beansDelta;
            }
        }

        if(msgGetBeansDelta.id != GameManager.playerId)
        {
            return;
        }

        //���½���������Ϣ
        ShowPlayersInfo(msgGetBeansDelta.playerBeansInfos[0].multiplier);
    }

    public void OnExitButtonClick()
    {
        PanelManager.Open<TipPanel>("�Ծֽ��������ڷ��ط���");
        PanelManager.Open<RoomPanel>();
        PanelManager.Close("GamePanel");

        //�˳�������Ϸ��ص����䣬��Ҫ����Ͼ���Ϸ��ҵ��ơ�
        GameManager.cards.Clear();
        GameManager.threeCards.Clear();
        GameManager.isLandLord = false;
        GameManager.canNotPlay = false;

        //����Ϊ�е�����״̬��������һ����Ϸ
        GameManager.status = PlayerStatus.Call;
        Close();
    }

    public void OnRestartButtonClick()
    {
        GamePanel gamePanel = transform.GetComponent<GamePanel>();
        gamePanel.WaitForNextGame();

        Close();
    }
}
