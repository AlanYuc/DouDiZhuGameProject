using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TipPanel : BasePanel
{

    public Text text;

    public override void OnInit()
    {
        skinPath = "TipPanel";
        layer = PanelManager.Layer.Tip;
    }

    public override void OnShow(params object[] para)
    {
        text = skin.transform.Find("Text").GetComponent<Text>();

        if(para.Length >= 1)
        {
            text.text = para[0] as string;
        }
    }

    public override void OnClose()
    {
        
    }

    private void Start()
    {
        StartCoroutine(Effect());
    }

    /// <summary>
    /// 协程实现TipPanel消失的效果
    /// </summary>
    /// <returns></returns>
    IEnumerator Effect()
    {
        for(int i = 0; i < 100; i++)
        {
            skin.transform.position += Vector3.up * 0.5f;
            text.color -= new Color(0, 0, 0, 0.01f);
            yield return new WaitForSeconds(0.02f);
        }
        Close();
    }

}
