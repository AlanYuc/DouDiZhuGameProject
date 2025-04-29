using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 获取开始玩家的id，表示从哪个玩家开始进行叫地主的操作
/// </summary>
public class MsgGetStartPlayer : MsgBase
{
    public string playerId = "";

    public MsgGetStartPlayer()
    {
        protocolName = "MsgGetStartPlayer";
    }
}
