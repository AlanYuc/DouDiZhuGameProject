using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgGetRoomInfo : MsgBase
{
    public PlayerInfo[] players;

    public MsgGetRoomInfo()
    {
        protocolName = "MsgGetRoomInfo";
    }
}
