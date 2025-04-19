using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public static class NetManager
{
    /// <summary>
    /// 客户端的socket
    /// </summary>
    private static Socket socket;
    /// <summary>
    /// 字节数组
    /// </summary>
    private static ByteArray byteArray;
    /// <summary>
    /// 消息的写入队列
    /// </summary>
    private static Queue<ByteArray> messageQueue;
    /// <summary>
    /// 是否正在连接
    /// </summary>
    public static bool isConnecting;
    /// <summary>
    /// 是否正在关闭
    /// </summary>
    private static bool isClosing;
    /// <summary>
    /// 消息列表
    /// </summary>
    private static List<MsgBase> msgList;
    /// <summary>
    /// 消息列表长度
    /// </summary>
    private static int msgCuont;
    /// <summary>
    /// 一帧处理消息的最大长度
    /// </summary>
    private static int processMsgCount;
    /// <summary>
    /// 是否启用心跳机制
    /// </summary>
    public static bool isUsePingPong;
    /// <summary>
    /// 心跳时间间隔
    /// </summary>
    public static int pingInterval;
    /// <summary>
    /// 上一次发送Ping协议的时间
    /// </summary>
    private static float lastPingTime;
    /// <summary>
    /// 上一次收到Pong协议的时间
    /// </summary>
    private static float lastPongTime;
    /// <summary>
    ///连接状态
    /// </summary>
    public enum NetEvent
    {
        ConnectSuccess = 1,
        ConnectFailed = 2,
        Close = 3,
    }
    /// <summary>
    /// 事件委托
    /// </summary>
    /// <param name="err"></param>
    public delegate void EventListener(string err);
    /// <summary>
    /// 事件监听列表
    /// </summary>
    public static Dictionary<NetEvent , EventListener> eventListenerList = new Dictionary<NetEvent , EventListener>();

    /// <summary>
    /// 添加事件
    /// </summary>
    /// <param name="netEvent"></param>
    /// <param name="eventListener"></param>
    public static void AddEventListener(NetEvent netEvent, EventListener eventListener)
    {
        if (eventListenerList.ContainsKey(netEvent))
        {
            eventListenerList[netEvent] += eventListener;
        }
        else
        {
            eventListenerList[netEvent] = eventListener;
        }
    }
    /// <summary>
    /// 删除事件
    /// </summary>
    /// <param name="netEvent"></param>
    /// <param name="eventListener"></param>
    public static void RemoveEventListener(NetEvent netEvent, EventListener eventListener)
    {
        if (eventListenerList.ContainsKey(netEvent))
        {
            eventListenerList[netEvent] -= eventListener;
            if (eventListenerList[netEvent] == null)
            {
                eventListenerList.Remove(netEvent);
            }
        }
    }
    /// <summary>
    /// 分发事件
    /// </summary>
    /// <param name="netEvent"></param>
    /// <param name="err"></param>
    private static void FireEvent(NetEvent netEvent, string err)
    {
        if (eventListenerList.ContainsKey(netEvent))
        {
            //eventListenerList[netEvent].Invoke(err);
            eventListenerList[netEvent](err);
        }
    }

    /// <summary>
    /// 消息事件委托
    /// </summary>
    /// <param name="msgBase"></param>
    public delegate void MsgListener(MsgBase msgBase);
    /// <summary>
    /// 消息事件列表
    /// </summary>
    public static Dictionary<string,MsgListener> msgListeners = new Dictionary<string, MsgListener>();
    /// <summary>
    /// 添加消息事件监听
    /// </summary>
    /// <param name="msgName"></param>
    /// <param name="msgListener"></param>
    public static void AddMsgListener(string msgName, MsgListener msgListener)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] += msgListener;
        }
        else
        {
            msgListeners[msgName] = msgListener;
        }
    }
    /// <summary>
    /// 移除消息事件
    /// </summary>
    /// <param name="msgName"></param>
    /// <param name="msgListener"></param>
    public static void RemoveMsgListener(string msgName, MsgListener msgListener)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName] -= msgListener;
            if (msgListeners[msgName] == null)
            {
                msgListeners.Remove(msgName);
            }
        }
    }
    /// <summary>
    /// 分发消息事件
    /// </summary>
    /// <param name="msgName"></param>
    /// <param name="msgBase"></param>
    public static void FireMsg(string msgName, MsgBase msgBase)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName](msgBase);
        }
    }

    /// <summary>
    /// 连接
    /// </summary>
    /// <param name="ip">IP地址</param>
    /// <param name="port">端口号</param>
    public static void Connect(string ip, int port)
    {
        if(socket!=null && socket.Connected)
        {
            Debug.Log("连接失败，已经连接了");
            return;
        }

        if (isConnecting)
        {
            Debug.Log("连接失败，正在连接中");
            return;
        }

        Init();
        isConnecting = true;
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        socket.BeginConnect(ipPoint, ConnectCallBack, socket);
    }

    /// <summary>
    /// 连接回调
    /// </summary>
    /// <param name="ar"></param>
    private static void ConnectCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            socket.EndConnect(ar);
            Debug.Log("连接成功");
            isConnecting = false;

            FireEvent(NetEvent.ConnectSuccess, "");
            //接收
            socket.BeginReceive(byteArray.bytes, byteArray.writeIndex, byteArray.Remain, SocketFlags.None, ReceiveCallBack, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("连接失败：" + ex.Message);
            FireEvent(NetEvent.ConnectFailed, ex.Message);
            isConnecting = false;
        }
    }
    /// <summary>
    /// 关闭
    /// </summary>
    public static void Close()
    {
        if(socket == null || !socket.Connected)
        {
            //socket没有对象，或者socket已经断开连接，就不需要关闭
            return;
        }
        if (isConnecting)
        {
            //socket正在连接中，不能关闭
            return;
        }
        if (messageQueue.Count > 0)
        {
            //消息队列中还有未发送的消息，不能关闭
            isClosing = true;
        }
        else
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
            FireEvent(NetEvent.Close, "");
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private static void Init()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        byteArray = new ByteArray();
        messageQueue = new Queue<ByteArray>();
        isConnecting = false;
        isClosing = false;

        msgList = new List<MsgBase>();
        msgCuont = 0;
        processMsgCount = 10;

        isUsePingPong = true;
        pingInterval = 30;
        lastPingTime = Time.time;
        lastPongTime = Time.time;

        if (!msgListeners.ContainsKey("MsgPong"))
        {
            msgListeners.Add("MsgPong", OnMsgPong);
        }
    }
    /// <summary>
    /// 发送消息
    /// </summary>
    /// <param name="msg">消息</param>
    public static void Send(MsgBase msg)
    {
        if (socket == null || !socket.Connected)
        {
            return;
        }
        if (isConnecting)
        {
            return;
        }
        if (isClosing)
        {
            return;
        }

        //编码
        byte[] nameBytes = MsgBase.EncodeProtocolName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        //在消息前加上一个int的大小，用于分包粘包的处理
        int indexNum = sizeof(int) + nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[indexNum];
        int index = 0;
        BitConverter.GetBytes(nameBytes.Length + bodyBytes.Length).CopyTo(sendBytes, index);
        index += sizeof(int);
        nameBytes.CopyTo(sendBytes, index);
        index += nameBytes.Length;
        bodyBytes.CopyTo(sendBytes, index);
        index += bodyBytes.Length;

        //将编码后的消息字节放进ByteArray和messageQueue消息队列中
        ByteArray byteArray = new ByteArray(sendBytes);
        //回调函数是新开的线程，需要锁住消息队列，防止同时访问
        int count = 0;
        lock (messageQueue)
        {
            messageQueue.Enqueue(byteArray);
            count = messageQueue.Count;
        }
        if (count == 1)
        {
            //发送消息
            socket.BeginSend(sendBytes, 0, sendBytes.Length, SocketFlags.None, SendCallBack, socket);
            Debug.Log("消息发送成功");
        }
    }
    /// <summary>
    /// 发送回调函数
    /// </summary>
    /// <param name="ar"></param>
    public static void SendCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            if(socket == null || !socket.Connected)
            {
                return;
            }
            //EndSend返回发送了多少字节
            int count = socket.EndSend(ar);
            
            //ByteArray ba;要在lock外面创建，不然会出问题
            ByteArray ba;
            lock (messageQueue)
            {
                ba = messageQueue.Peek();
            }
            //看当前的消息发送了多少，不够，就还的接着发
            ba.readIndex += count;
            if (ba.Length == 0)
            {
                lock (messageQueue)
                {
                    //消息全部发完了,把消息移除
                    messageQueue.Dequeue();
                    //判断下一个消息
                    ba = messageQueue.Peek();
                }
            }
            //继续发送
            if (ba != null)
            {
                socket.BeginSend(ba.bytes, ba.readIndex, ba.Length, SocketFlags.None, SendCallBack, socket);
            }
            else if (isClosing)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
        catch (SocketException ex)
        {
            Debug.Log("发送失败：" + ex.Message);
        }
    }
    /// <summary>
    /// 接收回调函数
    /// </summary>
    /// <param name="ar"></param>
    public static void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            int count = socket.EndReceive(ar);
            //没有信息可接收了，断开连接
            if(count == 0)
            {
                Close();
                return;
            }
            //成功接收
            byteArray.writeIndex += count;
            OnReceiveData();
            //byteArray长度过小，需要扩容
            if (byteArray.Remain < 8)
            {
                byteArray.MoveBytes();
                byteArray.Resize(byteArray.capacity * 2);
            }
            //继续接收消息
            socket.BeginReceive(byteArray.bytes, byteArray.writeIndex, byteArray.Remain, SocketFlags.None, ReceiveCallBack, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("接收失败：" + ex.Message);
        }
    }
    /// <summary>
    /// 处理接收来的消息
    /// </summary>
    public static void OnReceiveData()
    {
        if (byteArray.Length < sizeof(int))
        {
            //读取的数据长度小于4，也就是连一个起始的int数据（分包粘包）都读不出来
            return;
        }
        int readIndex = byteArray.readIndex;

        //解析第一个分包粘包的数据，也就是数据的完整长度
        int bodyLength = BitConverter.ToInt32(byteArray.bytes, readIndex);
        if(byteArray.Length < sizeof(int) + bodyLength)
        {
            //出现分包情况，还有数据没读进来
            return;
        }

        //解析消息名
        readIndex += sizeof(int);
        int nameCount;
        string protocolName = MsgBase.DecodeProtocolName(byteArray.bytes, readIndex, out nameCount);
        readIndex += nameCount;
        if(protocolName == "")
        {
            Debug.Log("解析消息名失败");
            return;
        }

        //解析消息体
        int bodyCount = bodyLength - nameCount;
        MsgBase msgBase = MsgBase.Decode(protocolName, byteArray.bytes, readIndex, bodyCount);
        readIndex += bodyCount;
        byteArray.readIndex = readIndex;
        byteArray.MoveBytes();
        lock (msgList)
        {
            msgList.Add(msgBase);
        }
        msgCuont++;
        if (byteArray.Length > sizeof(int))
        {
            OnReceiveData();
        }
    }
    /// <summary>
    /// 消息处理，把消息放大msgList中处理，后续放到Update中执行
    /// </summary>
    public static void MsgUpdate()
    {
        if(msgCuont == 0)
        {
            //没有消息
            return;
        }
        for(int i = 0; i < processMsgCount; i++)
        {
            MsgBase msgBase = null;
            lock (msgList)
            {
                if (msgCuont > 0)
                {
                    //取出第一个消息
                    msgBase = msgList[0];
                    msgList.RemoveAt(0);
                    msgCuont--;
                }
            }
            if (msgBase != null)
            {
                FireMsg(msgBase.protocolName, msgBase);
            }
            else
            {
                break;
            }
        }
    }
    /// <summary>
    /// 发送Ping协议
    /// </summary>
    private static void PingUpdate()
    {
        if (!isUsePingPong)
        {
            //没启动心跳机制，直接返回
            return;
        }
        //发送协议
        if (Time.time - lastPingTime > pingInterval)
        {
            MsgPing msgPing = new MsgPing();
            Send(msgPing);
            lastPingTime = Time.time;
        }

        //在pingInterval * 4的固定时间内接收不到Pong协议，就断开
        if(Time.time - lastPongTime> pingInterval * 4)
        {
            Debug.Log("PingUpdate: 心跳时间过长，客户端主动断开连接");
            Close();
        }
    }
    /// <summary>
    /// 需要在在脚本中的Update中调用，确保每帧执行
    /// </summary>
    public static void Update()
    {
        MsgUpdate();
        PingUpdate();
    }
    /// <summary>
    /// 接收到Pong消息后
    /// </summary>
    /// <param name="msgBase"></param>
    public static void OnMsgPong(MsgBase msgBase)
    {
        lastPongTime = Time.time;
    }
}
