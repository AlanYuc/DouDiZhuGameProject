using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScorePanel : BasePanel
{
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


    public override void OnInit()
    {
        skinPath = "ScorePanel";
        layer = PanelManager.Layer.Panel;
    }

    public override void OnShow(params object[] para)
    {
        //寻找组件
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
         * 这里出了点问题，第一次使用没问题，但是第二次打开后，
         * playersDic添加后，在消息回调函数内，所有的transform都变成null了
         * 将字典整个放到GameManager下才解决这个问题，其他的farmersWinImgTrans都是同一个问题
         */

        //playersDic.Clear();
        //playersDic.Add(GameManager.playerId, GameManager.player1ScoreInfo.transform);
        //playersDic.Add(GameManager.leftPlayerId, GameManager.player2ScoreInfo.transform);
        //playersDic.Add(GameManager.rightPlayerId, GameManager.player3ScoreInfo.transform);

        //将玩家和结算面板玩家信息部分绑定
        GameManager.playersDic.Clear();
        GameManager.playersDic.Add(GameManager.playerId, GameManager.player1ScoreInfo.transform);
        GameManager.playersDic.Add(GameManager.leftPlayerId, GameManager.player2ScoreInfo.transform);
        GameManager.playersDic.Add(GameManager.rightPlayerId, GameManager.player3ScoreInfo.transform);

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
        GameManager.farmersWinImgObj.SetActive(false);
        GameManager.farmersLoseImgObj.SetActive(false);
        GameManager.landlordWinImgObj.SetActive(false);
        GameManager.landlordLoseImgObj.SetActive(false);

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

        //显示上方的获胜的图片
        if (winResult == 2)
        {
            //地主获胜
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
            //winResult == 1 农民获胜
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

        if(msgGetBeansDelta.id != GameManager.playerId)
        {
            return;
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
