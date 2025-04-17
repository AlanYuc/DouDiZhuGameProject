using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public static class NetManager
{
    /// <summary>
    /// �ͻ��˵�socket
    /// </summary>
    private static Socket socket;
    /// <summary>
    /// �ֽ�����
    /// </summary>
    private static ByteArray byteArray;
    /// <summary>
    /// ��Ϣ��д�����
    /// </summary>
    private static Queue<ByteArray> messageQueue;
    /// <summary>
    /// �Ƿ���������
    /// </summary>
    private static bool isConnecting;
    /// <summary>
    /// �Ƿ����ڹر�
    /// </summary>
    private static bool isClosing;
    /// <summary>
    ///����״̬
    /// </summary>
    public enum NetEvent
    {
        ConnectSuccess = 1,
        ConnectFailed = 2,
        Close = 3,
    }
    /// <summary>
    /// �¼�ί��
    /// </summary>
    /// <param name="err"></param>
    public delegate void EventListener(string err);
    /// <summary>
    /// �¼������б�
    /// </summary>
    public static Dictionary<NetEvent , EventListener> eventListenerList = new Dictionary<NetEvent , EventListener>();

    /// <summary>
    /// ����¼�
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
    /// ɾ���¼�
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
    /// �ַ��¼�
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
    /// ����
    /// </summary>
    /// <param name="ip">IP��ַ</param>
    /// <param name="port">�˿ں�</param>
    public static void Connect(string ip, int port)
    {
        if(socket!=null && socket.Connected)
        {
            Debug.Log("����ʧ�ܣ��Ѿ�������");
            return;
        }

        if (isConnecting)
        {
            Debug.Log("����ʧ�ܣ�����������");
            return;
        }

        Init();
        isConnecting = true;
        IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        socket.BeginConnect(ipPoint, ConnectCallBack, socket);
    }

    /// <summary>
    /// ���ӻص�
    /// </summary>
    /// <param name="ar"></param>
    private static void ConnectCallBack(IAsyncResult ar)
    {
        try
        {
            Socket s = ar.AsyncState as Socket;
            s.EndConnect(ar);
            Debug.Log("���ӳɹ�");
            isConnecting = false;

            FireEvent(NetEvent.ConnectSuccess, "");
            //����
        }
        catch (SocketException ex)
        {
            Debug.Log("����ʧ�ܣ�" + ex.Message);
            FireEvent(NetEvent.ConnectFailed, ex.Message);
            isConnecting = false;
        }
    }
    /// <summary>
    /// �ر�
    /// </summary>
    public static void Close()
    {
        if(socket == null || !socket.Connected)
        {
            //socketû�ж��󣬻���socket�Ѿ��Ͽ����ӣ��Ͳ���Ҫ�ر�
            return;
        }
        if (isConnecting)
        {
            //socket���������У����ܹر�
            return;
        }
        if (messageQueue.Count > 0)
        {
            //��Ϣ�����л���δ���͵���Ϣ�����ܹر�
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
    /// ��ʼ��
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
