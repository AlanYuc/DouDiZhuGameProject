using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Test : MonoBehaviour
{
    MsgTest msgTest = new MsgTest();
    // Start is called before the first frame update
    void Start()
    {
        ////�ͻ��˱���������
        //MsgTest msgTest = new MsgTest();
        //msgTest.id = "123";
        ////���Ա���
        //byte[] bytes = MsgBase.Encode(msgTest);
        //Debug.Log(Encoding.UTF8.GetString(bytes));
        ////���Խ���
        //MsgTest msgDecodeTest = MsgBase.Decode("MsgTest", bytes, 0, bytes.Length) as MsgTest;
        //Debug.Log(msgDecodeTest.id);
        ////���Զ�Э�����ַ����ı���ͽ���
        //byte[] bytesName = MsgBase.EncodeProtocolName(msgTest);
        //int count;
        //string msgProtocolName = MsgBase.DecodeProtocolName(bytesName, 0, out count);
        //Debug.Log(msgProtocolName);
        //Debug.Log(count);

        //����˲���
        //NetManager.Connect("127.0.0.1", 8888);

        //����TipPanel��Ч��
        //PanelManager.Init();
        //PanelManager.Open<TipPanel>("hello");
    }

    private void Update()
    {
        NetManager.Update();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            NetManager.Send(msgTest);
        }
    }
}

public class MsgTest : MsgBase {
    public string id;
    public MsgTest()
    {
        protocolName = "MsgTest";
    }
}
