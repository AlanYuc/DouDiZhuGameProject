using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgGetCardList : MsgBase
{
    public CardInfo[] cardInfos = new CardInfo[CardManager.maxHandSize];

    public MsgGetCardList()
    {
        protocolName = "MsgGetCardList";
    }
}