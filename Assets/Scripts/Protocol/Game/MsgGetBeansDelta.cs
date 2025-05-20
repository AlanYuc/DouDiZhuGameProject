using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgGetBeansDelta : MsgBase
{
    /// <summary>
    /// 获取所有玩家及其欢乐豆变化
    /// </summary>
    public PlayerBeansInfo[] playerBeansInfos = new PlayerBeansInfo[3];

    public MsgGetBeansDelta()
    {
        protocolName = "MsgGetBeansDelta";
    }
}
