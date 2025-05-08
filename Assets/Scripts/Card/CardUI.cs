using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour,IPointerDownHandler,IPointerEnterHandler
{
    private Card card;
    private Image image;
    private bool isSelect;
    private bool isUp;
    private Color darkColor;
    private Color lightColor;

    /// <summary>
    /// 卡牌是否被选中的状态
    /// </summary>
    public bool IsSelect { 
        get => isSelect;
        set
        {
            isSelect = value;
            if (isSelect)
            {
                //卡牌被选中时，显示更暗的颜色
                image.color = darkColor;
            }
            else
            {
                //卡牌被取消选中时，恢复正常颜色
                image.color = lightColor;
            }
        }
    }

    /// <summary>
    /// 卡牌是否升起
    /// </summary>
    public bool IsUp { 
        get => isUp; 
        set => isUp = value; 
    }

    private void Start()
    {
        card = CardManager.GetCard(gameObject.name);
        image = gameObject.GetComponent<Image>();

        darkColor = new Color(0.6f, 0.6f, 0.6f, 1.0f);
        lightColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    private void Update()
    {
        //鼠标弹起可以在任何地方，不一定在卡牌内，因此不写在接口内而是Update。
        if (Input.GetMouseButtonUp(0))
        {
            GameManager.isPressing = false;
            if (IsSelect)
            {
                IsSelect = false;
                if (IsUp)
                {
                    //牌如果已经升起，就还原，并且把牌从已选择的手牌中去除
                    transform.position -= Vector3.up * 6;
                    IsUp = false;
                    if (GameManager.selectCards.Contains(card))
                    {
                        GameManager.selectCards.Remove(card);
                    }
                }
                else
                {
                    transform.position += Vector3.up * 6;
                    IsUp = true;
                    if (!GameManager.selectCards.Contains(card))
                    {
                        GameManager.selectCards.Add(card);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 鼠标点击
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.isPressing = true;

        //切换状态
        if (IsSelect)
        {
            IsSelect = false;
        }
        else
        {
            IsSelect = true;
        }
    }

    /// <summary>
    /// 鼠标进入卡牌
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.isPressing)
        {
            //如果鼠标按下的情况下进入卡牌，说明是进行连续选中的操作
            if (IsSelect)
            {
                IsSelect = false;
            }
            else
            {
                IsSelect = true;
            }
        }
    }
}
