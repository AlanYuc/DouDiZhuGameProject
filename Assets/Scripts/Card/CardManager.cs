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

    /// <summary>
    /// 出牌的所有类型
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// 单张
        /// </summary>
        SINGLE,
        /// <summary>
        /// 对子
        /// </summary>
        PAIR,
        /// <summary>
        /// 三张
        /// </summary>
        TRIPLE,
        /// <summary>
        /// 三带一
        /// </summary>
        TRIPLE_WITH_SINGLE,
        /// <summary>
        /// 三带二
        /// </summary>
        TRIPLE_WITH_PAIR,
        /// <summary>
        /// 顺子
        /// </summary>
        STRAIGHT,
        /// <summary>
        /// 连对
        /// </summary>
        STRAIGHT_PAIRS,
        /// <summary>
        /// 飞机
        /// </summary>
        AIRPLANE,
        /// <summary>
        /// 飞机带单张，333 444 5 6
        /// </summary>
        AIRPLANE_WITH_SINGLES,
        /// <summary>
        /// 飞机带对子，333 444 55 66
        /// </summary>
        AIRPLANE_WITH_PAIRS,
        /// <summary>
        /// 炸弹
        /// </summary>
        BOMB,
        /// <summary>
        /// 王炸
        /// </summary>
        JOKER_BOMB,
        /// <summary>
        /// 四代二
        /// </summary>
        FOUR_WITH_TWO,
        /// <summary>
        /// 无效出牌
        /// </summary>
        INVALID,
    }

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

    /// <summary>
    /// Card数组转成CardInfo数组
    /// </summary>
    /// <param name="cards"></param>
    /// <returns></returns>
    public static CardInfo[] GetCardInfos(Card[] cards)
    {
        CardInfo[] cardInfos = new CardInfo[cards.Length];
        for (int i = 0; i < cardInfos.Length; i++)
        {
            cardInfos[i] = cards[i].GetCardInfo();
        }
        return cardInfos;
    }

    /// <summary>
    /// CardInfo数组转成Card数组
    /// </summary>
    /// <param name="cardInfos"></param>
    /// <returns></returns>
    public static Card[] GetCards(CardInfo[] cardInfos)
    {
        Card[] cards = new Card[cardInfos.Length];
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = new Card(cardInfos[i].suit, cardInfos[i].rank);
        }
        return cards;
    }
}

