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
    /// 对局结果，1表示农民获胜，2表示地主获胜
    /// </summary>
    private int winResult;

    /// <summary>
    /// 储存结算界面每个玩家的信息对象的Transform
    /// </summary>
    private Dictionary<string , Transform> playersDic = new Dictionary<string , Transform>();
    /// <summary>
    /// 储存结算界面每个玩家的欢乐豆变化数据
    /// </summary>
    private Dictionary<string , int> playerBeansDelta = new Dictionary<string , int>();
    /// <summary>
    /// 管理音效
    /// </summary>
    private AudioSource audioSource;


    public override void OnInit()
    {
        skinPath = "ScorePanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        //寻找组件
        farmersWinImgTrans      = skin.transform.Find("Title_Img/FarmersWin");
        farmersLoseImgTrans     = skin.transform.Find("Title_Img/FarmersLose");
        landlordWinImgTrans     = skin.transform.Find("Title_Img/LandlordWin");
        landlordLoseImgTrans    = skin.transform.Find("Title_Img/LandlordLose");
        player1Trans            = skin.transform.Find("GameResult/Player1");
        player2Trans            = skin.transform.Find("GameResult/Player2");
        player3Trans            = skin.transform.Find("GameResult/Player3");
        exitBtn                 = skin.transform.Find("Exit_Btn").GetComponent<Button>();
        restartBtn              = skin.transform.Find("Restart_Btn").GetComponent<Button>();
        audioSource             = skin.transform.Find("Audio Source").GetComponent<AudioSource>();

        //将玩家和结算面板玩家信息部分绑定
        playersDic.Clear();
        playersDic.Add(GameManager.playerId, player1Trans);
        playersDic.Add(GameManager.leftPlayerId, player2Trans);
        playersDic.Add(GameManager.rightPlayerId, player3Trans);

        playerBeansDelta.Clear();

        if (para.Length >= 1)
        {
            winResult = (int)para[0];
        }

        //添加按钮事件
        exitBtn.onClick.AddListener(OnExitButtonClick);
        restartBtn.onClick.AddListener(OnRestartButtonClick);

        //TO DO... 先关闭，后面再说
        restartBtn.enabled = false;

        //先隐藏标题信息，根据玩家的胜负决定显示的内容
        farmersWinImgTrans.gameObject.SetActive(false);
        farmersLoseImgTrans.gameObject.SetActive(false);
        landlordWinImgTrans.gameObject.SetActive(false);
        landlordLoseImgTrans.gameObject.SetActive(false);

        //网络协议更新
        NetManager.AddMsgListener("MsgGetBeansDelta", OnMsgGetBeansDelta);

        //获取玩家欢乐豆信息
        MsgGetBeansDelta msgGetBeansDelta = new MsgGetBeansDelta();
        NetManager.Send(msgGetBeansDelta);
    }

    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgGetPlayersInfo", OnMsgGetBeansDelta);
    }

    public void ShowPlayersInfo(int multiplier)
    {
        //显示结算面板数据
        foreach(string id in playersDic.Keys)
        {
            Text idText = playersDic[id].Find("ID_Text").GetComponent<Text>();
            Text multiplierText = playersDic[id].Find("Multiplier_Text").GetComponent<Text>();
            Text beanText = playersDic[id].Find("Bean_Text").GetComponent<Text>();

            idText.text = id;
            multiplierText.text = multiplier.ToString();
            beanText.text = playerBeansDelta[id].ToString();
        }

        string audioPath = "Sounds/";

        //显示上方的获胜的图片
        if (winResult == 2)
        {
            //地主获胜
            if (GameManager.isLandLord)
            {
                farmersWinImgTrans.gameObject.SetActive(false);
                farmersLoseImgTrans.gameObject.SetActive(false);
                landlordWinImgTrans.gameObject.SetActive(true);
                landlordLoseImgTrans.gameObject.SetActive(false);

                audioPath = audioPath + "MusicEx_Win";
            }
            else
            {
                farmersWinImgTrans.gameObject.SetActive(false);
                farmersLoseImgTrans.gameObject.SetActive(true);
                landlordWinImgTrans.gameObject.SetActive(false);
                landlordLoseImgTrans.gameObject.SetActive(false);

                audioPath = audioPath + "MusicEx_Lose";
            }
        }
        else
        {
            //winResult == 1 农民获胜
            if (GameManager.isLandLord)
            {
                farmersWinImgTrans.gameObject.SetActive(false);
                farmersLoseImgTrans.gameObject.SetActive(false);
                landlordWinImgTrans.gameObject.SetActive(false);
                landlordLoseImgTrans.gameObject.SetActive(true);

                audioPath = audioPath + "MusicEx_Lose";
            }
            else
            {
                farmersWinImgTrans.gameObject.SetActive(true);
                farmersLoseImgTrans.gameObject.SetActive(false);
                landlordWinImgTrans.gameObject.SetActive(false);
                landlordLoseImgTrans.gameObject.SetActive(false);

                audioPath = audioPath + "MusicEx_Win";
            }
        }

        audioSource.clip = Resources.Load<AudioClip>(audioPath);
        audioSource.Play();
    }

    public void OnMsgGetBeansDelta(MsgBase msgBase)
    {
        MsgGetBeansDelta msgGetBeansDelta = msgBase as MsgGetBeansDelta;

        //先获取欢乐豆数据
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

        //更新结算面板的信息
        ShowPlayersInfo(msgGetBeansDelta.playerBeansInfos[0].multiplier);
    }

    public void OnExitButtonClick()
    {
        PanelManager.Open<TipPanel>("对局结束，正在返回房间");
        PanelManager.Open<RoomPanel>();
        PanelManager.Close("GamePanel");

        //退出本局游戏后回到房间，需要清空上局游戏玩家的牌。
        GameManager.cards.Clear();
        GameManager.threeCards.Clear();
        GameManager.isLandLord = false;
        GameManager.canNotPlay = false;

        //重置为叫地主的状态，方便下一次游戏
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
