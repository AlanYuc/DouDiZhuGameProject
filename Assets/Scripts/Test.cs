using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MsgTest msgTest = new MsgTest();
        msgTest.id = "123";
        //测试编码
        byte[] bytes = MsgBase.Encode(msgTest);
        Debug.Log(Encoding.UTF8.GetString(bytes));
        //测试解码
        MsgTest msgDecodeTest = MsgBase.Decode("MsgTest", bytes, 0, bytes.Length) as MsgTest;
        Debug.Log(msgDecodeTest.id);
        //测试对协议名字符串的编码和解码
        byte[] bytesName = MsgBase.EncodeProtocolName(msgTest);
        int count;
        string msgProtocolName = MsgBase.DecodeProtocolName(bytesName, 0, out count);
        Debug.Log(msgProtocolName);
        Debug.Log(count);
    }
}

public class MsgTest : MsgBase {
    public string id;
    public MsgTest()
    {
        protocolName = "MsgTest";
    }
}
