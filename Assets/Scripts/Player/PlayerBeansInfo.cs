using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 玩家欢乐豆的信息
/// </summary>
[Serializable]
public class PlayerBeansInfo
{
    /// <summary>
    /// 玩家id
    /// </summary>
    public string playerId;
    /// <summary>
    /// 该玩家增加或减少的欢乐斗
    /// </summary>
    public int beansDelta;
    /// <summary>
    /// 对局的倍数
    /// </summary>
    public int multiplier = 1;
}