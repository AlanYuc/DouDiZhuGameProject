using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 玩家下线的消息
/// </summary>
public class MsgKick : MsgBase
{
    /// <summary>
    /// 是否踢下线
    /// </summary>
    public bool isKick = true;

    public MsgKick()
    {
        protocolName = "MsgKick";
    }
}