using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 切换玩家的回合
/// </summary>
public class MsgSwitchTurn : MsgBase
{
    /// <summary>
    /// 返回下一个玩家的id
    /// </summary>
    public string nextPlayerId;

    public MsgSwitchTurn()
    {
        protocolName = "MsgSwitchTurn";
    }
}
