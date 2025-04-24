using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class PlayerInfo
{
    /// <summary>
    /// 玩家ID
    /// </summary>
    public string playerID;
    /// <summary>
    /// 欢乐斗数量
    /// </summary>
    public int bean;
    /// <summary>
    /// 是否准备
    /// </summary>
    public bool isPrepare;
    /// <summary>
    /// 是否是房主
    /// </summary>
    public bool isHost;
}
