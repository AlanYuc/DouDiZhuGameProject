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
    public static bool isConnecting;
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
    /// һ֡������Ϣ����󳤶�
    /// </summary>
    private static int processMsgCount;
    /// <summary>
    /// �Ƿ�������������
    /// </summary>
    public static bool isUsePingPong;
    /// <summary>
    /// ����ʱ����
    /// </summary>
    public static int pingInterval;
    /// <summary>
    /// ��һ�η���PingЭ���ʱ��
    /// </summary>
    private static float lastPingTime;
    /// <summary>
    /// ��һ���յ�PongЭ���ʱ��
    /// </summary>
    private static float lastPongTime;
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
    public static void FireMsg(string msgName, MsgBase msgBase)
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
    /// ������Ϣ
    /// </summary>
    /// <param name="msg">��Ϣ</param>
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

        //����
        byte[] nameBytes = MsgBase.EncodeProtocolName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        //����Ϣǰ����һ��int�Ĵ�С�����ڷְ�ճ���Ĵ���
        int indexNum = sizeof(int) + nameBytes.Length + bodyBytes.Length;
        byte[] sendBytes = new byte[indexNum];
        int index = 0;
        BitConverter.GetBytes(nameBytes.Length + bodyBytes.Length).CopyTo(sendBytes, index);
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
        if (count == 1)
        {
            //������Ϣ
            socket.BeginSend(sendBytes, 0, sendBytes.Length, SocketFlags.None, SendCallBack, socket);
            Debug.Log("��Ϣ���ͳɹ�");
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
            if(socket == null || !socket.Connected)
            {
                return;
            }
            //EndSend���ط����˶����ֽ�
            int count = socket.EndSend(ar);
            
            //ByteArray ba;Ҫ��lock���洴������Ȼ�������
            ByteArray ba;
            lock (messageQueue)
            {
                ba = messageQueue.Peek();
            }
            //����ǰ����Ϣ�����˶��٣��������ͻ��Ľ��ŷ�
            ba.readIndex += count;
            if (ba.Length == 0)
            {
                lock (messageQueue)
                {
                    //��Ϣȫ��������,����Ϣ�Ƴ�
                    messageQueue.Dequeue();
                    //�ж���һ����Ϣ
                    ba = messageQueue.Peek();
                }
            }
            //��������
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
        if (byteArray.Length < sizeof(int))
        {
            //��ȡ�����ݳ���С��4��Ҳ������һ����ʼ��int���ݣ��ְ�ճ��������������
            return;
        }
        int readIndex = byteArray.readIndex;

        //������һ���ְ�ճ�������ݣ�Ҳ�������ݵ���������
        int bodyLength = BitConverter.ToInt32(byteArray.bytes, readIndex);
        if(byteArray.Length < sizeof(int) + bodyLength)
        {
            //���ְַ��������������û������
            return;
        }

        //������Ϣ��
        readIndex += sizeof(int);
        int nameCount;
        string protocolName = MsgBase.DecodeProtocolName(byteArray.bytes, readIndex, out nameCount);
        readIndex += nameCount;
        if(protocolName == "")
        {
            Debug.Log("������Ϣ��ʧ��");
            return;
        }

        //������Ϣ��
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
    /// ��Ϣ��������Ϣ�Ŵ�msgList�д��������ŵ�Update��ִ��
    /// </summary>
    public static void MsgUpdate()
    {
        if(msgCuont == 0)
        {
            //û����Ϣ
            return;
        }
        for(int i = 0; i < processMsgCount; i++)
        {
            MsgBase msgBase = null;
            lock (msgList)
            {
                if (msgCuont > 0)
                {
                    //ȡ����һ����Ϣ
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
    /// ����PingЭ��
    /// </summary>
    private static void PingUpdate()
    {
        if (!isUsePingPong)
        {
            //û�����������ƣ�ֱ�ӷ���
            return;
        }
        //����Э��
        if (Time.time - lastPingTime > pingInterval)
        {
            MsgPing msgPing = new MsgPing();
            Send(msgPing);
            lastPingTime = Time.time;
        }

        //��pingInterval * 4�Ĺ̶�ʱ���ڽ��ղ���PongЭ�飬�ͶϿ�
        if(Time.time - lastPongTime> pingInterval * 4)
        {
            Debug.Log("PingUpdate: ����ʱ��������ͻ��������Ͽ�����");
            Close();
        }
    }
    /// <summary>
    /// ��Ҫ���ڽű��е�Update�е��ã�ȷ��ÿִ֡��
    /// </summary>
    public static void Update()
    {
        MsgUpdate();
        PingUpdate();
    }
    /// <summary>
    /// ���յ�Pong��Ϣ��
    /// </summary>
    /// <param name="msgBase"></param>
    public static void OnMsgPong(MsgBase msgBase)
    {
        lastPongTime = Time.time;
    }
}
