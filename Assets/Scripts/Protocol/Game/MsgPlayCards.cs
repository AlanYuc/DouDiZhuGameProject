using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgPlayCards : MsgBase
{
    public string id;
    /// <summary>
    /// 是否出牌
    /// </summary>
    public bool isPlay;
    /// <summary>
    /// 准备出的牌
    /// </summary>
    public CardInfo[] cardInfos = new CardInfo[20];
    /// <summary>
    /// 出牌的类型
    /// </summary>
    public int cardType;
    /// <summary>
    /// 是否处理完成
    /// </summary>
    public bool result;
    /// <summary>
    /// 是否允许 不出牌 的行为。默认是允许。
    /// </summary>
    public bool canNotPlay = true;

    public MsgPlayCards()
    {
        protocolName = "MsgPlayCards";
    }
}