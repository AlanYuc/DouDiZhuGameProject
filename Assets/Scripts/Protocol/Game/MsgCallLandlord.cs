using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgCallLandlord : MsgBase
{
    /// <summary>
    /// 叫地主的玩家id
    /// </summary>
    public string id = "";
    /// <summary>
    /// 是否叫地主
    /// </summary>
    public bool isCall;
    /// <summary>
    /// 服务端返回的叫地主的结果
    /// 0 表示当前玩家不叫地主，剩余玩家继续叫地主
    /// 1 表示当前玩家叫地主，剩余玩家开始抢地主
    /// 2 表示当前玩家不叫地主，所有玩家都不叫地主，重新洗牌
    /// 3 表示当前玩家叫地主，其他玩家都不叫，当前玩家是最后一个玩家并叫了地主，不需要抢地主
    /// </summary>
    public int result;

    public MsgCallLandlord()
    {
        protocolName = "MsgCallLandlord";
    }
}