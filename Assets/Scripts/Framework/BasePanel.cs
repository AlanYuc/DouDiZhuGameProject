using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    /// <summary>
    /// 加载路径
    /// </summary>
    public string skinPath;
    /// <summary>
    /// 面板
    /// </summary>
    public GameObject skin;
    /// <summary>
    /// 面板所在层级，默认为PanelManager.Layer.Panel
    /// </summary>
    public PanelManager.Layer layer = PanelManager.Layer.Panel;

    public void Init()
    {
        //根据skinPath路径，加载GameObject资源，并实例化
        skin = Instantiate(Resources.Load<GameObject>(skinPath));
    }

    /// <summary>
    /// 在初始化之前，先获取skinPath，也就是面板的预制体在资源文件夹的位置
    /// </summary>
    public virtual void OnInit()
    {
        
    }
    public virtual void OnShow(params object[] para)
    {

    }

    public virtual void OnClose()
    {

    }

    public void Close()
    {
        string name = GetType().ToString();
        PanelManager.Close(name);
    }
}
