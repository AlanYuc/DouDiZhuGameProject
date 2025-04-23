using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgRegister : MsgBase
{
    public string id = "";
    public string pw = "";
    /// <summary>
    /// 注册是否成功
    /// </summary>
    public bool result = true;

    public MsgRegister()
    {
        protocolName = "MsgRegister";
    }
}