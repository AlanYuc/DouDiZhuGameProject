using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgLogin : MsgBase
{
    public string userid;
    public string password;
    /// <summary>
    /// 服务端返回 账号密码是否正确
    /// </summary>
    public bool result;

    public MsgLogin()
    {
        protocolName = "MsgLogin";
    }
}
