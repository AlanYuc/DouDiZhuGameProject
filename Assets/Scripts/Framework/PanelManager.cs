using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Panel�㼶
/// </summary>
public static class PanelManager
{
    public enum Layer
    {
        Panel,
        Tip,
    }

    /// <summary>
    /// �㼶�б�
    /// </summary>
    private static Dictionary<Layer,Transform> layers = new Dictionary<Layer,Transform>();
    /// <summary>
    /// ����б����浱ǰ�Ѵ򿪵����
    /// </summary>
    private static Dictionary<string,BasePanel> panels = new Dictionary<string,BasePanel>();
    /// <summary>
    /// ��Ŀ¼
    /// </summary>
    private static Transform root;
    /// <summary>
    /// ����
    /// </summary>
    private static Transform canvas;

    /// <summary>
    /// ��ʼ��
    /// </summary>
    public static void Init()
    {
        root = GameObject.Find("Root").transform;
        canvas = root.Find("Canvas");
        layers.Add(Layer.Panel, canvas.Find("Panel"));
        layers.Add(Layer.Tip, canvas.Find("Tip"));
    }

    /// <summary>
    /// �����
    /// </summary>
    /// <typeparam name="T">���Ľű����ͣ��̳���BasePanel</typeparam>
    /// <param name="para"></param>
    public static void Open<T>(params object[] para)where T : BasePanel
    {
        //�ж�����Ƿ��Ѿ���
        string panelName = typeof(T).ToString();
        if (panels.ContainsKey(panelName))
        {
            //�Ѿ�����˵���Ѿ���
            return;
        }

        //���ȹ���BasePanel�ű�
        //BasePanel panel = root.gameObject.GetComponent<BasePanel>();
        ////�����ظ����BasePanel���
        //if (panel == null)
        //{
        //    panel = root.gameObject.AddComponent<BasePanel>();
        //}

        //���ȹ���T�ű�
        BasePanel panel = root.gameObject.AddComponent<T>();
        panel.OnInit();
        panel.Init();

        //��ȡpanel�����Ĳ㼶
        Transform layer = layers[panel.layer];
        //��panel��Ӧ��skin�������Ϊ�����㼶���Ӷ���
        panel.skin.transform.SetParent(layer,false);
        //��ӵ�����ֵ�
        panels.Add(panelName, panel);
        //��ʾ���
        panel.OnShow(para);
    }

    /// <summary>
    /// �ر����
    /// </summary>
    /// <param name="panelName">��Ҫ�رյ������������</param>
    public static void Close(string panelName)
    {
        //�ر�ǰ���ж��Ƿ��Ѿ��򿪣�ֻ�д��˲Ż�ִ�йر�
        if (!panels.ContainsKey(panelName))
        {
            return;
        }

        BasePanel panel = panels[panelName];
        //�ر����
        panel.OnClose();
        //�Ƴ��ֵ�
        panels.Remove(panelName);
        //�ȸ���panel�Ľű�ɾ����Ӧ��������
        GameObject.Destroy(panel.skin);
        //�ٰ�panel�Ľű��Ƴ�
        GameObject.Destroy(panel);
    }
}
