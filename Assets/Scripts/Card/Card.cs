using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 扑克牌的种类，包含四种花色（suits），大小王用无花色None表示
/// 命名的规则与资源文件夹内的资源相同，方便生成
/// </summary>
public enum Suit
{
    /// <summary>
    /// 无花色，用于表示大小王
    /// </summary>
    None,
    /// <summary>
    /// 梅花(Clover)
    /// </summary>
    Club,
    /// <summary>
    /// 方片Diamond
    /// </summary>
    Square,
    /// <summary>
    /// 红桃
    /// </summary>
    Heart,
    /// <summary>
    /// 黑桃(Spade)
    /// </summary>
    Spade,
}

/// <summary>
/// 扑克牌牌面的权值，通过枚举大小直接比较大小
/// </summary>
public enum Rank
{
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    One,
    Two,
    SJoker,
    LJoker,
}

public class Card
{
    public Suit suit;
    public Rank rank;

    public Card(Suit suit, Rank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }

    public Card(int suit, int rank)
    {
        this.suit = (Suit)suit;
        this.rank = (Rank)rank;
    }

    /// <summary>
    /// Card转成CardInfo
    /// </summary>
    /// <returns></returns>
    public CardInfo GetCardInfo()
    {
        CardInfo cardInfo = new CardInfo();
        cardInfo.suit = (int)suit;
        cardInfo.rank = (int)rank;
        return cardInfo;
    }
}

