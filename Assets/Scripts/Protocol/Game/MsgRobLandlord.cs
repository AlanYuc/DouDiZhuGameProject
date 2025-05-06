using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgRobLandlord : MsgBase
{
    /// <summary>
    /// 抢地主的客户端玩家id
    /// </summary>
    public string id = "";
    /// <summary>
    /// 地主的id
    /// </summary>
    public string landLordId = "";
    /// <summary>
    /// 客户端给的参数，告诉服务器是抢还是不抢
    /// </summary>
    public bool isRob;
    /// <summary>
    /// 下一家有没有资格抢地主。在服务端更新。
    /// 比如第一轮不叫，第二轮就没有资格抢地主
    /// </summary>
    public bool isNeedRob = true;

    public MsgRobLandlord()
    {
        protocolName = "MsgRobLandlord";
    }
}
