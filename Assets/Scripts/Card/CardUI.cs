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
    /// �����Ƿ�ѡ�е�״̬
    /// </summary>
    public bool IsSelect { 
        get => isSelect;
        set
        {
            isSelect = value;
            if (isSelect)
            {
                //���Ʊ�ѡ��ʱ����ʾ��������ɫ
                image.color = darkColor;
            }
            else
            {
                //���Ʊ�ȡ��ѡ��ʱ���ָ�������ɫ
                image.color = lightColor;
            }
        }
    }

    /// <summary>
    /// �����Ƿ�����
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
        //��굯��������κεط�����һ���ڿ����ڣ���˲�д�ڽӿ��ڶ���Update��
        if (Input.GetMouseButtonUp(0))
        {
            GameManager.isPressing = false;
            if (IsSelect)
            {
                IsSelect = false;
                if (IsUp)
                {
                    //������Ѿ����𣬾ͻ�ԭ�����Ұ��ƴ���ѡ���������ȥ��
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
    /// �����
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.isPressing = true;

        //�л�״̬
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
    /// �����뿨��
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (GameManager.isPressing)
        {
            //�����갴�µ�����½��뿨�ƣ�˵���ǽ�������ѡ�еĲ���
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
