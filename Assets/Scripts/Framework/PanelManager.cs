using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Panel层级
/// </summary>
public static class PanelManager
{
    public enum Layer
    {
        Panel,
        Tip,
    }

    /// <summary>
    /// 层级列表
    /// </summary>
    private static Dictionary<Layer,Transform> layers = new Dictionary<Layer,Transform>();
    /// <summary>
    /// 面板列表，储存当前已打开的面板
    /// </summary>
    private static Dictionary<string,BasePanel> panels = new Dictionary<string,BasePanel>();
    /// <summary>
    /// 根目录
    /// </summary>
    private static Transform root;
    /// <summary>
    /// 画布
    /// </summary>
    private static Transform canvas;

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        root = GameObject.Find("Root").transform;
        canvas = root.Find("Canvas");
        layers.Add(Layer.Panel, canvas.Find("Panel"));
        layers.Add(Layer.Tip, canvas.Find("Tip"));
    }

    /// <summary>
    /// 打开面板
    /// </summary>
    /// <typeparam name="T">面板的脚本类型，继承自BasePanel</typeparam>
    /// <param name="para"></param>
    public static void Open<T>(params object[] para)where T : BasePanel
    {
        //判断面板是否已经打开
        string panelName = typeof(T).ToString();
        if (panels.ContainsKey(panelName))
        {
            //已经有了说明已经打开
            return;
        }

        //首先挂载BasePanel脚本
        //BasePanel panel = root.gameObject.GetComponent<BasePanel>();
        ////避免重复添加BasePanel组件
        //if (panel == null)
        //{
        //    panel = root.gameObject.AddComponent<BasePanel>();
        //}

        //首先挂载T脚本
        BasePanel panel = root.gameObject.AddComponent<T>();
        panel.OnInit();
        panel.Init();

        //获取panel所处的层级
        Transform layer = layers[panel.layer];
        //将panel对应的skin面板设置为所处层级的子对象
        panel.skin.transform.SetParent(layer,false);
        //添加到面板字典
        panels.Add(panelName, panel);
        //显示面板
        panel.OnShow(para);
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    /// <param name="panelName">需要关闭的面板对象的名字</param>
    public static void Close(string panelName)
    {
        //关闭前先判断是否已经打开，只有打开了才会执行关闭
        if (!panels.ContainsKey(panelName))
        {
            return;
        }

        BasePanel panel = panels[panelName];
        //关闭面板
        panel.OnClose();
        //移出字典
        panels.Remove(panelName);
        //先根据panel的脚本删除对应的面板对象
        GameObject.Destroy(panel.skin);
        //再把panel的脚本移除
        GameObject.Destroy(panel);
    }
}
