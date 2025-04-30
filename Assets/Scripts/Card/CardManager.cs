using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;

public class CardManager
{
    /// <summary>
    /// 玩家手牌的数量上限
    /// </summary>
    public static int maxHandSize = 17;
    public static Dictionary<string,Card> nameCards = new Dictionary<string,Card>();

    public static void Init()
    {
        //四种花色
        for (int i = 1; i <= 4; i++)
        {
            //13种牌面大小
            for (int j = 0; j < 13; j++)
            {
                Card card = new Card(i, j);
                string name = card.suit.ToString() + card.rank.ToString();
                nameCards.Add(name, card);
            }
        }

        //添加大小王
        nameCards.Add("SJoker", new Card(Suit.None, Rank.SJoker));
        nameCards.Add("LJoker", new Card(Suit.None, Rank.LJoker));
    }

    public static string GetName(Card card)
    {
        foreach(string s in nameCards.Keys)
        {
            if (nameCards[s].suit == card.suit && nameCards[s].rank == card.rank) 
            {
                return s;
            }
        }
        return "";
    }

    public static Card GetCard(string name)
    {
        if (nameCards.ContainsKey(name))
        {
            return nameCards[name];
        }
        return null;
    }

    /// <summary>
    /// 卡牌排序
    /// </summary>
    /// <param name="cards">需要排序的卡牌数组</param>
    /// <returns></returns>
    public static Card[] CardSort(Card[] cards)
    {
        Card temp;
        for (int i= 0; i < cards.Length; i++)
        {
            for (int j = cards.Length - 1; j > i; j--) 
            {
                if (cards[j].rank > cards[j - 1].rank)
                {
                    temp = cards[j];
                    cards[j] = cards[j - 1];
                    cards[j - 1] = temp;
                }
            }
        }
        return cards;
    }
}

