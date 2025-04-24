using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgEnterRoom : MsgBase
{
    /// <summary>
    /// 进入的房间id
    /// </summary>
    public int roomID;
    /// <summary>
    /// 服务端返回结果，是否进入房间成功
    /// </summary>
    public bool result;

    public MsgEnterRoom()
    {
        protocolName = "MsgEnterRoom";
    }
}
