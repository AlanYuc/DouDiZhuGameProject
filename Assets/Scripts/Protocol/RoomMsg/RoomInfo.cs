using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class RoomInfo
{
    /// <summary>
    /// 房间号
    /// </summary>
    public int roomID;
    /// <summary>
    /// 房间人数
    /// </summary>
    public int count;
    /// <summary>
    /// 是否准备
    /// </summary>
    public bool isPrepare;
}