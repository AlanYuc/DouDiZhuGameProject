using System;
using System.Collections;
using System.Collections.Generic;
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
    private static bool isConnecting;
    /// <summary>
    /// 是否正在关闭
    /// </summary>
    private static bool isClosing;
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
            Socket s = ar.AsyncState as Socket;
            s.EndConnect(ar);
            Debug.Log("连接成功");
            isConnecting = false;

            FireEvent(NetEvent.ConnectSuccess, "");
            //接收
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
    }
}
