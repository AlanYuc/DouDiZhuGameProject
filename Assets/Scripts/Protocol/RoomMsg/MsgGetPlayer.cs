using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 获取玩家的协议
/// </summary>
public class MsgGetPlayer : MsgBase
{
    public int bean;

    public MsgGetPlayer()
    {
        protocolName = "MsgGetPlayer";
    }
}
