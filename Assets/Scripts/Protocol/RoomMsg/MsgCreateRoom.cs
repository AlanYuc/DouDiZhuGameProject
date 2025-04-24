using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgCreateRoom : MsgBase
{
    /// <summary>
    /// 是否创建成功
    /// </summary>
    public bool result;

    public MsgCreateRoom()
    {
        protocolName = "MsgCreateRoom";
    }
}