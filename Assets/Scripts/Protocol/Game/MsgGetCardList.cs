using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgGetCardList : MsgBase
{
    /// <summary>
    /// 玩家的手牌
    /// </summary>
    public CardInfo[] cardInfos = new CardInfo[CardManager.maxHandSize];
    /// <summary>
    /// 地主的三张底牌
    /// </summary>
    public CardInfo[] threeCards = new CardInfo[3];

    public MsgGetCardList()
    {
        protocolName = "MsgGetCardList";
    }
}