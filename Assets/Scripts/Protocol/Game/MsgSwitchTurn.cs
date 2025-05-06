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
    /// <summary>
    /// 一个回合内轮到后面的第几家。默认round=1，一次轮换一家。
    /// 比如，第一个不叫，第二个叫，第三个抢，那么下一回合会跳过不叫的第一个玩家，到第二个玩家进行抢地主，此时roung=2。
    /// </summary>
    public int round = 1;

    public MsgSwitchTurn()
    {
        protocolName = "MsgSwitchTurn";
    }
}
