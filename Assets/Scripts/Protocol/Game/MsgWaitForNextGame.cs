using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgWaitForNextGame : MsgBase
{
    public bool isWait;
    public bool result = false;

    public MsgWaitForNextGame()
    {
        protocolName = "MsgWaitForNextGame";
    }
}
