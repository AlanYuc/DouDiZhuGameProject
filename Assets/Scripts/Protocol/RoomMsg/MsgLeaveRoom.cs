using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgLeaveRoom : MsgBase
{
    public bool result;

    public MsgLeaveRoom()
    {
        protocolName = "MsgLeaveRoom";
    }
}
