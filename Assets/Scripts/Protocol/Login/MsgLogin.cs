using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgLogin : MsgBase
{
    //客户端和服务器端的字段必须保持一致，这样才能成功解析出来数据
    public string id;
    public string pw;
    /// <summary>
    /// 服务端返回 账号密码是否正确
    /// </summary>
    public bool result;

    public MsgLogin()
    {
        protocolName = "MsgLogin";
    }
}
