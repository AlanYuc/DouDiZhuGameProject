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
    /// ��Ϣ�б�
    /// </summary>
    private static List<MsgBase> msgList;
    /// <summary>
    /// ��Ϣ�б���
    /// </summary>
    private static int msgCuont;
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
    /// ��Ϣ�¼�ί��
    /// </summary>
    /// <param name="msgBase"></param>
    public delegate void MsgListener(MsgBase msgBase);
    /// <summary>
    /// ��Ϣ�¼��б�
    /// </summary>
    public static Dictionary<string,MsgListener> msgListeners = new Dictionary<string, MsgListener>();
    /// <summary>
    /// �����Ϣ�¼�����
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
    /// �Ƴ���Ϣ�¼�
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
    /// �ַ���Ϣ�¼�
    /// </summary>
    /// <param name="msgName"></param>
    /// <param name="msgBase"></param>
    public static void FireMsgListener(string msgName, MsgBase msgBase)
    {
        if (msgListeners.ContainsKey(msgName))
        {
            msgListeners[msgName](msgBase);
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
            Socket socket = ar.AsyncState as Socket;
            socket.EndConnect(ar);
            Debug.Log("���ӳɹ�");
            isConnecting = false;

            FireEvent(NetEvent.ConnectSuccess, "");
            //����
            socket.BeginReceive(byteArray.bytes, byteArray.writeIndex, byteArray.Remain, SocketFlags.None, ReceiveCallBack, socket);
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

        msgList = new List<MsgBase>();
        msgCuont = 0;
    }
    /// <summary>
    /// ������Ϣ
    /// </summary>
    /// <param name="msg">��Ϣ</param>
    public static void Send(MsgBase msg)
    {
        if(socket == null || !socket.Connected)
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

        //����
        byte[] nameBytes = MsgBase.EncodeProtocolName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        //����Ϣǰ����һ��int�Ĵ�С�����ڷְ�ճ���Ĵ���
        int indexNum = sizeof(int) + nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[indexNum];
        int index = 0;
        BitConverter.GetBytes(indexNum).CopyTo(sendBytes, index);
        index += sizeof(int);
        nameBytes.CopyTo(sendBytes, index);
        index += nameBytes.Length;
        bodyBytes.CopyTo(sendBytes, index);
        index += bodyBytes.Length;

        //����������Ϣ�ֽڷŽ�ByteArray��messageQueue��Ϣ������
        ByteArray byteArray = new ByteArray(sendBytes);
        //�ص��������¿����̣߳���Ҫ��ס��Ϣ���У���ֹͬʱ����
        int count = 0;
        lock (messageQueue)
        {
            messageQueue.Enqueue(byteArray);
            count = messageQueue.Count;
        }
        if(count == 1)
        {
            //������Ϣ
            socket.BeginSend(sendBytes, 0, sendBytes.Length, SocketFlags.None, SendCallBack, socket);
        }
    }
    /// <summary>
    /// ���ͻص�����
    /// </summary>
    /// <param name="ar"></param>
    public static void SendCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            //EndSend���ط����˶����ֽ�
            int count = socket.EndSend(ar);
            lock (messageQueue)
            {
                ByteArray byteArray = messageQueue.Peek();
            }
            //����ǰ����Ϣ�����˶��٣��������ͻ��Ľ��ŷ�
            byteArray.readIndex += count;
            if(byteArray.Length == 0)
            {
                lock (messageQueue)
                {
                    //��Ϣȫ��������,����Ϣ�Ƴ�
                    messageQueue.Dequeue();
                    //�ж���һ����Ϣ
                    byteArray = messageQueue.Peek();
                }
            }
            //��������
            if (byteArray != null)
            {
                socket.BeginSend(byteArray.bytes, byteArray.readIndex, byteArray.Length, SocketFlags.None, SendCallBack, socket);
            }
            else if (isClosing)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
        }
        catch (SocketException ex)
        {
            Debug.Log("����ʧ�ܣ�" + ex.Message);
        }
    }
    /// <summary>
    /// ���ջص�����
    /// </summary>
    /// <param name="ar"></param>
    public static void ReceiveCallBack(IAsyncResult ar)
    {
        try
        {
            Socket socket = ar.AsyncState as Socket;
            int count = socket.EndReceive(ar);
            //û����Ϣ�ɽ����ˣ��Ͽ�����
            if(count == 0)
            {
                Close();
                return;
            }
            //�ɹ�����
            byteArray.writeIndex += count;
            OnReceiveData();
            //byteArray���ȹ�С����Ҫ����
            if (byteArray.Remain < 8)
            {
                byteArray.MoveBytes();
                byteArray.Resize(byteArray.capacity * 2);
            }
            //����������Ϣ
            socket.BeginReceive(byteArray.bytes, byteArray.writeIndex, byteArray.Remain, SocketFlags.None, ReceiveCallBack, socket);
        }
        catch (SocketException ex)
        {
            Debug.Log("����ʧ�ܣ�" + ex.Message);
        }
    }
    /// <summary>
    /// �������������Ϣ
    /// </summary>
    public static void OnReceiveData()
    {

    }
}
