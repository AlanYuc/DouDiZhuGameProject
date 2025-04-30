using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 获得同桌的其他玩家
/// </summary>
public class MsgGetOtherPlayers : MsgBase
{
    /// <summary>
    /// 自己的玩家id
    /// </summary>
    public string playerId = "";
    /// <summary>
    /// 坐在同一桌左侧的玩家id
    /// </summary>
    public string leftPlayerId = "";
    /// <summary>
    /// 坐在同一桌右侧的玩家id
    /// </summary>
    public string rightPlayerId = "";

    public MsgGetOtherPlayers()
    {
        protocolName = "MsgGetOtherPlayers";
    }
}
