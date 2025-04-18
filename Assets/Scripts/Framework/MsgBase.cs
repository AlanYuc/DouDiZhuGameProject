using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MsgBase
{
    /// <summary>
    /// 不同的协议名
    /// </summary>
    public string protocolName;
    /// <summary>
    /// 将MsgBase类编码为json数据的字节数组
    /// </summary>
    /// <param name="msgBase"></param>
    /// <returns></returns>
    public static byte[] Encode(MsgBase msgBase)
    {
        string s = JsonUtility.ToJson(msgBase);
        return Encoding.UTF8.GetBytes(s);
    }
    /// <summary>
    /// 将传入的消息字节数组解码为MsgBase数据
    /// </summary>
    /// <param name="protocolName">协议名</param>
    /// <param name="bytes">字节数组</param>
    /// <param name="offset">开始解码的起始位置</param>
    /// <param name="length">需要解码的数据长度</param>
    /// <returns></returns>
    public static MsgBase Decode(string protocolName, byte[] bytes, int offset, int count)
    {
        string s = Encoding.UTF8.GetString(bytes, offset, count);
        //return JsonUtility.FromJson<MsgBase>(s);
        //根据协议名将其反序列化为对应的子类，然后转成MsgBase基类。
        return JsonUtility.FromJson(s,Type.GetType(protocolName)) as MsgBase;
    }
    /// <summary>
    /// 编码协议名
    /// </summary>
    /// <param name="msgBase"></param>
    /// <returns></returns>
    public static byte[] EncodeProtocolName(MsgBase msgBase)
    {
        //序列化字符串的时候，需要知道该字符串序列化后的长度，方便反序列化的时候取出。
        int indexNum = sizeof(int) + Encoding.UTF8.GetBytes(msgBase.protocolName).Length;
        byte[] bytes = new byte[indexNum];
        int index = 0;//从bytes数组中的第几个位置去存储数据
        byte[] nameByte = Encoding.UTF8.GetBytes(msgBase.protocolName);
        int nameNum = nameByte.Length;
        BitConverter.GetBytes(nameNum).CopyTo(bytes, index);
        index += sizeof(int);
        nameByte.CopyTo(bytes, index);
        index += nameNum;
        return bytes;
    }
    /// <summary>
    /// 解码协议名
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string DecodeProtocolName(byte[] bytes, int offset, out int count)
    {
        count = BitConverter.ToInt32(bytes, offset);
        offset += sizeof(int);
        string s = Encoding.UTF8.GetString(bytes, offset, count);
        //count不仅要返回消息名的长度，还要返回之前的int的长度，作为总的数据长度
        count += sizeof(int);
        return s;
    }
}
