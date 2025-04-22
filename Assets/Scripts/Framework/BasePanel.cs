using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    /// <summary>
    /// ����·��
    /// </summary>
    public string skinPath;
    /// <summary>
    /// ���
    /// </summary>
    public GameObject skin;
    /// <summary>
    /// ������ڲ㼶��Ĭ��ΪPanelManager.Layer.Panel
    /// </summary>
    public PanelManager.Layer layer = PanelManager.Layer.Panel;

    public void Init()
    {
        //����skinPath·��������GameObject��Դ����ʵ����
        skin = Instantiate(Resources.Load<GameObject>(skinPath));
    }

    public virtual void OnInit()
    {
        
    }
    public virtual void OnShow(params object[] para)
    {

    }

    public virtual void OnClose()
    {

    }
}
